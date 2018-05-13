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
using Newtonsoft.Json;
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
    public class GroupApiWrapper
    {
        private static LogService _log = LogService.Instance;

        public static RequestApiResult<WeixinGroup> Create(DomainContext domainContext, WeixinCreateGroupArgs args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinGroup> result = GroupApi.Create(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = GroupApi.Create(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("GroupApi.Create 失败",
                             result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("GroupApi.Create 失败",
                             result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult<WeixinGetGroupListResult> GetGroupList(DomainContext domainContext)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinGetGroupListResult> result = GroupApi.GetGroupList(accessToken);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = GroupApi.GetGroupList(accessToken);
                    if (result.Success == false)
                    {
                        _log.Write("ThirdPartyApi.GetGroupList 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("ThirdPartyApi.GetGroupList 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult<WeixinGetUserGroupIdResult> GetUserGroupId(DomainContext domainContext, string openid)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinGetUserGroupIdResult> result = GroupApi.GetUserGroupId(accessToken, openid);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = GroupApi.GetUserGroupId(accessToken, openid);
                    if (result.Success == false)
                    {
                        _log.Write("GroupApi.GetUserGroupId 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("GroupApi.GetUserGroupId 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult Update(DomainContext domainContext, WeixinGroup args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult result = GroupApi.Update(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = GroupApi.Update(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("GroupApi.Update 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("GroupApi.Update 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult SetUserGroup(DomainContext domainContext, WeixinSetUserGroupArgs args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult result = GroupApi.SetUserGroup(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = GroupApi.SetUserGroup(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("GroupApi.SetUserGroup 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("GroupApi.SetUserGroup 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult SetUserListGroup(DomainContext domainContext, WeixinSetUserListGroupArgs args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult result = GroupApi.SetUserListGroup(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = GroupApi.SetUserListGroup(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("GroupApi.SetUserListGroup 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("GroupApi.SetUserListGroup 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult Remove(DomainContext domainContext, int groupId)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult result = GroupApi.Remove(accessToken, groupId);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = GroupApi.Remove(accessToken, groupId);
                    if (result.Success == false)
                    {
                        _log.Write("GroupApi.Remove 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("GroupApi.Remove 失败",
                               result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

    }
}
