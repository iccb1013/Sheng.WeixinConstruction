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
    /// 统一下单返回参数
    /// https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=9_1
    /// </summary>
    [XmlRootAttribute("xml")]
    public class WeixinPayUnifiedOrderResult
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
        /// 预支付交易会话标识
        /// 微信生成的预支付回话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        [XmlElement("prepay_id")]
        public string PrepayId
        {
            get;
            set;
        }

        /// <summary>
        /// 二维码链接
        /// trade_type为NATIVE是有返回，可将该参数值生成二维码展示出来进行扫码支付
        /// </summary>
        [XmlElement("code_url")]
        public string CodeUrl
        {
            get;
            set;
        }

        #endregion
    }
}
