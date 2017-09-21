using Linkup.Common;
using Linkup.Data;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using Sheng.WeixinConstruction.WeixinContract.ThirdParty;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;

namespace Sheng.WeixinConstruction.Container
{
    /// <summary>
    /// 作为第三方平台运行时，维护第三方平台自己的 accessToken
    /// </summary>
    public class ThirdPartyAccessToken
    {
        private static readonly ThirdPartyAccessToken _instance = new ThirdPartyAccessToken();
        public static ThirdPartyAccessToken Instance
        {
            get { return _instance; }
        }

        private LogService _log = LogService.Instance;
        private WxConfigurationSection _configuration = ConfigService.Instance.Configuration;
        private ThirdPartyAuthHandler _thirdPartyAuthHandler = ThirdPartyAuthHandler.Instance;
        private ThirdPartyManager _thirdPartyManager = ThirdPartyManager.Instance;

        private object _lockObj = new object();
        private Timer _timer;
        private WeixinThirdPartyGetAccessTokenResult _token;

        private ThirdPartyAccessToken()
        {
            //尝试从数据库中恢复accessToken
             _token = _thirdPartyManager.GetAccessToken();
             if (_token == null || String.IsNullOrEmpty(_token.AccessToken) || _token.WillbeTimeout)
             {
                 //初始化AccessToken
                 RequestAccessToken();
             }

            _timer = new Timer(TimerCallback, null, 0, 1000 * 60);
        }

        public string Get()
        {
            if (_token == null || _token.WillbeTimeout)
            {
                Refresh();
            }

            if (_token != null)
                return _token.AccessToken;
            else
                return null;
        }

        public DateTime? GetExpiryTime()
        {
            if (_token != null)
                return _token.AccessTokenExpiryTime;
            else
                return null;
        }

        /// <summary>
        /// 强制刷新
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public string Refresh(string accessToken)
        {
            if (_token == null)
            {
                return Get();
            }
            else
            {

                if (_token.AccessToken == accessToken)
                {
                    lock (_lockObj)
                    {
                        if (_token.AccessToken == accessToken)
                            RequestAccessToken();
                    }
                }

                if (_token != null)
                    return _token.AccessToken;
                else
                    return null;
            }
        }

        public void TimerCallback(object state)
        {
            Refresh();
        }

        /// <summary>
        /// 注意，这个方法的刷新不是无条件的，除非
        /// _token == null || _token.WillbeTimeout 才会刷新
        /// </summary>
        private void Refresh()
        {
            if (_token == null || _token.WillbeTimeout)
            {
                lock (_lockObj)
                {
                    if (_token == null || _token.WillbeTimeout)
                    {
                        RequestAccessToken();
                    }
                }
            }
        }

        /// <summary>
        /// 向服务器请求一个新的 accessToken
        /// </summary>
        private void RequestAccessToken()
        {
            WeixinThirdPartyGetAccessTokenArgs args = new WeixinThirdPartyGetAccessTokenArgs();
            args.ComponentAppId = _configuration.ThirdParty.AppId;
            args.ComponentAppSecret = _configuration.ThirdParty.AppSecret;
            args.VerifyTicket = _thirdPartyAuthHandler.GetComponentVerifyTicket();
            RequestApiResult<WeixinThirdPartyGetAccessTokenResult> result =
                ThirdPartyApi.GetAccessToken(args);
            if (result.Success)
            {
                _token = result.ApiResult;

                //写数据库，用于启动时恢复
                _thirdPartyManager.UpdateAccessToken(
                    _token.AccessToken, DateTime.Now.AddSeconds(_token.ExpiresIn));
            }
            else
            {
                _log.Write("ThirdPartyAccessToken.GetToken 失败", result.GetDetail(), TraceEventType.Error);
            }
        }
    }
}