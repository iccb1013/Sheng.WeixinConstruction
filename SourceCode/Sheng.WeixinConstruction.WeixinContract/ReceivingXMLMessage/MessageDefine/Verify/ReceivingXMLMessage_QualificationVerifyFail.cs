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

namespace Sheng.WeixinConstruction.WeixinContract.ReceivingXMLMessage.MessageDefine
{
    /// <summary>
    /// 资质认证失败
    /// </summary>
    [XmlRootAttribute("xml")]
    public class ReceivingXMLMessage_QualificationVerifyFail : ReceivingXMLMessageBase
    {
        /// <summary>
        /// qualification_verify_fail
        /// </summary>
        [XmlElement("Event")]
        public string Event
        {
            get;
            set;
        }

        /// <summary>
        /// 失败发生时间 (整形)，时间戳
        /// </summary>
        [XmlElement("FailTime")]
        public int FailTime
        {
            get;
            set;
        }

        /// <summary>
        /// 认证失败的原因
        /// </summary>
        [XmlElement("FailReason")]
        public string FailReason
        {
            get;
            set;
        }
    }
}
