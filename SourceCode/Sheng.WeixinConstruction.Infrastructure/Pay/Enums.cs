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
    /// 支付订单的类型
    /// </summary>
    public enum EnumPayOrderType
    {
        [EnumMember(Value = "未知")]
        Unknow = 0,
        /// <summary>
        /// 账户充值
        /// </summary>
        [EnumMember(Value = "账户充值")]
        Deposit = 1,
        /// <summary>
        /// 积分商城
        /// </summary>
        [EnumMember(Value = "积分商城")]
        PointCommodity = 2,
        /// <summary>
        /// 在线捐款
        /// </summary>
        [EnumMember(Value = "在线捐款")]
        Donation = 3
    }

    /// <summary>
    /// 订单状态  和微信接口的 TradeState 一致
    /// </summary>
    public enum EnumPayTradeState
    {
        /// <summary>
        /// 支付成功
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// 转入退款
        /// </summary>
        REFUND = 1,
        /// <summary>
        /// 未支付
        /// </summary>
        NOTPAY = 2,
        /// <summary>
        /// 已关闭
        /// </summary>
        CLOSED = 3,
        /// <summary>
        /// 已撤销（刷卡支付）
        /// </summary>
        REVOKED = 4,
        /// <summary>
        /// 用户支付中
        /// </summary>
        USERPAYING = 5,
        /// <summary>
        /// 支付失败(其他原因，如银行返回失败)
        /// </summary>
        PAYERROR = 6
    }

    public enum EnumCashAccountTrackType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EnumMember(Value = "未知")]
        Unknow = 0,
        /// <summary>
        /// 账户充值
        /// </summary>
        [EnumMember(Value = "账户充值")]
        Deposit = 1,
        /// <summary>
        /// 一般消费
        /// </summary>
        [EnumMember(Value = "一般消费")]
        Charge = 2,
        /// <summary>
        /// 现金充值，不走微信支付接口
        /// </summary>
        [EnumMember(Value = "现金充值")]
        CashDeposit = 3,
        /// <summary>
        /// 现金退款，不走微信退款接口
        /// </summary>
        [EnumMember(Value = "现金退款")]
        CashRefund = 4,
        /// <summary>
        /// 1元抢购消费
        /// </summary>
        [EnumMember(Value = "1元抢购")]
        OneDollarBuying = 5,
        /// <summary>
        /// 积分商城
        /// </summary>
        [EnumMember(Value = "积分商城")]
        PointCommodity = 6

    }
}
