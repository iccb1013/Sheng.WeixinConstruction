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
    public class WeixinAddMaterialResult
    {
        [DataMember(Name = "media_id")]
        public string MediaId
        {
            get;
            set;
        }

        [DataMember(Name = "url")]
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// 自用，用于从文件服务器返回文件名
        /// </summary>
        [DataMember(Name = "fileName")]
        public string FileName
        {
            get;
            set;
        }
    }
}
