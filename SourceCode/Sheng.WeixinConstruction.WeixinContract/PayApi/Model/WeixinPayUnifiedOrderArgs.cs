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
    /// 统一下单
    /// https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=9_1
    /// </summary>
    public class WeixinPayUnifiedOrderArgs
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
        /// 支付通知中将把此字段原样返回
        /// 在此处存储本地的 PayOrder Id 
        /// 微信虽然不允许针对同样的 OutTradeNo 创建多个订单，但实测中出现了同一个 OutTradeNo 被下多个订单的情况，后来未重现
        /// 所以当 创建了多个微信支付订单时，支付其中的一笔，在回调中根据 OutTradeNo 就不够保险
        /// 这时需要根据本地的 PayOrder Id 来区分 
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

        private string _feeType = "CNY";
        /// <summary>
        /// 货币类型
        /// 符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        /// fee_type
        /// </summary>
        public string FeeType
        {
            get { return _feeType; }
            set { _feeType = value; }
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
        public string TimeStart
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
        public string TimeExpire
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
        /// 通知地址
        /// 接收微信支付异步通知回调地址，通知url必须为直接可访问的url，不能携带参数。
        /// notify_url
        /// </summary>
        public string NotifyUrl
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
    }
}
