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
    /// <summary>
    /// 将一条长链接转成短链接。
    /// </summary>
    [DataContract]
    public class WeixinCreateShortUrlArgs
    {
        [DataMember(Name = "action")]
        public string Action
        {
            get { return "long2short"; }
        }

        [DataMember(Name = "long_url")]
        public string LongUrl
        {
            get;
            set;
        }
    }
}
