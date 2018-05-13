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
    public class PointCommodityShoppingCartOperateResult
    {
        /// <summary>
        /// 该商品的最新数量
        /// </summary>
        public int Quantity
        {
            get;
            set;
        }

        /// <summary>
        /// 购物车共有多少 种 商品
        /// </summary>
        public int PointCommodityCount
        {
            get;
            set;
        }
    }
}
