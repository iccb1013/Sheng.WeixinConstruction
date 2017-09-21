using Linkup.Common;
using Linkup.Data;
using Linkup.DataRelationalMapping;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using Sheng.WeixinConstruction.WeixinContract.ThirdParty;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Collections.Concurrent;

namespace Sheng.WeixinConstruction.Container
{
    /// <summary>
    /// 作为第三方平台运营时，维护授权公众号的AccessToken
    /// </summary>
    public class AuthorizerAccessTokenPool
    {
        private static readonly AuthorizerAccessTokenPool _instance = new AuthorizerAccessTokenPool();
        public static AuthorizerAccessTokenPool Instance
        {
            get { return _instance; }
        }

        private LogService _log = LogService.Instance;
        private ThirdPartyManager _thirdPartyManager = ThirdPartyManager.Instance;

        //Key:appId
        //Value:AuthorizerAccessTokenWrapper
        //private Hashtable _accessTokenWrapperTable = new Hashtable();
        //private object _lockObj = new object();
        private Timer _timer;

        private ConcurrentDictionary<string, AuthorizerAccessTokenWrapper> _accessTokenWrapperList
            = new ConcurrentDictionary<string, AuthorizerAccessTokenWrapper>();
        private object _getWrapperLockObj = new object();
        private object _timerCallbackLockObj = new object();

        private AuthorizerAccessTokenPool()
        {
            LoadAuthorizerAccessToken();

            _timer = new Timer(TimerCallback, null, 0, 1000 * 60);
        }

        /// <summary>
        /// 从数据库中恢复token数据
        /// </summary>
        private void LoadAuthorizerAccessToken()
        {
            foreach (AuthorizerAccessTokenWrapper item in _thirdPartyManager.GetAuthorizerAccessTokenList())
            {
                _accessTokenWrapperList.TryAdd(item.AppId, item);
            }

            _log.Write("从数据库中恢复 AuthorizerAccessTokenWrapper " + _accessTokenWrapperList.Count + " 个");
        }

        public void Add(string appId, string accessToken, int expiresIn, string refreshToken)
        {
            lock (_getWrapperLockObj)
            {
                AuthorizerAccessTokenWrapper wrapper = new AuthorizerAccessTokenWrapper();
                wrapper.AppId = appId;
                wrapper.AccessToken = accessToken;
                wrapper.AccessTokenExpiryTime = DateTime.Now.AddSeconds(expiresIn);
                wrapper.RefreshToken = refreshToken;

                _accessTokenWrapperList.AddOrUpdate(appId, wrapper,
                    (itemAppId, itemWrapper) => { return wrapper; });
            }
        }

        public void Remove(string appId)
        {
            AuthorizerAccessTokenWrapper wrapper;
            _accessTokenWrapperList.TryRemove(appId, out wrapper);
        }

        public string Get(string appId)
        {
            if (String.IsNullOrEmpty(appId))
            {
                return null;
            }

            AuthorizerAccessTokenWrapper wrapper = _accessTokenWrapperList[appId];

            if (wrapper == null)
            {
                lock (_getWrapperLockObj)
                {
                    wrapper = _accessTokenWrapperList[appId];

                    if (wrapper == null)
                    {
                        wrapper = CreateWrapper(appId);
                        _accessTokenWrapperList.TryAdd(appId, wrapper);
                    }
                }
            }

            if (wrapper != null)
                return wrapper.AccessToken;
            else
                return null;
        }

        public List<AuthorizerAccessTokenWrapper> GetAuthorizerAccessTokenList()
        {
            List<AuthorizerAccessTokenWrapper> wrapperList =
                _accessTokenWrapperList.Values.Cast<AuthorizerAccessTokenWrapper>().ToList();

            return wrapperList;
        }

        /// <summary>
        /// 强制刷新，微信的AccessToken似乎存在没到有效期就失效的现象
        /// accessTokenString 用于实现锁机制，防止并发的重复刷新
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public string Refresh(string appId, string accessToken)
        {
            //accessTokenString为空不给强刷，防止不传accessTokenString的滥用
            if (String.IsNullOrEmpty(appId) || String.IsNullOrEmpty(accessToken))
            {
                _log.Write("调用了 AuthorizerAccessTokenPool.Refresh 但是参数不完整", appId + "," + accessToken,
                    TraceEventType.Error);
                return null;
            }

            AuthorizerAccessTokenWrapper wrapper;
            lock (_accessTokenWrapperList)
            {
                wrapper = _accessTokenWrapperList[appId];

                if (wrapper == null)
                {
                    wrapper = CreateWrapper(appId);
                    if (wrapper != null)
                    {
                        _accessTokenWrapperList.TryAdd(appId, wrapper);
                    }
                }
                else
                {
                    if (wrapper.AccessToken == accessToken)
                    {
                        UpdateWrapper(wrapper);
                    }
                }
            }

            if (wrapper != null)
                return wrapper.AccessToken;
            else
                return null;
        }

        public void TimerCallback(object state)
        {
            List<AuthorizerAccessTokenWrapper> wrapperList =
                _accessTokenWrapperList.Values.Cast<AuthorizerAccessTokenWrapper>().ToList();

            foreach (AuthorizerAccessTokenWrapper wrapper in wrapperList)
            {
                if (wrapper.WillbeTimeout)
                {
                    lock (_timerCallbackLockObj)
                    {
                        if (wrapper.WillbeTimeout)
                        {
                            UpdateWrapper(wrapper);
                        }
                    }
                }
            }
        }

        private AuthorizerAccessTokenWrapper CreateWrapper(string appId)
        {
            string refreshToken = _thirdPartyManager.GetAuthorizerRefreshToken(appId);
            if (String.IsNullOrEmpty(refreshToken))
            {
                _log.Write(appId + " 的 RefreshToken 不存在", TraceEventType.Warning);
                return null;
            }

            RequestApiResult<WeixinThirdPartyGetAuthorizerAccessTokenResult> result =
                ThirdPartyApiWrapper.GetAuthorizerAccessToken(appId, refreshToken);

            if (result.Success)
            {
                AuthorizerAccessTokenWrapper wrapper = new AuthorizerAccessTokenWrapper();
                wrapper.AppId = appId;
                wrapper.RefreshToken = result.ApiResult.RefreshToken;
                wrapper.AccessToken = result.ApiResult.AccessToken;
                wrapper.AccessTokenExpiryTime = DateTime.Now.AddSeconds(result.ApiResult.ExpiresIn);
                return wrapper;
            }

            return null;
        }

        private void UpdateWrapper(AuthorizerAccessTokenWrapper wrapper)
        {
            RequestApiResult<WeixinThirdPartyGetAuthorizerAccessTokenResult> result =
                ThirdPartyApiWrapper.GetAuthorizerAccessToken(wrapper.AppId, wrapper.RefreshToken);

            if (result.Success)
            {
                wrapper.RefreshToken = result.ApiResult.RefreshToken;
                wrapper.AccessToken = result.ApiResult.AccessToken;
                wrapper.AccessTokenExpiryTime = DateTime.Now.AddSeconds(result.ApiResult.ExpiresIn);
            }
        }

    }
}