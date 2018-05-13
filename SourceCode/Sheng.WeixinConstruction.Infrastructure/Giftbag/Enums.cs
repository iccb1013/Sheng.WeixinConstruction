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
    /// <summary>
    /// 新人礼包领取方式
    /// </summary>
    public enum EnumGiftbagPayment
    {
        /// <summary>
        /// 关注即可领取
        /// </summary>
        [EnumMember(Value = "关注即可领取")]
        Attention = 0,
        /// <summary>
        /// 完善会员信息（验证手机号）
        /// </summary>
        [EnumMember(Value = "完善会员信息")]
        FillInPersonalInfo = 1
    }
}
