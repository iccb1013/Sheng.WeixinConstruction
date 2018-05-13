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


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using Newtonsoft.Json;
using Linkup.Common;

namespace Sheng.WeixinConstruction.Service
{
    /*
     *  StackExchange.Redis Client
     * http://www.tuicool.com/articles/rqaYfuJ
     * 
     * 停止 redis 后，redis.IsConnected 始终为true
     */
    /// <summary>
    /// Key 本身是区分大小写的，此处 Key 自动强制小写
    /// </summary>
    public class CachingService
    {
        private static CachingService _instance;
        public static CachingService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CachingService();

                return _instance;
            }
        }

        private LogService _log = LogService.Instance;

        private ConnectionMultiplexer _redis;
        private IDatabase _redisDb;
        private bool _redisReady = false;

        private CachingService()
        {
            try
            {
                _redis = ConnectionMultiplexer.Connect(System.Configuration.ConfigurationManager.AppSettings["Redis"]);
                _redis.ConnectionFailed += _redis_ConnectionFailed;
                _redis.ConnectionRestored += _redis_ConnectionRestored;

                //如果连接中断，会报错 SocketFailure on GET
                //UnableToResolvePhysicalConnection 
                _redisDb = _redis.GetDatabase();
                _redisReady = true;

                _log.Write("Redis 连接成功", TraceEventType.Information);
            }
            catch (Exception ex)
            {
                _log.Write("Redis 连接失败", ex.Message, TraceEventType.Error);
            }
        }

        void _redis_ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            _redisReady = true;
            _log.Write("Redis 连接恢复", TraceEventType.Error);
        }

        void _redis_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _redisReady = false;
            string message = String.Empty;
            if (e.Exception != null)
                message = e.Exception.Message;
            _log.Write("Redis 连接中断", message, TraceEventType.Error);
        }

        /// <summary>
        /// set 命令用于向缓存添加新的键值对。如果键已经存在，则之前的值将被替换。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set(string key, string value)
        {
            if (value == null)
                return false;

            if (_redisReady == false)
                return false;

            try
            {
                return _redisDb.StringSet(key.ToLower(), value);
            }
            catch (Exception ex)
            {
                _log.Write("Redis 访问出错", ex.Message, TraceEventType.Error);
                return false;
            }
        }

        /// <summary>
        /// 在缓存中保存键值对的时间长度（以秒为单位，0 表示永远）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        public bool Set(string key, string value, TimeSpan expiresIn)
        {
            if (value == null)
                return false;

            if (_redisReady == false)
                return false;

            try
            {
                return _redisDb.StringSet(key.ToLower(), value, expiresIn);
            }
            catch (Exception ex)
            {
                _log.Write("Redis 访问出错", ex.Message, TraceEventType.Error);
                return false;
            }
        }

        /// <summary>
        /// set 命令用于向缓存添加新的键值对。如果键已经存在，则之前的值将被替换。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value)
        {
            if (value == null)
                return false;

            if (_redisReady == false)
                return false;

            try
            {
                return _redisDb.StringSet(key.ToLower(), JsonConvert.SerializeObject(value));
            }
            catch (Exception ex)
            {
                _log.Write("Redis 访问出错", ex.Message, TraceEventType.Error);
                return false;
            }
        }

        /// <summary>
        /// 在缓存中保存键值对的时间长度（以秒为单位，0 表示永远）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            if (value == null)
                return false;

            if (_redisReady == false)
                return false;

            try
            {
                return _redisDb.StringSet(key.ToLower(), JsonConvert.SerializeObject(value), expiresIn);
            }
            catch (Exception ex)
            {
                _log.Write("Redis 访问出错", ex.Message, TraceEventType.Error);
                return false;
            }
        }

        public string Get(string key)
        {
            if (_redisReady == false)
                return null;

            try
            {
                return _redisDb.StringGet(key.ToLower());
            }
            catch (Exception ex)
            {
                _log.Write("Redis 访问出错", ex.Message, TraceEventType.Error);
                return null;
            }
        }

        /// <summary>
        /// 对于值类型，如果没有指定键的缓存数据
        /// 会返回值类型的默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (_redisReady == false)
                return default(T);

            string value = null;

            try
            {
                value = _redisDb.StringGet(key.ToLower());
            }
            catch (Exception ex)
            {
                _log.Write("Redis 访问出错", ex.Message, TraceEventType.Error);
                return default(T);
            }

            if (String.IsNullOrEmpty(value))
                return default(T);

            T t = JsonConvert.DeserializeObject<T>(value);
            return t;
        }

        /// <summary>
        /// delete 命令用于删除 memcached 中的任何现有值。
        /// 您将使用一个键调用 delete，如果该键存在于缓存中，则删除该值。如果不存在，则返回一条 NOT_FOUND 消息。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if (_redisReady == false)
                return false;

            try
            {
                return _redisDb.KeyDelete(key.ToLower());
            }
            catch (Exception ex)
            {
                _log.Write("Redis 访问出错", ex.Message, TraceEventType.Error);
                return false;
            }
        }

        public bool ContainsKey(string key)
        {
            if (_redisReady == false)
                return false;

            try
            {
            return _redisDb.KeyExists(key.ToLower());
            }
            catch (Exception ex)
            {
                _log.Write("Redis 访问出错", ex.Message, TraceEventType.Error);
                return false;
            }
        }
    }
}
