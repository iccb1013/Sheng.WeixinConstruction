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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    [DataContract]
    public class WeixinGetJsApiTicketResult
    {
        [DataMember(Name="ticket")]
        public string Ticket
        {
            get;
            set;
        }

        /// <summary>
        /// 微信API返回的单位是秒
        /// </summary>
        [DataMember(Name = "expires_in")]
        public int ExpiresIn
        {
            get;
            set;
        }
    }
}
