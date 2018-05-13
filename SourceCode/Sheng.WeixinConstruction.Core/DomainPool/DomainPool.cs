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
using Linkup.Data;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    /*
     * 对于服务端来说，要以domainId为Key，但是对于客户端，又要以authorizerAppId为主
     * 此处还是统一以domainId为键，对于客户端收到的请求，先自行定位到对应的 DomainId 再处理
     */
    public abstract class DomainPool<T> where T : DomainContext
    {
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        protected static readonly SettingsManager _settingsManager = SettingsManager.Instance;
        protected static readonly MemberManager _memberManager = MemberManager.Instance;
        protected static readonly LogService _logService = LogService.Instance;
        protected static readonly CachingService _caching = CachingService.Instance;
        private static readonly DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        //TODO:当公众号解除授权时，要清理掉缓存
        //因为有可能用户将同一个公众号授权给另一个Domain
        //Key:DomainId Value:T（DomainContext）
        private Hashtable _domainContextCached = new Hashtable();
        private object _domainContextCachedLockObj = new object();

        //Key:appId Value:DomainId
        private Hashtable _appIdDomainIdMapping = new Hashtable();
        private object _appIdDomainIdMappingLockObj = new object();

        //刷新Domain信息
        private Timer _refreshDomainTimer;

        public DomainPool()
        {
            //不从载入数据库中的所有Domain，因为可能存在许多僵尸号，无需为其加载资源

            //1 * 60 * 1000
            _refreshDomainTimer = new System.Threading.Timer(RefreshDomainTimerCallback, null, 0, 1 * 60 * 1000);

        }

        private void RefreshDomainTimerCallback(object state)
        {
            List<T> domainContextList = _domainContextCached.Values.Cast<T>().ToList();
            foreach (T domainContext in domainContextList)
            {
                //domainContext.Domain.LastUpdateTime 比 lastUpdateTime 的精度更高
                //从数据库取出放到 dataTable 中的数据，用 property.PropertyInfo.SetValue(resultObj, value, null); 的方式
                //设置到对象上，精度就和数据库 datetime 一样，带毫秒，但是 value.ToString() 或者在调试工具中看它的值
                //都是看不到毫秒的，用 DateTime.Parse(value) 会把毫秒丢失，造成这里的时间比较永远不一致
                //http://blog.csdn.net/mliu/article/details/1541322
                DateTime lastUpdateTime = _domainManager.GetLastUpdateTime(domainContext.Domain.Id);
                if (lastUpdateTime != domainContext.Domain.LastUpdateTime)
                {
                    domainContext.Refresh();
                    OnRefresh(domainContext);
                }

                DateTime? lastDockingTime = _domainManager.GetLastDockingTime(domainContext.Domain.Id);
                if (lastDockingTime.HasValue && lastDockingTime.Value != domainContext.Domain.LastDockingTime)
                {
                    //此处也要Refresh一下，除了更新lastDockingTime
                    //在被动解除绑定要，要更新授权公众号的信息
                    domainContext.Refresh();

                    if (domainContext.Online)
                    {
                        OnDocking(domainContext);
                    }
                    else
                    {
                        UnDocking(domainContext);
                    }
                }
            }
        }

        public DomainEntity Get(Guid domainId)
        {
            return GetDomainContext(domainId).Domain;
        }

        public T GetDomainContext(Guid domainId)
        {
            if (_domainContextCached.Contains(domainId))
            {
                return ((T)_domainContextCached[domainId]);
            }
            else
            {
                lock (_domainContextCachedLockObj)
                {
                    if (_domainContextCached.Contains(domainId))
                    {
                        return ((T)_domainContextCached[domainId]);
                    }

                    DomainEntity domain = _domainManager.GetDomain(domainId);
                    if (domain == null)
                        return null;

                    T domainContext = Create(domain);
                    _domainContextCached.Add(domainId, domainContext);

                    return domainContext;
                }
            }
        }

        /// <summary>
        /// 刷新指定Domain信息的缓存
        /// </summary>
        /// <param name="domainId"></param>
        public void Refresh(Guid domainId)
        {
            DomainEntity domain = _domainManager.GetDomain(domainId);
            if (_domainContextCached.Contains(domainId))
            {
                ((T)_domainContextCached[domain.Id]).Domain = domain;
            }
            else
            {
                lock (_domainContextCachedLockObj)
                {
                    if (_domainContextCached.Contains(domainId))
                    {
                        ((T)_domainContextCached[domain.Id]).Domain = domain;
                    }
                    else
                    {
                        T domainContext = Create(domain);
                        _domainContextCached.Add(domainId, domainContext);
                    }
                }
            }

            OnRefresh((T)_domainContextCached[domain.Id]);
        }

        private T Create(DomainEntity domain)
        {
            //T domainContext = new T(domain);
            T domainContext = (T)System.Activator.CreateInstance(typeof(T), domain);
            return domainContext;
        }

        /// <summary>
        /// 根据AppId获取所属DomainId
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Guid? GetDomainId(string appId)
        {
            if (String.IsNullOrEmpty(appId))
                return null;

            if (_appIdDomainIdMapping.ContainsKey(appId))
            {
                return (Guid)_appIdDomainIdMapping[appId];
            }
            else
            {
                lock (_appIdDomainIdMappingLockObj)
                {
                    if (_appIdDomainIdMapping.ContainsKey(appId))
                    {
                        return (Guid)_appIdDomainIdMapping[appId];
                    }

                    List<CommandParameter> parameterList = new List<CommandParameter>();
                    parameterList.Add(new CommandParameter("@appId", appId));

                    Guid domainId = Guid.Empty;
                    if (_dataBase.ExecuteScalar<Guid>(
                        "SELECT [Domain] FROM [Authorizer] WHERE [AppId] = @appId AND [Online] = 1", parameterList,
                        (scalarValue) => { domainId = scalarValue; }))
                    {
                        _appIdDomainIdMapping.Add(appId, domainId);
                        return domainId;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 授权完成
        /// </summary>
        /// <param name="domainContext"></param>
        protected virtual void OnDocking(DomainContext domainContext)
        {
            _logService.Write("DomainPool.OnDocking",
                JsonHelper.Serializer(domainContext.Domain), TraceEventType.Verbose);

        }

        /// <summary>
        /// 解除授权
        /// </summary>
        /// <param name="domainContext"></param>
        protected virtual void UnDocking(DomainContext domainContext)
        {

        }


        /// <summary>
        /// 刷新了缓存中的Domain
        /// </summary>
        /// <param name="domainContext"></param>
        protected virtual void OnRefresh(DomainContext domainContext)
        {
            domainContext.Refresh();
        }

    }
}
