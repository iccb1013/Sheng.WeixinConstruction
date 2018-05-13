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
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    /// <summary>
    /// 统一微信端和管理端对同一个domain的accessToken的访问
    /// 获取授权公众号的token和jsapiticket
    /// </summary>
    public static class AccessTokenGetter
    {
        private static readonly LogService _log = LogService.Instance;
        private static readonly HttpService _httpService = HttpService.Instance;

        private static string _getUrl;
        private static string _refreshUrl;

        private static string _getJsApiTicketUrl;
        private static string _refreshJsApiTicketUrl;

        static AccessTokenGetter()
        {
            string accessTokenService = ConfigurationManager.AppSettings["ContainerService"];

            _getUrl = accessTokenService + "ThirdPartyAuth/GetAuthorizerAccessToken?appId={0}";
            _refreshUrl = accessTokenService + "ThirdPartyAuth/RefreshAuthorizerAccessToken?appId={0}&accessToken={1}";

            _getJsApiTicketUrl = accessTokenService + "ThirdPartyAuth/GetAuthorizerJsApiTicket?appId={0}";
            _refreshJsApiTicketUrl = accessTokenService + "ThirdPartyAuth/RefreshAuthorizerJsApiTicket?appId={0}&jsApiTicket={1}";

        }

        public static string Get(string authorizerAppId)
        {
            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_getUrl, authorizerAppId);

            HttpRequestResult result = _httpService.Request(args);
            if (result.Success == false)
            {
                _log.Write("AccessTokenGetter.Get 请求失败", result.Exception.Message, TraceEventType.Error);
                return null;
            }

            return result.Content;
        }

        /// <summary>
        /// 强制刷新
        /// </summary>
        /// <param name="authorizerAppId"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string Refresh(string authorizerAppId, string accessToken)
        {
            _log.Write("调用 AccessTokenGetter.Refresh 强制刷新", accessToken, TraceEventType.Warning);

            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_refreshUrl,authorizerAppId, accessToken);

            HttpRequestResult result = _httpService.Request(args);
            if (result.Success == false)
            {
                _log.Write("AccessTokenGetter.Refresh 请求失败", result.Exception.Message, TraceEventType.Error);
                return null;
            }

            _log.Write("调用 AccessTokenGetter.Refresh 刷新成功", result.Content, TraceEventType.Warning);

            return result.Content;
        }

        public static string GetJsApiTicket(string authorizerAppId)
        {
            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_getJsApiTicketUrl, authorizerAppId);

            HttpRequestResult result = _httpService.Request(args);
            if (result.Success == false)
            {
                _log.Write("AccessTokenGetter.GetJsApiTicket 请求失败", result.Exception.Message, TraceEventType.Error);
                return null;
            }

            return result.Content;
        }

        public static string RefreshJsApiTicket(string authorizerAppId, string jsApiTicket)
        {
            _log.Write("调用 AccessTokenGetter.Refresh 强制刷新", jsApiTicket, TraceEventType.Warning);

            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_refreshJsApiTicketUrl, authorizerAppId, jsApiTicket);

            HttpRequestResult result = _httpService.Request(args);
            if (result.Success == false)
            {
                _log.Write("AccessTokenGetter.RefreshJsApiTicket 请求失败", result.Exception.Message, TraceEventType.Error);
                return null;
            }

            _log.Write("调用 AccessTokenGetter.RefreshJsApiTicket 刷新成功", result.Content, TraceEventType.Warning);

            return result.Content;
        }

    }
}
