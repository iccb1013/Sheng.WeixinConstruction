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
    [Table("Coupon")]
    public class CouponEntity
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

        public string ImageUrl
        {
            get;
            set;
        }

        public string Condition
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public bool InfiniteStock
        {
            get;
            set;
        }

        public int Stock
        {
            get;
            set;
        }

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

        public string Remark
        {
            get;
            set;
        }

        public bool Removed
        {
            get;
            set;
        }
    }
}
