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
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Service
{
    public class ControlledCachingService
    {
        private static readonly ControlledCachingService _instance = new ControlledCachingService();
        public static ControlledCachingService Instance
        {
            get { return _instance; }
        }

        private LogService _log = LogService.Instance;
        private HttpService _httpService = HttpService.Instance;

        private string _controlledCacheServiceUrl;

        private ControlledCachingService()
        {
            _controlledCacheServiceUrl =
                ConfigurationManager.AppSettings["ContainerService"] + "ControlledCache/";
        }

        public bool Contains(string key)
        {
            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_controlledCacheServiceUrl + "Contains?key={0}", key);

            HttpRequestResult requestResult = _httpService.Request(args);
            if (requestResult.Success == false)
            {
                _log.Write("ControlledCachingService.Contains 请求失败", requestResult.Exception.Message, TraceEventType.Error);
                return false;
            }

            bool result = false;
            if (bool.TryParse(requestResult.Content, out result))
            {
                return result;
            }
            else
            {
                return false;
            }
        }

        public void Set(string key, string data, int seconds)
        {
            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_controlledCacheServiceUrl + "Set?key={0}&data={1}&seconds={2}", key,data,seconds);

            HttpRequestResult requestResult = _httpService.Request(args);
            if (requestResult.Success == false)
            {
                _log.Write("ControlledCachingService.Set 请求失败", requestResult.Exception.Message, TraceEventType.Error);
            }

            return;
        }

        public string Get(string key)
        {
            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_controlledCacheServiceUrl + "Get?key={0}", key);

            HttpRequestResult requestResult = _httpService.Request(args);
            if (requestResult.Success == false)
            {
                _log.Write("ControlledCachingService.Get 请求失败", requestResult.Exception.Message, TraceEventType.Error);
                return String.Empty;
            }

            return requestResult.Content;
        }

        public void Remove(string key)
        {
            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_controlledCacheServiceUrl + "Remove?key={0}", key);

            HttpRequestResult requestResult = _httpService.Request(args);
            if (requestResult.Success == false)
            {
                _log.Write("ControlledCachingService.Remove 请求失败", requestResult.Exception.Message, TraceEventType.Error);
            }

            return;
        }

        public void ExtendExpiryTime(string key, int seconds)
        {
            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(_controlledCacheServiceUrl + "ExtendExpiryTime?key={0}&seconds={1}", key, seconds);

            HttpRequestResult requestResult = _httpService.Request(args);
            if (requestResult.Success == false)
            {
                _log.Write("ControlledCachingService.ExtendExpiryTime 请求失败", requestResult.Exception.Message, TraceEventType.Error);
            }

            return;
        }

    }
}
