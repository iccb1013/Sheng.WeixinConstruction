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
    /// <summary>
    /// 发现存在 AccessToken 没到给出的7200秒就失效的现象
    /// 所以对使用了 AccessToken 的API 进行封装，在出现相应的 40001 错误时，能自动刷新 AccessToken 重试一次
    /// </summary>
    public static class UserApiWrapper
    {
        private static LogService _log = LogService.Instance;

        /// <summary>
        /// 用户管理-获取用户基本信息
        /// 未认证订阅号 未认证服务号 没有此权限
        /// </summary>
        /// <param name="domainContext"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinUser> GetUserInfo(DomainContext domainContext, string openId)
        {
            RequestApiResult<WeixinUser> result;

            if (domainContext == null || domainContext.Authorizer == null)
            {
                result = new RequestApiResult<WeixinUser>();
                result.ApiError = new WeixinApiErrorResult();
                result.ApiError.ErrorCode = -1;
                result.ApiError.ErrorMessage = "无域信息或无授权信息。";
                return result;
            }

            if (domainContext.Authorizer.AuthorizationType == EnumAuthorizationType.UnauthorizedService ||
                domainContext.Authorizer.AuthorizationType == EnumAuthorizationType.UnauthorizedSubscription)
            {
                result = new RequestApiResult<WeixinUser>();
                result.ApiError = new WeixinApiErrorResult();
                result.ApiError.ErrorCode = -1;
                result.ApiError.ErrorMessage = "未认证订阅号 及 未认证服务号 没有获取用户基本信息权限。";
                return result;
            }

            string accessToken = domainContext.AccessToken;
            result = UserApi.GetUserInfo(accessToken, openId);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = UserApi.GetUserInfo(accessToken, openId);
                    if (result.Success == false)
                    {
                        _log.Write("UserApi.GetUserInfo 失败",
                             result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("UserApi.GetUserInfo 失败",
                                result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        /// <summary>
        /// 用户管理-获取用户列表
        /// 未认证订阅号 未认证服务号 没有此权限
        /// </summary>
        /// <param name="domainContext"></param>
        /// <param name="next_openid"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinGetUserListResult> GetUserList(DomainContext domainContext, string next_openid)
        {
            RequestApiResult<WeixinGetUserListResult> result;
            if (domainContext.Authorizer.AuthorizationType == EnumAuthorizationType.UnauthorizedService ||
                domainContext.Authorizer.AuthorizationType == EnumAuthorizationType.UnauthorizedSubscription)
            {
                result = new RequestApiResult<WeixinGetUserListResult>();
                result.ApiError = new WeixinApiErrorResult();
                result.ApiError.ErrorCode = -1;
                result.ApiError.ErrorMessage = "未认证订阅号 及 未认证服务号 没有获取用户列表权限。";
                return result;
            }

            string accessToken = domainContext.AccessToken;
            result = UserApi.GetUserList(accessToken, next_openid);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = UserApi.GetUserList(accessToken, next_openid);
                    if (result.Success == false)
                    {
                        string logMessage = result.GetDetail() + Environment.NewLine + new StackTrace().ToString();
                        _log.Write("UserApi.GetUserList 失败", logMessage, TraceEventType.Warning);
                    }
                }
                else
                {
                    string logMessage = result.GetDetail() + Environment.NewLine + new StackTrace().ToString();
                    _log.Write("UserApi.GetUserList 失败", logMessage, TraceEventType.Warning);
                }
            }

            return result;
        }
    }
}
