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
     <xml>
<ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[SCAN]]></Event>
<EventKey><![CDATA[SCENE_VALUE]]></EventKey>
<Ticket><![CDATA[TICKET]]></Ticket>
</xml>
     * 
     ToUserName	开发者微信号
FromUserName	发送方帐号（一个OpenID）
CreateTime	消息创建时间 （整型）
MsgType	消息类型，event
Event	事件类型，SCAN
EventKey	事件KEY值，是一个32位无符号整数，即创建二维码时的二维码scene_id
Ticket	二维码的ticket，可用来换取二维码图片
     */

    /// <summary>
    /// 扫描带参数二维码事件
    /// 用户已关注时的事件推送
    /// </summary>
    [XmlRootAttribute("xml")]
    public class ReceivingXMLMessage_ScanEventMessage : ReceivingXMLMessageBase
    {
        [XmlElement("Event")]
        public string Event
        {
            get;
            set;
        }

        /// <summary>
        /// 事件KEY值，qrscene_为前缀，后面为二维码的参数值
        /// </summary>
        [XmlElement("EventKey")]
        public string EventKey
        {
            get;
            set;
        }

        /// <summary>
        /// 二维码的ticket，可用来换取二维码图片
        /// </summary>
        [XmlElement("Ticket")]
        public string Ticket
        {
            get;
            set;
        }
    }
}
