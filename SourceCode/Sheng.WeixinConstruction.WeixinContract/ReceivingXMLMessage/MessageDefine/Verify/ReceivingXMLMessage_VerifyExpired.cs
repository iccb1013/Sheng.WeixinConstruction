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

namespace Sheng.WeixinConstruction.WeixinContract
{
    /// <summary>
    /// 认证过期失效通知
    /// </summary>
    [XmlRootAttribute("xml")]
    public class ReceivingXMLMessage_VerifyExpired : ReceivingXMLMessageBase
    {
        /// <summary>
        /// verify_expired
        /// </summary>
        [XmlElement("Event")]
        public string Event
        {
            get;
            set;
        }

        /// <summary>
        /// 有效期 (整形)，指的是时间戳，表示已于该时间戳认证过期，需要重新发起微信认证
        /// </summary>
        [XmlElement("ExpiredTime")]
        public int ExpiredTime
        {
            get;
            set;
        }
    }
}
