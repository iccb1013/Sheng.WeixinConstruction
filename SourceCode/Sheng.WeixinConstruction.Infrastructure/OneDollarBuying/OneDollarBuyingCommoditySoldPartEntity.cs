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


using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    /// <summary>
    /// 1元抢购商品售出号码
    /// </summary>
    [Table("OneDollarBuyingCommoditySoldPart")]
    public class OneDollarBuyingCommoditySoldPartEntity
    {
        public Guid Domain
        {
            get;
            set;
        }

        public string AppId
        {
            get;
            set;
        }

        public Guid CommodityId
        {
            get;
            set;
        }

        public Guid SaleId
        {
            get;
            set;
        }

        public Guid Member
        {
            get;
            set;
        }

        public int PartNumber
        {
            get;
            set;
        }

        public DateTime SoldTime
        {
            get;
            set;
        }
    }
}
