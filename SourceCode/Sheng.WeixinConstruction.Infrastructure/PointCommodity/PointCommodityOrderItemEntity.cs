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
    [Table("PointCommodityOrderItem")]
    public class PointCommodityOrderItemEntity
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

        public Guid Order
        {
            get;
            set;
        }

        public Guid PointCommodity
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Point
        {
            get;
            set;
        }

        /// <summary>
        /// 金额 分
        /// </summary>
        public int Price
        {
            get;
            set;
        }

        public string ImageUrl
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
}
