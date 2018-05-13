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
    public class WeixinTagBatchTaggingArgs
    {
        [DataMember(Name = "data")]
        public WeixinTagBatchTaggingArgs_Data Data { get; set; }

        /// <summary>
        /// 微信接口永远会返回 NextOpenId，哪怕数据只有几条不需要分次获取
        /// NextOpenId 就是 Data 中 OpenId 列表的最后一个
        /// </summary>
        [DataMember(Name = "tagid")]
        public int TagId { get; set; }

    }

    [DataContract]
    public class WeixinTagBatchTaggingArgs_Data
    {
        [DataMember(Name = "openid_list")]
        public string[] OpenIdList { get; set; }
    }

}
