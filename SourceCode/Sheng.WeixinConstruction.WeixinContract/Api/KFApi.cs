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
     * http://mp.weixin.qq.com/wiki/14/d9be34fe03412c92517da10a5980e7ee.html#.E6.8E.A5.E5.8F.A3.E7.9A.84.E7.BB.9F.E4.B8.80.E5.8F.82.E6.95.B0.E8.AF.B4.E6.98.8E
     */
    public static class KFApi
    {
        private static readonly HttpService _httpService = HttpService.Instance;

        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult Send(string access_token, KFMessageBase message)
        {
            RequestApiResult result = new RequestApiResult();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(message);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiError = WeixinApiHelper.Parse(result.HttpRequestResult.Content);
            }

            return result;
        }

    }
}
