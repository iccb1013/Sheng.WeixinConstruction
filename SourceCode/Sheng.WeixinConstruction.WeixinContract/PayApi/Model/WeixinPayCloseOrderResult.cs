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
    /// 关闭订单返回参数
    /// https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=9_3
    /// </summary>
    [XmlRootAttribute("xml")]
    public class WeixinPayCloseOrderResult
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

        //result_code 在文档里表格里没有示例里有，推测是漏掉了
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
        /// 错误代码描述
        /// 结果信息描述
        /// </summary>
        [XmlElement("err_code_des")]
        public string ErrCodeDes
        {
            get;
            set;
        }

        #endregion
    }
}
