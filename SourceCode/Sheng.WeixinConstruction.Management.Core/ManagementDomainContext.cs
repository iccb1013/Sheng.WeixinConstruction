/*
********************************************************************
*
*    曹旭升（sheng.c）
*    E-mail: cao.silhouette@msn.com
*    QQ: 279060597
*    https://github.com/iccb1013
*    http://shengxunwei.com
*
*    © Copyright 2016
*
********************************************************************/


using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Management.Core
{
    public class ManagementDomainContext : DomainContext
    {
        //如果是调试模式，则直接模拟一个用户而不请求微信接口
        private static readonly bool _debug = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["debug"]);

        private Task _syncUserListTask;
        private DateTime _syncFlag;

        /// <summary>
        /// 正在同步用户列表
        /// </summary>
        public bool SynchronizingUserList
        {
            get;
            set;
        }

        /// <summary>
        /// 正在同步分组列表
        /// </summary>
        public bool SynchronizingGroupList
        {
            get;
            set;
        }

        public ManagementDomainContext(DomainEntity domain)
            : base(domain)
        {
            //由于 程序更新频繁，加之更新时间很短，暂时取消在启动时同步
            //程序启动后第一次使用此公众号时，同步一下用户列表
            //if (this.Online && _debug == false)
            //{
            //    SyncMember();
            //}
        }

        /// <summary>
        /// 同步关注者及其分组信息
        /// </summary>
        public void SyncMember()
        {
            //未认证订阅号 未认证服务号 没有此权限
            //用户管理-用户分组管理
            //用户管理-获取用户列表
            //用户管理-获取用户基本信息
            if (Authorizer.AuthorizationType == EnumAuthorizationType.UnauthorizedService ||
               Authorizer.AuthorizationType == EnumAuthorizationType.UnauthorizedSubscription)
            {
                return;
            }

            SyncGroupList();
            SyncUserList();

        }

        #region 同步分组

        private void SyncGroupList()
        {
            lock (this)
            {
                if (SynchronizingGroupList)
                    return;

                SynchronizingGroupList = true;
            }

            MemberGroupManager.Instance.SyncGroupList(this);

            SynchronizingGroupList = false;
        }

        #endregion

        #region 同步用户列表

        /// <summary>
        /// 需要同步用户列表
        /// </summary>
        private void SyncUserList()
        {
            string logMessage = new StackTrace().ToString();
            _logService.Write("ManagementDomainContext.SyncUserList", logMessage, TraceEventType.Verbose);

            lock (this)
            {
                if (SynchronizingUserList)
                    return;

                SynchronizingUserList = true;
            }

            _syncUserListTask = new Task(SyncUserListTaskMethod);
            _syncUserListTask.ContinueWith((task) =>
            {
                if (task.IsFaulted)
                {
                    SynchronizingUserList = false;
                    if (task.Exception.InnerException != null)
                    {
                        _logService.Write("syncUserListTask 异常", task.Exception.InnerException.Message + "\r\n" +
                            task.Exception.InnerException.StackTrace, TraceEventType.Error);
                    }
                    else
                    {
                        _logService.Write("syncUserListTask 异常", task.Exception.Message + "\r\n" +
                            task.Exception.StackTrace, TraceEventType.Error);
                    }
                }
            });
            _syncFlag = DateTime.Now;
            _syncUserListTask.Start();
        }

        private void SyncUserListTaskMethod()
        {
            SyncUserList(null);
        }

        private void SyncUserList(string next_openid)
        {
            RequestApiResult<WeixinGetUserListResult> getUserListResult =
                UserApiWrapper.GetUserList(this, next_openid);
            if (getUserListResult.Success == false)
            {
                EndSyncUserList("用户列表同步失败：" + getUserListResult.Message);
                return;
            }

            if (getUserListResult.ApiResult.Count == 0)
            {
                //将取消了关注的用户剔出来
                _memberManager.UpdateUnsubscribeMemberList(this.Domain.Id, this.AppId, _syncFlag);

                EndSyncUserList("用户列表同步成功。");
                return;
            }

            foreach (string openId in getUserListResult.ApiResult.Data.OpenIdList)
            {
                RequestApiResult<WeixinUser> getUserInfoResult = UserApiWrapper.GetUserInfo(this, openId);
                if (getUserInfoResult.Success == false)
                {
                    _logService.Write("用户列表同步失败", getUserInfoResult.Message, TraceEventType.Error);

                    EndSyncUserList("用户列表同步失败：" + getUserInfoResult.Message);
                    return;
                }

                //值为0时，代表此用户没有关注该公众号，拉取不到其余信息。
                if (getUserInfoResult.ApiResult.Subscribe == 0)
                {
                    continue;
                }

                //根据OpenId获取用户信息
                MemberEntity member = _memberManager.GetMemberByOpenId(this.Domain.Id, this.AppId, openId);
                AddMemberArgs args = new AddMemberArgs();
                args.WeixinUser = getUserInfoResult.ApiResult;
                args.SyncFlag = _syncFlag;

                if (member == null)
                {
                    //添加新用户
                    member = _memberManager.AddMember(this, args);
                }
                else
                {
                    //更新当前用户信息
                    _memberManager.UpdateMember(member, args);
                }
            }

            //事实上微信接口永远会返回 NextOpenId，哪怕数据只有几条不需要分次获取
            if (String.IsNullOrEmpty(getUserListResult.ApiResult.NextOpenId) == false)
            {
                SyncUserList(getUserListResult.ApiResult.NextOpenId);
            }
        }

        private void EndSyncUserList(string message)
        {
            lock (this)
            {
                SynchronizingUserList = false;

                //  SyncUserListResult = message;
            }
        }

        #endregion
    }
}
