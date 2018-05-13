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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class PointCommodityCheckoutOrderArgs
    {
        public Guid DomainId
        {
            get;
            set;
        }

        public string AppId
        {
            get;
            set;
        }

        public Guid MemberId
        {
            get;
            set;
        }

        public List<PointCommodity> ItemList
        {
            get;
            set;
        }
    }

    public class PointCommodity
    {
        public Guid Id
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }
    }

    public class PointCommodityCheckoutOrderResult
    {
        public string OrderNumber
        {
            get;
            set;
        }

        public Guid OrderId
        {
            get;
            set;
        }
    }
}
