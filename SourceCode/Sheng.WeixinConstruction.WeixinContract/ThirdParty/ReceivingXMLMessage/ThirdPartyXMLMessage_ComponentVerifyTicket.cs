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

namespace Sheng.WeixinConstruction.WeixinContract.ThirdParty
{
    /// <summary>
    /// 推送component_verify_ticket协议
    /// </summary>
    [XmlRootAttribute("xml")]
    public class ThirdPartyXMLMessage_ComponentVerifyTicket
    {
        /// <summary>
        /// 第三方平台appid
        /// </summary>
        [XmlElement("AppId")]
        public string AppId
        {
            get;
            set;
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        [XmlElement("CreateTime")]
        public int CreateTime
        {
            get;
            set;
        }

        /// <summary>
        /// component_verify_ticket
        /// </summary>
        [XmlElement("InfoType")]
        public string InfoType
        {
            get;
            set;
        }

        /// <summary>
        /// Ticket内容
        /// </summary>
        [XmlElement("ComponentVerifyTicket")]
        public string ComponentVerifyTicket
        {
            get;
            set;
        }
    }
}
