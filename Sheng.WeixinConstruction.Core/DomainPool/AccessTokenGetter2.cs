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
    /// </summary>
    public static class AccessTokenGetter
    {
        private static readonly LogService _log = LogService.Instance;
        private static readonly HttpService _httpService = HttpService.Instance;

        private static string _getUrl;
        private static string _refreshUrl;

        static AccessTokenGetter()
        {
            string accessTokenService = ConfigurationManager.AppSettings["ContainerService"];

            _getUrl = accessTokenService + "AccessToken/Get?domainId={0}&appId={1}&appSecret={2}";
            _refreshUrl = accessTokenService + "AccessToken/Refresh?domainId={0}&appId={1}&appSecret={2}&accessToken={3}";
        }

        public static string Get(DomainEntity domain)
        {
            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_getUrl, domain.Id, domain.AppId, domain.AppSecret);

            HttpRequestResult result = _httpService.Request(args);
            if (result.Success == false)
            {
                _log.Write("AccessTokenGetter.Get 请求失败", result.Exception.Message, TraceEventType.Error);
                return String.Empty;
            }

            return result.Content;
        }

        /// <summary>
        /// 强制刷新
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string Refresh(DomainEntity domain, string accessToken)
        {
            _log.Write("调用 AccessTokenGetter.Refresh 强制刷新", accessToken, TraceEventType.Warning);

            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_refreshUrl, domain.Id, domain.AppId, domain.AppSecret, accessToken);

            HttpRequestResult result = _httpService.Request(args);
            if (result.Success == false)
            {
                _log.Write("AccessTokenGetter.Refresh 请求失败", result.Exception.Message, TraceEventType.Error);
                return String.Empty;
            }

            _log.Write("调用 AccessTokenGetter.Refresh 刷新成功", result.Content, TraceEventType.Warning);

            return result.Content;
        }
    }
}
