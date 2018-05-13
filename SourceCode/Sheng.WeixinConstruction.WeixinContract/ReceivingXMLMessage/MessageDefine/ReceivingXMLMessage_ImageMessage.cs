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
     * 注意，同样是图片消息，但是呆逼微信的接口对于接收和发送，格式是不一样的
     * 
    <xml>
        <ToUserName><![CDATA[toUser]]></ToUserName>
        <FromUserName><![CDATA[fromUser]]></FromUserName>
        <CreateTime>12345678</CreateTime>
        <MsgType><![CDATA[image]]></MsgType>
        <Image>
         <MediaId><![CDATA[media_id]]></MediaId>
        </Image>
    </xml>
     * 
       ToUserName	是	接收方帐号（收到的OpenID）
        FromUserName	是	开发者微信号
        CreateTime	是	消息创建时间 （整型）
        MsgType	是	image
        MediaId	是	通过素材管理接口上传多媒体文件，得到的id。
     */

    [XmlRootAttribute("xml")]
    public class ReceivingXMLMessage_ImageMessage : ReceivingXMLMessageBase
    {
        /// <summary>
        /// 图片链接
        /// </summary>
        [XmlElement("PicUrl")]
        public string PicUrl
        {
            get;
            set;
        }

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
        /// 接收普通消息时有
        /// </summary>
        [XmlElement("MsgId")]
        public long MsgId
        {
            get;
            set;
        }

        public ReceivingXMLMessage_ImageMessage()
        {
            //this.MsgType = "image";
        }
    }

}
