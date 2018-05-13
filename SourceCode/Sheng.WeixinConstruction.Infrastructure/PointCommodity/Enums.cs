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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public enum PointCommodityOrderStatus
    {
        [EnumMember(Value = "未知")]
        Unknow = 0,
        /// <summary>
        /// 已支付
        /// </summary>
        [EnumMember(Value = "已支付")]
        Order = 1,
        /// <summary>
        /// 已领取
        /// </summary>
        [EnumMember(Value = "已发货")]
        Deal = 2,
        /// <summary>
        /// 已取消
        /// </summary>
        [EnumMember(Value = "已取消")]
        Cancel = 3,
        /// <summary>
        /// 待支付
        /// </summary>
        [EnumMember(Value = "待支付")]
        NoPayment = 4
    }
}
