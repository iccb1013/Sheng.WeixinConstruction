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
    public class TokenApiWrapper
    {
        private static LogService _log = LogService.Instance;

        public static RequestApiResult<WeixinGetJsApiTicketResult> GetJsApiTicket(string appId)
        {
            string accessToken = AccessTokenGetter.Get(appId);
            RequestApiResult<WeixinGetJsApiTicketResult> result = TokenApi.GetJsApiTicket(accessToken);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(appId, accessToken);
                    }

                    result = TokenApi.GetJsApiTicket(accessToken);
                    if (result.Success == false)
                    {
                        _log.Write("TokenApi.GetJsApiTicket 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("TokenApi.GetJsApiTicket 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }
             else
            {
                //保存新的jsApiTicket到数据库
                WeixinGetJsApiTicketResult token = result.ApiResult;
                ThirdPartyManager.Instance.UpdateAuthorizerJsApiTicket(
                    appId, token.Ticket, DateTime.Now.AddSeconds(token.ExpiresIn));
            }

            return result;
        }
    }
}
