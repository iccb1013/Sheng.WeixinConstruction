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
    public class CreatePayOrderArgs
    {
        public EnumPayOrderType OrderType
        {
            get;
            set;
        }

        public Guid MemberId
        {
            get;
            set;
        }

        public string OpenId
        {
            get;
            set;
        }

        /// <summary>
        /// 充值金额 分
        /// </summary>
        public float Fee
        {
            get;
            set;
        }

        /// <summary>
        /// 商户订单号
        /// 商户系统内部的订单号,32个字符内、可包含字母, 其他说明见商户订单号
        /// out_trade_no
        /// </summary>
        public string OutTradeNo
        {
            get;
            set;
        }

        /// <summary>
        /// 商品描述
        /// 商品或支付单简要描述
        /// body
        /// </summary>
        public string Body
        {
            get;
            set;
        }

        /// <summary>
        /// 终端IP
        /// APP和网页支付提交用户端ip，Native支付填调用微信支付API的机器IP。
        /// spbill_create_ip
        /// </summary>
        public string SpbillCreateIp
        {
            get;
            set;
        }
    }

    public class CreatePayOrderResult
    {
        /// <summary>
        /// 生成的微信支付订单Id（本地ID）
        /// </summary>
        public Guid PayOrderId
        {
            get;
            set;
        }

        /// <summary>
        /// 预支付交易会话标识
        /// 微信生成的预支付回话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        public string PrepayId
        {
            get;
            set;
        }

    }
}
