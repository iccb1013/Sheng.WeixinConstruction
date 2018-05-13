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
    public class TagsApiWrapper
    {
        private static LogService _log = LogService.Instance;

        public static RequestApiResult<WeixinTag> Create(DomainContext domainContext, WeixinCreateTagArgs args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinTag> result = TagsApi.Create(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = TagsApi.Create(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("TagsApi.Create 失败",
                             result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("TagsApi.Create 失败",
                             result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult<WeixinGetTagListResult> GetTagList(DomainContext domainContext)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinGetTagListResult> result = TagsApi.GetTagList(accessToken);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = TagsApi.GetTagList(accessToken);
                    if (result.Success == false)
                    {
                        _log.Write("TagsApi.GetTagList 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("TagsApi.GetTagList 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult Update(DomainContext domainContext, WeixinTag args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult result = TagsApi.Update(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = TagsApi.Update(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("TagsApi.Update 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("TagsApi.Update 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult Remove(DomainContext domainContext, int tagId)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult result = TagsApi.Remove(accessToken, tagId);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = TagsApi.Remove(accessToken, tagId);
                    if (result.Success == false)
                    {
                        _log.Write("TagsApi.Remove 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("TagsApi.Remove 失败",
                               result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult<WeixinGetTagUserListResult> GetTagUserList(DomainContext domainContext, int tagId, string next_openid)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinGetTagUserListResult> result = TagsApi.GetTagUserList(accessToken, tagId, next_openid);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = TagsApi.GetTagUserList(accessToken, tagId, next_openid);
                    if (result.Success == false)
                    {
                        _log.Write("TagsApi.GetTagUserList 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("TagsApi.GetTagUserList 失败",
                               result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult BatchTagging(DomainContext domainContext, WeixinTagBatchTaggingArgs args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult result = TagsApi.BatchTagging(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = TagsApi.BatchTagging(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("TagsApi.BatchTagging 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("TagsApi.BatchTagging 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult BatchUntagging(DomainContext domainContext, WeixinTagBatchTaggingArgs args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult result = TagsApi.BatchUntagging(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = TagsApi.BatchUntagging(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("TagsApi.BatchUntagging 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("TagsApi.BatchUntagging 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult<WeixinGetUserTagListResult> GetUserTagList(DomainContext domainContext, string openId)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinGetUserTagListResult> result = TagsApi.GetUserTagList(accessToken, openId);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = TagsApi.GetUserTagList(accessToken, openId);
                    if (result.Success == false)
                    {
                        _log.Write("TagsApi.GetUserTagList 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("TagsApi.GetUserTagList 失败",
                               result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

    }
}
