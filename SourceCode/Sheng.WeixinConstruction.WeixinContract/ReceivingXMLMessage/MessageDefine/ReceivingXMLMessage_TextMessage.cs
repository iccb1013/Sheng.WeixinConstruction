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
         <FromUserName><![CDATA[fromUser]]></FromUserName> 
         <CreateTime>1348831860</CreateTime>
         <MsgType><![CDATA[text]]></MsgType>
         <Content><![CDATA[this is a test]]></Content>
         <MsgId>1234567890123456</MsgId>
     </xml>
     * 
        ToUserName	开发者微信号
        FromUserName	发送方帐号（一个OpenID）
        CreateTime	消息创建时间 （整型）
        MsgType	text
        Content	文本消息内容
        MsgId	消息id，64位整型
     */

    [XmlRootAttribute("xml")]
    public class ReceivingXMLMessage_TextMessage : ReceivingXMLMessageBase
    {
        [XmlElement("Content")]
        public string Content
        {
            get;
            set;
        }

        [XmlElement("MsgId")]
        public long MsgId
        {
            get;
            set;
        }

        public ReceivingXMLMessage_TextMessage()
        {
            //this.MsgType = "text";
        }
    }

}
