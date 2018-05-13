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
    public class WeixinSetUserListGroupArgs
    {
        private List<string> _openIdList = new List<string>();
        [DataMember(Name = "openid_list")]
        public List<string> OpenIdList
        {
            get { return _openIdList; }
            set { _openIdList = value; }
        }

        [DataMember(Name = "to_groupid")]
        public int GroupId
        {
            get;
            set;
        }
    }
}
