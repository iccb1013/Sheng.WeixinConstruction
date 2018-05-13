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
    public class MemberCenterViewModel
    {
        /// <summary>
        /// 所属会员卡
        /// 可能为null
        /// </summary>
        public MemberCardLevelEntity MemberCard
        {
            get;
            set;
        }

        ///// <summary>
        ///// 有效卡券数量
        ///// </summary>
        //public int ValidCouponCount
        //{
        //    get;
        //    set;
        //}
    }
}