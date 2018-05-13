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
    /// 1元抢购商品基本信息
    /// </summary>
    [Table("OneDollarBuyingCommodity")]
    public class OneDollarBuyingCommodityEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

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

        public string Name
        {
            get;
            set;
        }

        public int Price
        {
            get;
            set;
        }

        public bool ForSale
        {
            get;
            set;
        }

        /// <summary>
        /// 是否无限库存
        /// </summary>
        public bool InfiniteStock
        {
            get;
            set;
        }

        /// <summary>
        /// 库存
        /// </summary>
        public int Stock
        {
            get;
            set;
        }

        /// <summary>
        /// 已售
        /// </summary>
        public int Sold
        {
            get;
            set;
        }

        public string ImageUrl
        {
            get;
            set;
        }

        public string Introduction
        {
            get;
            set;
        }

        [OrderBy(OrderBy.DESC)]
        public DateTime CreateTime
        {
            get;
            set;
        }

        public Guid CreateUser
        {
            get;
            set;
        }

        //[OrderBy(OrderBy.ASC)]
        public int Sort
        {
            get;
            set;
        }

        public bool Remove
        {
            get;
            set;
        }

    }
}
