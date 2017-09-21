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

namespace Sheng.WeixinConstruction.Container
{
    /// <summary>
    /// 作为第三方平台运营时，维护授权公众号的jsapi_ticket
    /// </summary>
    public class AuthorizerJsApiTicketPool
    {
        private static readonly AuthorizerJsApiTicketPool _instance = new AuthorizerJsApiTicketPool();
        public static AuthorizerJsApiTicketPool Instance
        {
            get { return _instance; }
        }

        private LogService _log = LogService.Instance;
        private ThirdPartyManager _thirdPartyManager = ThirdPartyManager.Instance;
        private AuthorizerAccessTokenPool _accessTokenPool = AuthorizerAccessTokenPool.Instance;

        //Key:appId
        //Value:AuthorizerJsApiTicketWrapper
        private Hashtable _wrapperTable = new Hashtable();
        private object _lockObj = new object();
        private Timer _timer;

        private AuthorizerJsApiTicketPool()
        {
            LoadAuthorizerJsApiTicket();

            _timer = new Timer(TimerCallback, null, 0, 1000 * 60);
        }

        /// <summary>
        /// 从数据库中恢复token数据
        /// </summary>
        private void LoadAuthorizerJsApiTicket()
        {
            foreach (AuthorizerJsApiTicketWrapper item in _thirdPartyManager.GetAuthorizerJsApiTicketList())
            {
                _wrapperTable.Add(item.AppId, item);
            }

            _log.Write("从数据库中恢复 AuthorizerJsApiTicket " + _wrapperTable.Count + " 个");
        }

        public void Add(string appId, string jsApiTicket, int expiresIn)
        {
            lock (_lockObj)
            {
                AuthorizerJsApiTicketWrapper wrapper = _wrapperTable[appId] as AuthorizerJsApiTicketWrapper;
                if (wrapper != null)
                {
                    _wrapperTable.Remove(appId);
                }

                wrapper = new AuthorizerJsApiTicketWrapper();
                wrapper.AppId = appId;
                wrapper.JsApiTicket = jsApiTicket;
                wrapper.JsApiTicketExpiryTime = DateTime.Now.AddSeconds(expiresIn);

                _wrapperTable.Add(appId, wrapper);
            }
        }

        public void Remove(string appId)
        {
            lock (_lockObj)
            {
                if (_wrapperTable.ContainsKey(appId))
                {
                    _wrapperTable.Remove(appId);
                }
            }
        }

        public string Get(string appId)
        {
            if (String.IsNullOrEmpty(appId))
            {
                return null;
            }

            AuthorizerJsApiTicketWrapper wrapper = _wrapperTable[appId] as AuthorizerJsApiTicketWrapper;

            if (wrapper == null || wrapper.WillbeTimeout)
            {
                lock (_lockObj)
                {
                    wrapper = _wrapperTable[appId] as AuthorizerJsApiTicketWrapper;

                    if (wrapper == null || wrapper.WillbeTimeout)
                    {
                        if (wrapper == null)
                        {
                            wrapper = CreateWrapper(appId);
                            _wrapperTable.Add(appId, wrapper);
                        }
                        else
                        {
                            UpdateWrapper(wrapper);
                        }                       
                    }
                }
            }

            if (wrapper != null)
                return wrapper.JsApiTicket;
            else
                return null;
        }

        public List<AuthorizerJsApiTicketWrapper> GetAuthorizerJsApiTicketList()
        {
            List<AuthorizerJsApiTicketWrapper> wrapperList =
                _wrapperTable.Values.Cast<AuthorizerJsApiTicketWrapper>().ToList();

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
        public string Refresh(string appId, string jsApiTicket)
        {
            //accessTokenString为空不给强刷，防止不传accessTokenString的滥用
            if (String.IsNullOrEmpty(appId) || String.IsNullOrEmpty(jsApiTicket))
            {
                _log.Write("调用了 AccessTokenPool.Refresh 但是参数不完整", appId + "," + jsApiTicket,
                    TraceEventType.Error);
                return null;
            }

            AuthorizerJsApiTicketWrapper wrapper;
            lock (_lockObj)
            {
                wrapper = _wrapperTable[appId] as AuthorizerJsApiTicketWrapper;

                if (wrapper == null)
                {
                    wrapper = CreateWrapper(appId);
                    if (wrapper != null)
                    {
                        _wrapperTable.Add(appId, wrapper);
                    }
                }
                else
                {
                    if (wrapper.JsApiTicket == jsApiTicket)
                    {
                        UpdateWrapper(wrapper);
                    }
                }
            }

            if (wrapper != null)
                return wrapper.JsApiTicket;
            else
                return null;
        }

        public void TimerCallback(object state)
        {
            List<AuthorizerJsApiTicketWrapper> wrapperList =
                _wrapperTable.Values.Cast<AuthorizerJsApiTicketWrapper>().ToList();

            foreach (AuthorizerJsApiTicketWrapper wrapper in wrapperList)
            {
                if (wrapper.WillbeTimeout)
                {
                    lock (_lockObj)
                    {
                        if (wrapper.WillbeTimeout)
                        {
                            UpdateWrapper(wrapper);
                        }
                    }
                }
            }
        }

        private AuthorizerJsApiTicketWrapper CreateWrapper(string appId)
        {
            RequestApiResult<WeixinGetJsApiTicketResult> result = TokenApiWrapper.GetJsApiTicket(appId);

            if (result.Success)
            {
                AuthorizerJsApiTicketWrapper wrapper = new AuthorizerJsApiTicketWrapper();
                wrapper.AppId = appId;
                wrapper.JsApiTicket = result.ApiResult.Ticket;
                wrapper.JsApiTicketExpiryTime = DateTime.Now.AddSeconds(result.ApiResult.ExpiresIn);
                return wrapper;
            }

            return null;
        }

        private void UpdateWrapper(AuthorizerJsApiTicketWrapper wrapper)
        {
            RequestApiResult<WeixinGetJsApiTicketResult> result = TokenApiWrapper.GetJsApiTicket(wrapper.AppId);

            if (result.Success)
            {
                wrapper.JsApiTicket = result.ApiResult.Ticket;
                wrapper.JsApiTicketExpiryTime = DateTime.Now.AddSeconds(result.ApiResult.ExpiresIn);
            }
        }

    }
}