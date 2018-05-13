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
    public class OrderOneDollarBuyingCommodityArgs
    {
        public Guid SaleId
        {
            get;
            set;
        }

        public Guid MemberId
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

    public class OrderOneDollarBuyingCommodityResult
    {
        /// <summary>
        /// 是否兑换成功
        /// </summary>
        public bool Success
        {
            get { return Reason == 0; }
        }

        /// <summary>
        /// 订购失败时的原因代码
        /// </summary>
        public int Reason
        {
            get;
            set;
        }

        /// <summary>
        /// 实际买入份数
        /// </summary>
        public int OrderQuantity
        {
            get;
            set;
        }


    }
}
