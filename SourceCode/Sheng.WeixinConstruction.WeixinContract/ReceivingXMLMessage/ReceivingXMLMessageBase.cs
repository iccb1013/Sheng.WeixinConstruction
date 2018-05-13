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
     * 
     * 傻逼微信同时使用了XML和JSON
     * 傻逼微信对于同样的消息，接收的XML和发送的XML是不一样的格式
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
     * 
     * XML：
     * http://mp.weixin.qq.com/wiki/18/c66a9f0b5aa952346e46dc39de20f672.html#.E5.9B.9E.E5.A4.8D.E5.9B.BE.E7.89.87.E6.B6.88.E6.81.AF
     * JSON:
     * http://mp.weixin.qq.com/wiki/14/d9be34fe03412c92517da10a5980e7ee.html#.E6.8E.A5.E5.8F.A3.E7.9A.84.E7.BB.9F.E4.B8.80.E5.8F.82.E6.95.B0.E8.AF.B4.E6.98.8E
    */

    /// <summary>
    /// 接收到的XML消息的基类
    /// </summary>
    [XmlRootAttribute("xml")]
    public class ReceivingXMLMessageBase
    {
        [XmlElement("ToUserName")]
        public string ToUserName
        {
            get;
            set;
        }

        [XmlElement("FromUserName")]
        public string FromUserName
        {
            get;
            set;
        }

        [XmlElement("CreateTime")]
        public int CreateTime
        {
            get;
            set;
        }

        [XmlElement("MsgType")]
        public string MsgType
        {
            get;
            set;
        }
    }
}
