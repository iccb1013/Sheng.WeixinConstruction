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
    [Table("PointCommodity")]
    public class PointCommodityEntity
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

        /// <summary>
        /// 所需积分
        /// </summary>
        public int Point
        {
            get;
            set;
        }

        /// <summary>
        /// 所需现金 分
        /// </summary>
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

        public int Stock
        {
            get;
            set;
        }

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

        [Json]
        public List<ImageWrapper> ImageList
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

        //[OrderBy(OrderBy.ASC)]
        public int Sort
        {
            get;
            set;
        }
    }
}
