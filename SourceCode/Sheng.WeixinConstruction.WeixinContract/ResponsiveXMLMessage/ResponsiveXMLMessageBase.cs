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
    */

    /// <summary>
    /// 用于被动响应的XML消息的基类
    /// </summary>
    [XmlRootAttribute("xml")]
    public class ResponsiveXMLMessageBase
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
