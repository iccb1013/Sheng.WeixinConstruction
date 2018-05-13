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
using System.Data;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class OneDollarBuyingCommodityDetailViewModel
    {
        /// <summary>
        /// 商品信息
        /// </summary>
        public OneDollarBuyingCommodityEntity Commodity
        {
            get;
            set;
        }

        /// <summary>
        /// 本期上架信息
        /// </summary>
        public OneDollarBuyingCommodityForSaleEntity ForSale
        {
            get;
            set;
        }

        public MemberEntity LuckyMember
        {
            get;
            set;
        }

        public int LuckyMemberPartNumberCount
        {
            get;
            set;
        }

        /// <summary>
        /// 我参与的号码
        /// </summary>
        public List<int> MyPartNumberList
        {
            get;
            set;
        }

        /// <summary>
        /// 正在销售中的下一期
        /// </summary>
        public Guid? AvailableSaleId
        {
            get;
            set;
        }
    }
}