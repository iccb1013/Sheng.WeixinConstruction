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

namespace Sheng.WeixinConstruction.WeixinContract.PayApi
{
    /// <summary>
    /// 申请退款
    /// https://api.mch.weixin.qq.com/secapi/pay/refund
    /// </summary>
    public class WeixinPayRefundArgs
    {
        /// <summary>
        /// 公众账号ID
        /// 微信分配的公众账号ID（企业号corpid即为此appId）
        /// appid
        /// </summary>
        public string AppId
        {
            get;
            set;
        }

        /// <summary>
        /// 微信支付分配的商户号
        /// mch_id
        /// </summary>
        public string MchId
        {
            get;
            set;
        }

        /// <summary>
        /// 设备号
        /// 终端设备号(门店号或收银设备ID)，注意：PC网页或公众号内支付请传"WEB"
        /// device_info
        /// </summary>
        public string DeviceInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 随机字符串
        /// 随机字符串，不长于32位。推荐随机数生成算法
        /// nonce_str
        /// </summary>
        public string NonceStr
        {
            get;
            set;
        }

        /// <summary>
        /// 签名
        /// 签名，详见签名生成算法
        /// sign
        /// </summary>
        public string Sign
        {
            get;
            set;
        }

        /// <summary>
        /// 微信订单号
        /// 微信的订单号，优先使用
        /// transaction_id
        /// </summary>
        public string TransactionId
        {
            get;
            set;
        }

        /// <summary>
        /// 商户订单号
        /// 商户系统内部的订单号，当没提供transaction_id时需要传这个。
        /// out_trade_no
        /// </summary>
        public string OutTradeNo
        {
            get;
            set;
        }

        /// <summary>
        /// 商户退款单号
        /// 商户系统内部的退款单号，商户系统内部唯一，同一退款单号多次请求只退一笔
        /// out_trade_no
        /// </summary>
        public string OutRefundNo
        {
            get;
            set;
        }

        /// <summary>
        /// 总金额
        /// 订单总金额，单位为分，只能为整数，详见支付金额
        /// </summary>
        public int TotalFee
        {
            get;
            set;
        }

        /// <summary>
        /// 退款金额
        /// 退款总金额，订单总金额，单位为分，只能为整数，详见支付金额
        /// </summary>
        public int RefundFee
        {
            get;
            set;
        }

        /// <summary>
        /// 货币种类
        /// 货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        /// refund_fee_type
        /// </summary>
        public string RefundFeeType
        {
            get;
            set;
        }

        /// <summary>
        /// 操作员
        /// 操作员帐号, 默认为商户号
        /// op_user_id
        /// </summary>
        public string OpUserId
        {
            get;
            set;
        }
    }
}
