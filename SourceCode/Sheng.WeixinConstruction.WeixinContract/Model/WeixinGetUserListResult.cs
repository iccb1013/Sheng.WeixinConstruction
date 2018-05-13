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
    public class WeixinGetUserListResult
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "data")]
        public GetUserListResult_Data Data { get; set; }

        /// <summary>
        /// 微信接口永远会返回 NextOpenId，哪怕数据只有几条不需要分次获取
        /// NextOpenId 就是 Data 中 OpenId 列表的最后一个
        /// </summary>
        [DataMember(Name = "next_openid")]
        public string NextOpenId { get; set; }

    }

    [DataContract]
    public class GetUserListResult_Data
    {
        [DataMember(Name = "openid")]
        public string[] OpenIdList { get; set; }
    }

}
