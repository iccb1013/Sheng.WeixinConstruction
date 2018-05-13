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


using Linkup.Common;
using Linkup.Data;
using Linkup.DataRelationalMapping;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class MemberGroupManager
    {
        private static readonly MemberGroupManager _instance = new MemberGroupManager();
        public static MemberGroupManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private static LogService _log = LogService.Instance;

        private MemberGroupManager()
        {

        }

        /// <summary>
        /// 刷新指定域下面分组信息，将调用微信API获取最新分组信息
        /// </summary>
        /// <param name="domainContext"></param>
        public void SyncGroupList(DomainContext domainContext)
        {
            RequestApiResult<WeixinGetGroupListResult> result = GroupApiWrapper.GetGroupList(domainContext);
            if (result.Success)
            {
                List<CommandParameter> parameterList = new List<CommandParameter>();
                parameterList.Add(new CommandParameter("@domain", domainContext.Domain.Id));
                parameterList.Add(new CommandParameter("@appId", domainContext.AppId));

                _dataBase.ExecuteNonQuery("DELETE FROM [MemberGroup] WHERE [Domain] = @domain AND [AppId] = @appId",
                   parameterList);

                foreach (WeixinGetGroupListResult_Group item in result.ApiResult.GroupList)
                {
                    MemberGroupEntity group = new MemberGroupEntity();
                    group.GroupId = item.Id;
                    group.Name = item.Name;
                    group.Domain = domainContext.Domain.Id;
                    group.AppId = domainContext.AppId;
                    _dataBase.Insert(group);
                }

                //过滤用户，将已不存在的分组置为0
            }
            else
            {
                _log.Write("RefreshGroupList 失败", result.Message, TraceEventType.Warning);
            }
        }

        public List<MemberGroupEntity> GetMemberGroupList(Guid domainId, string appId)
        {
            List<AttachedWhereItem> attachedWhere = new List<AttachedWhereItem>();
            attachedWhere.Add(new AttachedWhereItem("Domain", domainId));
            attachedWhere.Add(new AttachedWhereItem("AppId", appId));

            List<MemberGroupEntity> list = _dataBase.Select<MemberGroupEntity>(attachedWhere);
            return list;
        }

        public MemberGroupEntity GetMemberGroup(Guid id)
        {
            MemberGroupEntity memberGroup = new MemberGroupEntity();
            memberGroup.Id = id;

            if (_dataBase.Fill<MemberGroupEntity>(memberGroup))
                return memberGroup;
            else
                return null;
        }

        public MemberGroupEntity GetMemberGroup(Guid domainId, string appId, int groupId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domain", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@groupId", groupId));

            List<MemberGroupEntity> memberGroupList = _dataBase.Select<MemberGroupEntity>(
                "SELECT * FROM [MemberGroup] WHERE [Domain] = @domain AND [AppID] = @appId AND [GroupId] = @groupId", parameterList);
            if (memberGroupList.Count == 0)
                return null;
            else
                return memberGroupList[0];
        }

        public string CreateMemberGroup(DomainContext domainContext, MemberGroupEntity group)
        {
            if (group == null)
            {
                return "参数错误。";
            }

            //先往微信后台添加，成功后写数据库
            WeixinCreateGroupArgs apiArgs = new WeixinCreateGroupArgs();
            apiArgs.Group.Name = group.Name;
            RequestApiResult<WeixinGroup> createResult = GroupApiWrapper.Create(domainContext, apiArgs);
            if (createResult.Success == false)
            {
                return createResult.Message;
            }

            group.Domain = domainContext.Domain.Id;
            group.GroupId = createResult.ApiResult.Group.Id;
            _dataBase.Insert(group);

            return null;
        }

        public string UpdateMemberGroup(DomainContext domainContext, MemberGroupEntity group)
        {
            if (group == null)
            {
                return "参数错误。";
            }

            //先往微信后台更新，成功后写数据库
            WeixinGroup apiArgs = new WeixinGroup();
            apiArgs.Group.Id = group.GroupId;
            apiArgs.Group.Name = group.Name;
            RequestApiResult updateResult = GroupApiWrapper.Update(domainContext, apiArgs);
            if (updateResult.Success == false)
            {
                return updateResult.Message;
            }

            _dataBase.Update(group);

            return null;
        }

        public string RemoveMemberGroup(DomainContext domainContext, Guid id)
        {
            MemberGroupEntity memberGroup = GetMemberGroup(id);
            if (memberGroup == null)
                return "分组不存在。";

            //先往微信后台更新，成功后写数据库
            RequestApiResult removeResult = GroupApiWrapper.Remove(domainContext, memberGroup.GroupId);
            if (removeResult.Success == false)
            {
                return removeResult.Message;
            }

            //将此分组下的用户分到默认分组下面
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domain", domainContext.Domain.Id));
            parameterList.Add(new CommandParameter("@appId", domainContext.AppId));
            parameterList.Add(new CommandParameter("@groupId", memberGroup.GroupId));

            _dataBase.ExecuteNonQuery(
                "UPDATE [Member] SET [GroupId] = 0 WHERE [Domain] = @domain AND [AppId] = @appId AND [GroupId] = @groupId",
                parameterList);

            _dataBase.Remove(memberGroup);

            return null;
        }

        public string MoveMemberListToGroup(DomainContext domainContext, MoveMemberListToGroupArgs args)
        {
            WeixinSetUserListGroupArgs weixinArgs = new WeixinSetUserListGroupArgs();
            weixinArgs.OpenIdList = args.OpenIdList;
            weixinArgs.GroupId = args.GroupId;

            //先往微信后台更新，成功后写数据库
            RequestApiResult removeResult = GroupApiWrapper.SetUserListGroup(domainContext, weixinArgs);
            if (removeResult.Success == false)
            {
                return removeResult.Message;
            }

            List<SqlExpression> _sqlList = new List<SqlExpression>();
            //将此分组下的用户分到默认分组下面
            foreach (var openId in args.OpenIdList)
            {
                SqlStructureBuild sqlBuild = new SqlStructureBuild();
                sqlBuild.Type = SqlExpressionType.Update;
                sqlBuild.Table = "Member";
                sqlBuild.AddParameter("Domain", domainContext.Domain.Id, true);
                sqlBuild.AddParameter("AppId", domainContext.AppId, true);
                sqlBuild.AddParameter("OpenId", openId, true);
                sqlBuild.AddParameter("GroupId", args.GroupId);
                _sqlList.Add(sqlBuild.GetSqlExpression());
            }

            _dataBase.ExcuteSqlExpression(_sqlList);

            return null;
        }
    }
}
