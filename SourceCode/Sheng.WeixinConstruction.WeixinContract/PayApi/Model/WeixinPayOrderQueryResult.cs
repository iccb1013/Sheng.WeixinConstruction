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
    /// 查询订单返回参数
    /// https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=9_2
    /// </summary>
    [XmlRootAttribute("xml")]
    public class WeixinPayOrderQueryResult
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

        #endregion

        #region 以下字段在return_code 和result_code都为SUCCESS的时候有返回

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
        /// 用户标识
        /// 用户在商户appid下的唯一标识
        /// </summary>
        [XmlElement("openid")]
        public string OpenId
        {
            get;
            set;
        }

        /// <summary>
        /// 是否关注公众账号
        /// 用户是否关注公众账号，Y-关注，N-未关注，仅在公众账号类型支付有效
        /// </summary>
        [XmlElement("is_subscribe")]
        public string IsSubscribe
        {
            get;
            set;
        }

        /// <summary>
        /// 交易类型
        /// 取值如下：JSAPI，NATIVE，APP，详细说明见参数规定
        /// </summary>
        [XmlElement("trade_type")]
        public string TradeType
        {
            get;
            set;
        }

        /// <summary>
        /// 交易状态
        /// SUCCESS—支付成功
        /// REFUND—转入退款
        /// NOTPAY—未支付
        /// CLOSED—已关闭
        /// REVOKED—已撤销（刷卡支付）
        /// USERPAYING--用户支付中
        /// PAYERROR--支付失败(其他原因，如银行返回失败)
        /// </summary>
        [XmlElement("trade_state")]
        public string TradeState
        {
            get;
            set;
        }

        /// <summary>
        /// 付款银行
        /// 银行类型，采用字符串类型的银行标识
        /// </summary>
        [XmlElement("bank_type")]
        public string BankType
        {
            get;
            set;
        }

        /// <summary>
        /// 总金额
        /// 订单总金额，单位为分，详见支付金额
        /// total_fee
        /// </summary>
        [XmlElement("total_fee")]
        public int TotalFee
        {
            get;
            set;
        }

        /// <summary>
        /// 货币类型
        /// 符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        /// </summary>
        [XmlElement("fee_type")]
        public string FeeType
        {
            get;
            set;
        }

        /// <summary>
        /// 现金支付金额
        /// 现金支付金额订单现金支付金额，详见支付金额
        /// </summary>
        [XmlElement("cash_fee")]
        public int CashFee
        {
            get;
            set;
        }

        /// <summary>
        /// 现金支付货币类型
        /// 货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        /// </summary>
        [XmlElement("cash_fee_type")]
        public string CashFeeType
        {
            get;
            set;
        }

        /// <summary>
        /// 代金券或立减优惠金额
        /// “代金券或立减优惠”金额 <= 订单总金额，订单总金额-“代金券或立减优惠”金额=现金支付金额，详见支付金额
        /// </summary>
        [XmlElement("coupon_fee")]
        public int CouponFee
        {
            get;
            set;
        }

        /// <summary>
        /// 代金券或立减优惠使用数量
        /// </summary>
        [XmlElement("coupon_count")]
        public int CouponCount
        {
            get;
            set;
        }

        /*
         * SB微信接口这些数据居然是生成XML字段而不是用XML数组来存储
         代金券或立减优惠批次ID	coupon_batch_id_$n	否	String(20)	100	代金券或立减优惠批次ID ,$n为下标，从0开始编号
        代金券或立减优惠ID	coupon_id_$n	否	String(20)	10000 	代金券或立减优惠ID, $n为下标，从0开始编号
        单个代金券或立减优惠支付金额	coupon_fee_$n	否	Int	100	单个代金券或立减优惠支付金额, $n为下标，从0开始编号
         */

        public List<WeixinPayOrderQueryResult_Coupon> CouponList
        {
            get;
            set;
        }

        /// <summary>
        /// 微信支付订单号
        /// </summary>
        [XmlElement("transaction_id")]
        public string TransactionId
        {
            get;
            set;
        }

        /// <summary>
        /// 商户订单号
        /// 商户系统的订单号，与请求一致。
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo
        {
            get;
            set;
        }

        /// <summary>
        /// 附加数据
        /// 附加数据，原样返回
        /// </summary>
        [XmlElement("attach")]
        public string Attach
        {
            get;
            set;
        }

        /// <summary>
        /// 支付完成时间
        /// 订单支付时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010。其他详见时间规则
        /// </summary>
        [XmlElement("time_end")]
        public string TimeEnd
        {
            get;
            set;
        }

        /// <summary>
        /// 交易状态描述
        /// 对当前查询订单状态的描述和下一步操作的指引
        /// </summary>
        [XmlElement("trade_state_desc")]
        public string TradeStateDesc
        {
            get;
            set;
        }

        #endregion
    }
}
