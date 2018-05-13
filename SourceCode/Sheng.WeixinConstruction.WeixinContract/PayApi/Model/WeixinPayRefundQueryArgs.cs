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
    /// 查询退款
    /// https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=9_5
    /// 由于微信接口返回的XML字段不固定，是根据退款笔数来的，所以此接口的返回XML无法定义与接口对应的模型类
    /// </summary>
    public class WeixinPayRefundQueryArgs
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
        /// 商户号
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

        //四选一

        /// <summary>
        /// 微信订单号
        /// transaction_id
        /// </summary>
        public string TransactionId
        {
            get;
            set;
        }

        /// <summary>
        /// 商户订单号
        /// 商户系统内部的订单号
        /// out_trade_no
        /// </summary>
        public string OutTradeNo
        {
            get;
            set;
        }

        /// <summary>
        /// 商户退款单号
        /// 商户侧传给微信的退款单号
        /// out_refund_no
        /// </summary>
        public string OutRefundNo
        {
            get;
            set;
        }

        /// <summary>
        /// 微信退款单号
        /// 微信生成的退款单号，在申请退款接口有返回
        /// refund_id
        /// </summary>
        public string RefundId
        {
            get;
            set;
        }

    }
}
