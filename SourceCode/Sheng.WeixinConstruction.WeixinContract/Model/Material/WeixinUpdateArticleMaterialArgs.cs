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
    public class WeixinUpdateArticleMaterialArgs
    {
        [DataMember(Name = "media_id")]
        public string MediaId
        {
            get;
            set;
        }

        [DataMember(Name = "index")]
        public int Index
        {
            get;
            set;
        }

        [DataMember(Name = "articles")]
        public WeixinArticleMaterial Article
        {
            get;
            set;
        }
    }
}
