using Sheng.WeixinConstruction.Container.Models;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Container
{
    /// <summary>
    /// 不根据访问频率确定生存周期
    /// 只根据明确的生存周期存储，获取不增加生成周期，无命中率概念
    /// 用于文件服务器判断用户是否已登录
    /// </summary>
    public class ControlledCachePool
    {
        private static readonly ControlledCachePool _instance = new ControlledCachePool();
        public static ControlledCachePool Instance
        {
            get { return _instance; }
        }

        //Key:domainId
        //Value:object
        private Hashtable _cacheTable = new Hashtable();

        private object _lockObj = new object();

        /// <summary>
        /// 指定key的缓存是否存在
        /// </summary>
        /// <param name="key"></param>
        public bool Contains(string key)
        {
            if (String.IsNullOrEmpty(key))
                return false;

            return _cacheTable.ContainsKey(key);
        }

        /// <summary>
        /// 设置缓存项目
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, string data, int seconds)
        {
            if (String.IsNullOrEmpty(key))
                return;

            lock (_lockObj)
            {
                if (_cacheTable.ContainsKey(key))
                {
                    ControlledCachedItem item = (ControlledCachedItem)_cacheTable[key];
                    item.Data = data;
                    item.ExpiryTime = DateTime.Now.AddSeconds(seconds);
                }
                else
                {
                    ControlledCachedItem item = new ControlledCachedItem();
                    item.Data = data;
                    item.ExpiryTime = DateTime.Now.AddSeconds(seconds);
                    _cacheTable.Add(key, item);
                }
            }
        }

        public string Get(string key)
        {
            if (String.IsNullOrEmpty(key))
                return null;

            ControlledCachedItem item = _cacheTable[key] as ControlledCachedItem;
            if (item != null)
                return item.Data;
            else
                return null;
        }

        public void Remove(string key)
        {
            if (String.IsNullOrEmpty(key))
                return;

            lock (_lockObj)
            {
                if (_cacheTable.ContainsKey(key))
                {
                    _cacheTable.Remove(key);
                }
            }
        }

        public void ExtendExpiryTime(string key, int seconds)
        {
            if (String.IsNullOrEmpty(key))
                return;

            ControlledCachedItem item = _cacheTable[key] as ControlledCachedItem;
            if (item != null)
            {
                item.ExpiryTime = DateTime.Now.AddSeconds(seconds);
            }
        }

        public List<DictionaryEntry> GetList()
        {
            return _cacheTable.Cast<DictionaryEntry>().ToList();
        }
    }
}