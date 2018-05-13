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
   <Event><![CDATA[subscribe]]></Event>
   </xml>
    * 
      ToUserName	开发者微信号
       FromUserName	发送方帐号（一个OpenID）
       CreateTime	消息创建时间 （整型）
       MsgType	消息类型，event
       Event	事件类型，subscribe(订阅)、unsubscribe(取消订阅)
    */
    /// <summary>
    /// 取消关注
    /// </summary>
    [XmlRootAttribute("xml")]
    public class ReceivingXMLMessage_UnsubscribeEventMessage : ReceivingXMLMessageBase
    {
        [XmlElement("Event")]
        public string Event
        {
            get;
            set;
        }
    }
}
