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
     */
    /// <summary>
    /// 回复图片消息
    /// </summary>
    [XmlRootAttribute("xml")]
    public class ResponsiveXMLMessage_ImageMessage : ResponsiveXMLMessageBase
    {
        private ResponsiveXMLMessage_ImageMessage_Image _image = new ResponsiveXMLMessage_ImageMessage_Image();
        [XmlElement("Image")]
        public ResponsiveXMLMessage_ImageMessage_Image Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public ResponsiveXMLMessage_ImageMessage()
        {
            this.MsgType = "image";
        }
    }

    public class ResponsiveXMLMessage_ImageMessage_Image
    {
        [XmlElement("MediaId")]
        public string MediaId
        {
            get;
            set;
        }
    }
}
