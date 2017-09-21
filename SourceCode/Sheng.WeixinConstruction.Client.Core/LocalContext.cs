using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using Sheng.WeixinConstruction.Infrastructure;
using Linkup.Common;

namespace Sheng.WeixinConstruction.Client.Core
{
    public class LocalContext
    {
        private static LocalContext _instance;
        public static LocalContext Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LocalContext();


                return _instance;
            }
        }

        private HttpService _httpService = HttpService.Instance;

        private string _refreshAccessTokenUri;

        private System.Threading.Timer _accessTokenTimer;

        public string AppId
        {
            get;
            private set;
        }

        public string AppSecret
        {
            get;
            private set;
        }

        public string Token
        {
            get;
            private set;
        }

        public string AccessToken
        {
            get;
            private set;
        }

        private LocalContext()
        {
            AppId = "wx8c36b3c0000a0a49";
            AppSecret = "c072453550caa298e5df7c72e7e77d16";
            Token = "QDG6eK";

            _refreshAccessTokenUri = String.Format(
                "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}",
                AppId, AppSecret);
            //1 * 60 * 1000
            _accessTokenTimer = new System.Threading.Timer(AccessTokenTimerCallback, null, 0, 1 * 60 * 1000);

        }

        private void AccessTokenTimerCallback(object state)
        {
            RefreshAccessToken();
        }

        public void RefreshAccessToken()
        {
            if (String.IsNullOrEmpty(_refreshAccessTokenUri))
                return;

            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = _refreshAccessTokenUri;

            HttpRequestResult result = _httpService.Request(args);
            if (result.Success == false)
            {
                //TODO:日志
                return;
            }

            JObject jObject = JObject.Parse(result.Content);
            JToken jToken = jObject["errcode"];
            if (jToken != null)
            {
                //TODO:失败
            }
            else
            {
                AccessToken = jObject["access_token"].ToString();
                int expiresIn = jObject["expires_in"].Value<int>();

                _accessTokenTimer.Change(0, expiresIn - (1 * 60 * 1000));
            }
        }
    }
}