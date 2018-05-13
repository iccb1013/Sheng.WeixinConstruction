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
        <CreateTime>1351776360</CreateTime>
        <MsgType><![CDATA[location]]></MsgType>
        <Location_X>23.134521</Location_X>
        <Location_Y>113.358803</Location_Y>
        <Scale>20</Scale>
        <Label><![CDATA[位置信息]]></Label>
        <MsgId>1234567890123456</MsgId>
     </xml> 
     * 
        ToUserName	开发者微信号
        FromUserName	发送方帐号（一个OpenID）
        CreateTime	消息创建时间 （整型）
        MsgType	location
        Location_X	地理位置维度
        Location_Y	地理位置经度
        Scale	地图缩放大小
        Label	地理位置信息
        MsgId	消息id，64位整型
     */
    /// <summary>
    /// 地理位置消息
    /// </summary>
    [XmlRootAttribute("xml")]
    public class ReceivingXMLMessage_LocationMessage : ReceivingXMLMessageBase
    {
        [XmlElement("Location_X")]
        public decimal Location_X
        {
            get;
            set;
        }

        [XmlElement("Location_Y")]
        public decimal Location_Y
        {
            get;
            set;
        }

        [XmlElement("Scale")]
        public int Scale
        {
            get;
            set;
        }

        [XmlElement("Label")]
        public string Label
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

        public ReceivingXMLMessage_LocationMessage()
        {
           // this.MsgType = "location";
        }
    }

}
