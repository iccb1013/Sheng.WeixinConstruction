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


using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Client.Core
{
    /*
     * 客户端不需要刷AccessToken
     * 如果客户端的 DomainPool 先于服务端的 DomainPool 建立了某个 Domain 的上下文
     * 那也没关系 ，因为在使用 AccessToken 的时候，AccessTokenGetter.Get 会在取不到Token时，去刷新
     * 如果服务端始终没有在 DomainPool 建议并维护此 Domain，客户端依赖 AccessTokenGetter.Get 方法
     * 完成AccessToken的刷新
     * 但是这样做使得服务端的定时器刷新 AccessToken 意义不大了
     * 取消服务端的定时器刷新机制，统一在 AccessTokenGetter.Get 中处理
     */
    public class ClientDomainPool : DomainPool<ClientDomainContext>
    {
        private static readonly ClientDomainPool _instance = new ClientDomainPool();
        public static ClientDomainPool Instance
        {
            get { return _instance; }
        }

        private ClientDomainPool()
        {
            
        }

        //protected override void OnRefresh(DomainContext domainContext)
        //{
        //    base.OnRefresh(domainContext);

        //    ClientDomainContext clientDomainContext = domainContext as ClientDomainContext;
        //    if (clientDomainContext == null)
        //    {
        //        _logService.Write("ClientDomainPool 在处理 OnRefresh 事件时遇到了非 ClientDomainContext ",
        //           TraceEventType.Error);
        //        return;
        //    }

        //    clientDomainContext.Refresh();

        //}
    }
}
