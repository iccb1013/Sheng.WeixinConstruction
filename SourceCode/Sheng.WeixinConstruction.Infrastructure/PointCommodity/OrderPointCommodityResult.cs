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
    public class OrderPointCommodityResult
    {
        /// <summary>
        /// 是否兑换成功
        /// </summary>
        public bool Success
        {
            get { return Reason == 0; }
        }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNumber
        {
            get;
            set;
        }

        public string OrderId
        {
            get;
            set;
        }

        /// <summary>
        /// 订购失败时的原因代码
        /// </summary>
        public int Reason
        {
            get;
            set;
        }

    }
}
