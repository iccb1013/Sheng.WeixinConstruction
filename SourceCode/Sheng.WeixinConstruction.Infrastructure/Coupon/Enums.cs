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
    public enum EnumCouponStatus
    {
        //未核销
        [EnumMember(Value = "未核销")]
        Unused = 0,
        /// <summary>
        /// 已核销
        /// </summary>
        [EnumMember(Value = "已核销")]
        Deal = 1
    }
}
