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
    /*
     推送XML数据包示例：
    <xml><ToUserName><![CDATA[toUser]]></ToUserName>
    <FromUserName><![CDATA[fromUser]]></FromUserName>
    <CreateTime>1442401156</CreateTime>
    <MsgType><![CDATA[event]]></MsgType>
    <Event><![CDATA[qualification_verify_success]]></Event>
    <ExpiredTime>1442401156</ExpiredTime>
    </xml> 
     */

    /// <summary>
    /// 资质认证成功（此时立即获得接口权限）
    /// </summary>
    [XmlRootAttribute("xml")]
    public class ReceivingXMLMessage_QualificationVerifySuccess : ReceivingXMLMessageBase
    {
        /// <summary>
        /// qualification_verify_success
        /// </summary>
        [XmlElement("Event")]
        public string Event
        {
            get;
            set;
        }

        /// <summary>
        /// 有效期 (整形)，指的是时间戳，将于该时间戳认证过期
        /// </summary>
        [XmlElement("ExpiredTime")]
        public int ExpiredTime
        {
            get;
            set;
        }
    }
}
