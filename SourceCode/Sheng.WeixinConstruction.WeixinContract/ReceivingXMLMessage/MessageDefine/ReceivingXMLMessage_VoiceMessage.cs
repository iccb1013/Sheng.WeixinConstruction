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
     * http://mp.weixin.qq.com/wiki/17/f298879f8fb29ab98b2f2971d42552fd.html#.E8.AF.AD.E9.9F.B3.E6.B6.88.E6.81.AF
     * <xml>
        <ToUserName><![CDATA[toUser]]></ToUserName>
        <FromUserName><![CDATA[fromUser]]></FromUserName>
        <CreateTime>1357290913</CreateTime>
        <MsgType><![CDATA[voice]]></MsgType>
        <MediaId><![CDATA[media_id]]></MediaId>
        <Format><![CDATA[Format]]></Format>
        <MsgId>1234567890123456</MsgId>
        </xml>
     * 
     * ToUserName	开发者微信号
        FromUserName	发送方帐号（一个OpenID）
        CreateTime	消息创建时间 （整型）
        MsgType	语音为voice
        MediaId	语音消息媒体id，可以调用多媒体文件下载接口拉取数据。
        Format	语音格式，如amr，speex等
        MsgID	消息id，64位整型
     */
    /// <summary>
    /// 语音消息
    /// </summary>
    [XmlRootAttribute("xml")]
    public class ReceivingXMLMessage_VoiceMessage : ReceivingXMLMessageBase
    {
        /// <summary>
        /// 图片消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        [XmlElement("MediaId")]
        public string MediaId
        {
            get;
            set;
        }

        /// <summary>
        /// 语音格式，如amr，speex等
        /// </summary>
        [XmlElement("Format")]
        public string Format
        {
            get;
            set;
        }

        /// <summary>
        /// 接收普通消息时有
        /// </summary>
        [XmlElement("MsgId")]
        public long MsgId
        {
            get;
            set;
        }
    }
}
