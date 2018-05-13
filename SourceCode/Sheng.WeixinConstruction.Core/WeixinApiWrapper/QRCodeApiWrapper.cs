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
    public class QRCodeApiWrapper
    {
        private static LogService _log = LogService.Instance;

        public static RequestApiResult<WeixinCreateQRCodeResult> Create(DomainContext domainContext, WeixinCreateQRCodeArgs args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinCreateQRCodeResult> result = QRCodeApi.Create(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = QRCodeApi.Create(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("QRCodeApi.Create 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("QRCodeApi.Create 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

    }
}
