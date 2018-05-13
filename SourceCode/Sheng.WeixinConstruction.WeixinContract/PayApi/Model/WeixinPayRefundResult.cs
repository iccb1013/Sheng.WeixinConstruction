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
using System.Xml.Serialization;

namespace Sheng.WeixinConstruction.WeixinContract.PayApi
{
    /// <summary>
    /// 申请退款返回参数
    /// https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=9_4
    /// </summary>
    [XmlRootAttribute("xml")]
    public class WeixinPayRefundResult
    {
        /// <summary>
        /// SUCCESS/FAIL
        /// 此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        [XmlElement("return_code")]
        public string ReturnCode
        {
            get;
            set;
        }

        /// <summary>
        /// 返回信息，如非空，为错误原因
        /// 签名失败
        /// 参数格式校验错误
        /// </summary>
        [XmlElement("return_msg")]
        public string ReturnMsg
        {
            get;
            set;
        }

        #region 以下字段在return_code为SUCCESS的时候有返回

        /// <summary>
        /// 业务结果
        /// SUCCESS/FAIL
        /// </summary>
        [XmlElement("result_code")]
        public string ResultCode
        {
            get;
            set;
        }

        /// <summary>
        /// 错误码信息
        /// </summary>
        [XmlElement("err_code")]
        /// <remarks/>
        public string ErrCode
        {
            get;
            set;
        }

        /// <summary>
        /// 结果信息描述
        /// </summary>
        [XmlElement("err_code_des")]
        /// <remarks/>
        public string ErrCodeDes
        {
            get;
            set;
        }

        /// <summary>
        /// 公众账号ID
        /// 调用接口提交的公众账号ID
        /// </summary>
        [XmlElement("appid")]
        public string AppId
        {
            get;
            set;
        }

        /// <summary>
        /// 商户号
        /// 调用接口提交的商户号
        /// </summary>
        [XmlElement("mch_id")]
        public string MchId
        {
            get;
            set;
        }

        /// <summary>
        /// 设备号
        /// 调用接口提交的终端设备号
        /// </summary>
        [XmlElement("device_info")]
        public string DeviceInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 随机字符串
        /// 微信返回的随机字符串
        /// </summary>
        [XmlElement("nonce_str")]
        public string NonceStr
        {
            get;
            set;
        }

        /// <summary>
        /// 签名
        /// 微信返回的签名，详见签名算法
        /// </summary>
        [XmlElement("sign")]
        public string Sign
        {
            get;
            set;
        }

        /// <summary>
        /// 微信订单号
        /// </summary>
        [XmlElement("transaction_id")]
        public string TransactionId
        {
            get;
            set;
        }

        /// <summary>
        /// 商户订单号
        /// 商户系统内部的订单号
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo
        {
            get;
            set;
        }

        /// <summary>
        /// 商户退款单号
        /// </summary>
        [XmlElement("out_refund_no")]
        public string OutRefundNo
        {
            get;
            set;
        }

        /// <summary>
        /// 微信退款单号
        /// </summary>
        [XmlElement("refund_id")]
        public string RefundId
        {
            get;
            set;
        }

        /// <summary>
        /// 退款渠道
        /// ORIGINAL—原路退款
        /// BALANCE—退回到余额
        /// </summary>
        [XmlElement("refund_channel")]
        public string RefundChannel
        {
            get;
            set;
        }

        /// <summary>
        /// 退款金额
        /// 退款总金额,单位为分,可以做部分退款
        /// </summary>
        [XmlElement("refund_fee")]
        public int RefundFee
        {
            get;
            set;
        }

        /// <summary>
        /// 订单总金额
        /// 订单总金额，单位为分，只能为整数，详见支付金额
        /// </summary>
        [XmlElement("total_fee")]
        public int TotalFee
        {
            get;
            set;
        }

        /// <summary>
        /// 订单金额货币种类
        /// 订单金额货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        /// </summary>
        [XmlElement("fee_type")]
        public string FeeType
        {
            get;
            set;
        }

        /// <summary>
        /// 现金支付金额
        /// 现金支付金额，单位为分，只能为整数，详见支付金额
        /// </summary>
        [XmlElement("cash_fee")]
        public int CashFee
        {
            get;
            set;
        }

        /// <summary>
        /// 现金退款金额
        /// 现金退款金额，单位为分，只能为整数，详见支付金额
        /// </summary>
        [XmlElement("cash_refund_fee")]
        public int CashRefundFee
        {
            get;
            set;
        }

        #endregion
    }
}
