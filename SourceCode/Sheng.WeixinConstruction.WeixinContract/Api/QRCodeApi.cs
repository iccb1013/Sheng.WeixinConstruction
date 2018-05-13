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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    /*
     * 生成带参数的二维码
     * http://mp.weixin.qq.com/wiki/18/8a8bbd4f0abfa3e58d7f68ce7252c0d6.html
     */
    public static class QRCodeApi
    {
        private static readonly HttpService _httpService = HttpService.Instance;

        /// <summary>
        /// 创建二维码ticket
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinCreateQRCodeResult> Create(string access_token, WeixinCreateQRCodeArgs args)
        {
            RequestApiResult<WeixinCreateQRCodeResult> result =
               new RequestApiResult<WeixinCreateQRCodeResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinCreateQRCodeResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }
    }
}
