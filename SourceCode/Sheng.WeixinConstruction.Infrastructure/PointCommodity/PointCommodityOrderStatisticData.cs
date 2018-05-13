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
    public class PointCommodityOrderStatisticData
    {
        /// <summary>
        /// 订单总数
        /// </summary>
        public int TotalOrder
        {
            get;
            set;
        }

        /// <summary>
        /// 待支付
        /// </summary>
        public int NoPayment
        {
            get;
            set;
        }

        /// <summary>
        /// 已支付
        /// </summary>
        public int Paid
        {
            get;
            set;
        }

        /// <summary>
        /// 已发货
        /// </summary>
        public int Deal
        {
            get;
            set;
        }

        /// <summary>
        /// 已取消
        /// </summary>
        public int Canceled
        {
            get;
            set;
        }
    }
}
