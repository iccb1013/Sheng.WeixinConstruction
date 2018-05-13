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
using Sheng.WeixinConstruction.ApiContract;
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
    /// 作为第三方平台运营时，获取平台的AccessToken
    /// </summary>
    public static class ThirdPartyAccessTokenGetter
    {
        private static readonly LogService _log = LogService.Instance;
        private static readonly HttpService _httpService = HttpService.Instance;

        private static string _getUrl;
        private static string _refreshUrl;

        private static string _createAuthorizerUrl;
        private static string _getAuthorizerUrl;
        private static string _refreshAuthorizerUrl;

        static ThirdPartyAccessTokenGetter()
        {
            string accessTokenService = ConfigurationManager.AppSettings["ContainerService"];

            _getUrl = accessTokenService + "ThirdPartyAuth/GetAccessToken";
            _refreshUrl = accessTokenService + "ThirdPartyAuth/RefreshAccessToken?accessToken={0}";

            _createAuthorizerUrl = accessTokenService + "ThirdPartyAuth/CreateAuthorizer?domainId={0}&authCode={1}";
            _getAuthorizerUrl = accessTokenService + "ThirdPartyAuth/GetAuthorizerAccessToken?appId={0}";
            _refreshAuthorizerUrl = accessTokenService + "ThirdPartyAuth/RefreshAuthorizerAccessToken?appId={0}&accessToken={1}";

        }

        public static string Get()
        {
            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = _getUrl;

            HttpRequestResult result = _httpService.Request(args);
            if (result.Success == false)
            {
                _log.Write("ThirdPartyAccessTokenGetter.Get 请求失败", result.Exception.Message, TraceEventType.Error);
                return String.Empty;
            }

            return result.Content;
        }

        public static string Refresh(string accessToken)
        {
            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_refreshUrl, accessToken);

            HttpRequestResult result = _httpService.Request(args);
            if (result.Success == false)
            {
                _log.Write("ThirdPartyAccessTokenGetter.Refresh 请求失败", result.Exception.Message, TraceEventType.Error);
                return String.Empty;
            }

            return result.Content;
        }

        public static ApiResult<CreateAuthorizerResult> CreateAuthorizer(Guid domainId, string authCode)
        {
            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_createAuthorizerUrl, domainId, authCode);

            HttpRequestResult result = _httpService.Request(args);

            ApiResult<CreateAuthorizerResult> apiResult;

            if (result.Success == false)
            {
                _log.Write("ThirdPartyAccessTokenGetter.CreateAuthorizer 请求失败", 
                    result.Exception.Message, TraceEventType.Error);

                apiResult = new ApiResult<CreateAuthorizerResult>();
                apiResult.Message = result.Exception.Message;
                return apiResult;
            }

            apiResult = 
                JsonConvert.DeserializeObject<ApiResult<CreateAuthorizerResult>>(result.Content);

            return apiResult;
        }

        public static string GetAuthorizerAccessToken(string appId)
        {
            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_getAuthorizerUrl, appId);

            HttpRequestResult result = _httpService.Request(args);
            if (result.Success == false)
            {
                _log.Write("ThirdPartyAccessTokenGetter.GetAuthorizerAccessToken 请求失败",
                    result.Exception.Message, TraceEventType.Error);
                return null;
            }

            return result.Content;
        }

        public static string RefreshAuthorizerAccessToken(string appId, string accessToken)
        {
            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_refreshAuthorizerUrl, appId, accessToken);

            HttpRequestResult result = _httpService.Request(args);
            if (result.Success == false)
            {
                _log.Write("ThirdPartyAccessTokenGetter.RefreshAuthorizerAccessToken 请求失败", 
                    result.Exception.Message, TraceEventType.Error);
                return String.Empty;
            }

            return result.Content;
        }

    }
}
