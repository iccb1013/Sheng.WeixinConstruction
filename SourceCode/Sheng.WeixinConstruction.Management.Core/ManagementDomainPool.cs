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
using Sheng.WeixinConstruction.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Management.Core
{
    /// <summary>
    /// 管理端所使用的默认实现
    /// </summary>
    public class ManagementDomainPool : DomainPool<ManagementDomainContext>
    {
        private static readonly ManagementDomainPool _instance = new ManagementDomainPool();
        public static ManagementDomainPool Instance
        {
            get { return _instance; }
        }

        private ManagementDomainPool()
        {

        }

        protected override void OnDocking(DomainContext domainContext)
        {
            base.OnDocking(domainContext);

            ManagementDomainContext managementDomainContext = domainContext as ManagementDomainContext;

            if (managementDomainContext != null)
            {
                managementDomainContext.SyncMember();
            }
            else
            {
                _logService.Write("ManagementDomainPool 在处理 Docking 事件时遇到了非 ManagementDomainContext ",
                    TraceEventType.Error);
            }
        }

        //protected override void OnRefresh(DomainContext domainContext)
        //{
        //    base.OnRefresh(domainContext);

        //    ManagementDomainContext managementDomainContext = domainContext as ManagementDomainContext;
        //    if (managementDomainContext == null)
        //    {
        //        _logService.Write("ManagementDomainPool 在处理 OnRefresh 事件时遇到了非 ManagementDomainContext ",
        //           TraceEventType.Error);
        //        return;
        //    }

        //    domainContext.Refresh();
        //}
    }
}
