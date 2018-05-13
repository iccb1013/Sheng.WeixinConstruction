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


using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("PayOrder")]
    public class PayOrderEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Guid Domain
        {
            get;
            set;
        }

        public Guid Member
        {
            get;
            set;
        }

        /// <summary>
        /// 订单类型
        /// </summary>
        public EnumPayOrderType Type
        {
            get;
            set;
        }

        #region WeixinPayUnifiedOrderArgs

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
        /// 商品详情
        /// 商品名称明细列表
        /// detail
        /// </summary>
        public string Detail
        {
            get;
            set;
        }

        /// <summary>
        /// 附加数据
        /// 附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据
        /// attach
        /// </summary>
        public string Attach
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
        /// 总金额
        /// 订单总金额，单位为分，详见支付金额
        /// total_fee
        /// </summary>
        public int TotalFee
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

        /// <summary>
        /// 交易起始时间
        /// 订单生成时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010。其他详见时间规则
        /// time_start
        /// </summary>
        public DateTime TimeStart
        {
            get;
            set;
        }

        /// <summary>
        /// 交易结束时间
        /// 订单失效时间，格式为yyyyMMddHHmmss，如2009年12月27日9点10分10秒表示为20091227091010。其他详见时间规则
        /// 注意：最短失效时间间隔必须大于5分钟
        /// time_expire
        /// </summary>
        public DateTime TimeExpire
        {
            get;
            set;
        }

        /// <summary>
        /// 商品标记
        /// 商品标记，代金券或立减优惠功能的参数，说明详见代金券或立减优惠
        /// goods_tag
        /// </summary>
        public string GoodsTag
        {
            get;
            set;
        }

        /// <summary>
        /// 交易类型
        /// 取值如下：JSAPI，NATIVE，APP，详细说明见参数规定
        /// trade_type
        /// </summary>
        public string TradeType
        {
            get;
            set;
        }

        /// <summary>
        /// 商品ID
        /// trade_type=NATIVE，此参数必传。此id为二维码中包含的商品ID，商户自行定义。
        /// product_id
        /// </summary>
        public string ProductId
        {
            get;
            set;
        }

        /// <summary>
        /// 指定支付方式
        /// no_credit--指定不能使用信用卡支付
        /// limit_pay
        /// </summary>
        public string LimitPay
        {
            get;
            set;
        }

        /// <summary>
        /// 用户标识
        /// trade_type=JSAPI，此参数必传，用户在商户appid下的唯一标识。openid如何获取，可参考【获取openid】。
        /// 企业号请使用【企业号OAuth2.0接口】获取企业号内成员userid，再调用【企业号userid转openid接口】进行转换
        /// </summary>
        public string OpenId
        {
            get;
            set;
        }

        ////////////////

        /// <summary>
        /// 预支付交易会话标识
        /// 微信生成的预支付回话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        public string PrepayId
        {
            get;
            set;
        }

        #endregion

        #region WeixinPayOrderQueryResult

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
        public EnumPayTradeState TradeState
        {
            get;
            set;
        }

        public string TradeStateDesc
        {
            get;
            set;
        }

        #endregion

        #region WeixinPayNotify

        /// <summary>
        /// 付款银行
        /// 银行类型，采用字符串类型的银行标识，银行类型见银行列表
        /// </summary>
        public string BankType
        {
            get;
            set;
        }

        /// <summary>
        /// 货币种类
        /// 货币类型，符合ISO4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        /// </summary>
        public string FeeType
        {
            get;
            set;
        }

        /// <summary>
        /// 代金券或立减优惠金额
        /// 代金券或立减优惠金额 小于等于 订单总金额，订单总金额-代金券或立减优惠金额=现金支付金额，详见支付金额
        /// </summary>
        public int CouponFee
        {
            get;
            set;
        }

        /// <summary>
        /// 代金券或立减优惠使用数量
        /// </summary>
        public int CouponCount
        {
            get;
            set;
        }

        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string TransactionId
        {
            get;
            set;
        }

        /// <summary>
        /// 支付完成时间
        /// 订单支付时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010。其他详见时间规则
        /// </summary>
        public DateTime? TimeEnd
        {
            get;
            set;
        }

        /// <summary>
        /// 是否已接收到微信服务器返回的支付通知
        /// </summary>
        public bool Notify
        {
            get;
            set;
        }

        /// <summary>
        /// SUCCESS/FAIL
        /// 此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public string Notify_ReturnCode
        {
            get;
            set;
        }

        /// <summary>
        /// 返回信息，如非空，为错误原因
        /// 签名失败
        /// 参数格式校验错误
        /// </summary>
        public string Notify_ReturnMsg
        {
            get;
            set;
        }

        /// <summary>
        /// 业务结果
        /// SUCCESS/FAIL
        /// </summary>
        public string Notify_ResultCode
        {
            get;
            set;
        }

        /// <summary>
        /// 错误码信息
        /// </summary>
        public string Notify_ErrCode
        {
            get;
            set;
        }

        /// <summary>
        /// 错误代码描述
        /// 结果信息描述
        /// </summary>
        public string Notify_ErrCodeDes
        {
            get;
            set;
        }

        #endregion
    }
}
