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
     * 由于群发任务提交后，群发任务可能在一定时间后才完成，因此，群发接口调用时，
     * 仅会给出群发任务是否提交成功的提示，若群发任务提交成功，则在群发任务结束时，
     * 会向开发者在公众平台填写的开发者URL（callback URL）推送事件。
     */

    /// <summary>
    /// 高级群发接口
    /// http://mp.weixin.qq.com/wiki/15/40b6865b893947b764e2de8e4a1fb55f.html#.E9.A2.84.E8.A7.88.E6.8E.A5.E5.8F.A3.E3.80.90.E8.AE.A2.E9.98.85.E5.8F.B7.E4.B8.8E.E6.9C.8D.E5.8A.A1.E5.8F.B7.E8.AE.A4.E8.AF.81.E5.90.8E.E5.9D.87.E5.8F.AF.E7.94.A8.E3.80.91
    /// </summary>
    public class GroupMessageApi
    {
        private static readonly HttpService _httpService = HttpService.Instance;

        /// <summary>
        /// 根据分组进行群发【订阅号与服务号认证后均可用】
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinGroupMessageSendAllResult> SendAll(string access_token, WeixinGroupMessageSendAllArgs args)
        {
            /*
             * 此处返回的JSON又傻逼一样的和其它接口不同
             * {
                   "errcode":0,
                   "errmsg":"send job submission success",
                   "msg_id":34182, 
                   "msg_data_id": 206227730
                }
             * 
             */

            RequestApiResult<WeixinGroupMessageSendAllResult> result =
               new RequestApiResult<WeixinGroupMessageSendAllResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/message/mass/sendall?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinGroupMessageSendAllResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 根据OpenID列表群发【订阅号不可用，服务号认证后可用】
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinGroupMessageSendResult> Send(string access_token, WeixinGroupMessageSendArgs args)
        {
            RequestApiResult<WeixinGroupMessageSendResult> result =
               new RequestApiResult<WeixinGroupMessageSendResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/message/mass/send?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinGroupMessageSendResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 查询群发消息发送状态【订阅号与服务号认证后均可用】
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinGroupMessageGetStatusResult> GetStatus(string access_token, string msgId)
        {
            RequestApiResult<WeixinGroupMessageGetStatusResult> result =
               new RequestApiResult<WeixinGroupMessageGetStatusResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/message/mass/send?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(new { msg_id = msgId });

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinGroupMessageGetStatusResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 预览接口【订阅号与服务号认证后均可用】
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinGroupMessagePreviewResult> Preview(string access_token, WeixinGroupMessagePreviewArgs args)
        {
            RequestApiResult<WeixinGroupMessagePreviewResult> result =
               new RequestApiResult<WeixinGroupMessagePreviewResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/message/mass/preview?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinGroupMessagePreviewResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }
    }
}
