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
    public class WeixinNormalMaterial : WeixinMaterialListItemBase
    {
        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 如果是图片，能返回URL，如：
        /// http://mmbiz.qpic.cn/mmbiz/9wc35jemHx04SS4g2uTJW3a0sImfKdKTdlkUyyUqq4ic0gZ4KibxfIcS4O9Wia32xMtwuvEia9CicRiaXAnAQJ1oLYtQ/0?wx_fmt=jpeg
        /// 但是无法在腾讯系以外域名使用
        /// </summary>
        [DataMember(Name = "url")]
        public string Url
        {
            get;
            set;
        }
    }
}
