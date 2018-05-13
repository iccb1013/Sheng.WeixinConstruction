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
    /*
     * 发红包返回参数
     * https://pay.weixin.qq.com/wiki/doc/api/cash_coupon.php?chapter=13_5
     */
    [XmlRootAttribute("xml")]
    public class WeixinPaySendRedpackResult
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

        /// <summary>
        /// 生成签名方式详见签名生成算法
        /// </summary>
        [XmlElement("sign")]
        public string Sign
        {
            get;
            set;
        }

        /// <summary>
        /// SUCCESS/FAIL
        /// </summary>
        [XmlElement("result_code")]
        /// <remarks/>
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
        /// 商户订单号（每个订单号必须唯一）
        /// 组成：mch_id+yyyymmdd+10位一天内不能重复的数字
        /// </summary>
        [XmlElement("mch_billno")]
        /// <remarks/>
        public string MchBillno
        {
            get;
            set;
        }

        /// <summary>
        /// 微信支付分配的商户号
        /// </summary>
        [XmlElement("mch_id")]
        public string MchId
        {
            get;
            set;
        }

        /// <summary>
        /// 商户appid，接口传入的所有appid应该为公众号的appid（在mp.weixin.qq.com申请的），
        /// 不能为APP的appid（在open.weixin.qq.com申请的）。
        /// </summary>
        [XmlElement("wxappid")]
        public string WxAppId
        {
            get;
            set;
        }

        /// <summary>
        /// 接受收红包的用户
        /// 用户在wxappid下的openid
        /// </summary>
        [XmlElement("re_openid")]
        public string ReOpenId
        {
            get;
            set;
        }

        /// <summary>
        /// 付款金额，单位分
        /// </summary>
        [XmlElement("total_amount")]
        public int TotalAmount
        {
            get;
            set;
        }

        /// <summary>
        /// 红包发送时间
        /// </summary>
        [XmlElement("send_time")]
        public string SendTime
        {
            get;
            set;
        }

        /// <summary>
        /// 红包订单的微信单号
        /// </summary>
        [XmlElement("send_listid")]
        public string SendListid
        {
            get;
            set;
        }

    }
}
