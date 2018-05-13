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


using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class PointCommodityDetailViewModel
    {
        public PointCommodityEntity PointCommodity
        {
            get;
            set;
        }

        /// <summary>
        /// 购物车中的商品 种类 数量
        /// </summary>
        public int ShoppingCartPointCommodityCount
        {
            get;
            set;
        }
    }
}