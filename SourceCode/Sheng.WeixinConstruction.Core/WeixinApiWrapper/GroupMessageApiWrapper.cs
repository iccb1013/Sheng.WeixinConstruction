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
    public class GroupMessageApiWrapper
    {
        private static LogService _log = LogService.Instance;

        public static RequestApiResult<WeixinGroupMessageSendAllResult> SendAll(DomainContext domainContext, WeixinGroupMessageSendAllArgs args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinGroupMessageSendAllResult> result = GroupMessageApi.SendAll(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = GroupMessageApi.SendAll(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("GroupMessageApi.SendAll 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("GroupMessageApi.SendAll 失败",
                              result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult<WeixinGroupMessageSendResult> Send(DomainContext domainContext, WeixinGroupMessageSendArgs args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinGroupMessageSendResult> result = GroupMessageApi.Send(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = GroupMessageApi.Send(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("GroupMessageApi.Send 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("GroupMessageApi.Send 失败",
                           result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult<WeixinGroupMessagePreviewResult> Preview(DomainContext domainContext, WeixinGroupMessagePreviewArgs args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinGroupMessagePreviewResult> result = GroupMessageApi.Preview(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = GroupMessageApi.Preview(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("GroupMessageApi.Preview 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("GroupMessageApi.Preview 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }
    }
}
