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
    public class WeixinSetUserGroupArgs
    {
        [DataMember(Name = "openid")]
        public string OpenId
        {
            get;
            set;
        }

        [DataMember(Name = "to_groupid")]
        public int GroupId
        {
            get;
            set;
        }
    }
}
