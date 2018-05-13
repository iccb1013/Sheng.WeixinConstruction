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
    public class WeixinGetGroupListResult
    {
        private List<WeixinGetGroupListResult_Group> _groupList = new List<WeixinGetGroupListResult_Group>();
        [DataMember(Name = "groups")]
        public List<WeixinGetGroupListResult_Group> GroupList
        {
            get { return _groupList; }
            set { _groupList = value; }
        }
    }

    [DataContract]
    public class WeixinGetGroupListResult_Group
    {
        [DataMember(Name = "id")]
        public int Id
        {
            get;
            set;
        }

        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }

        [DataMember(Name = "count")]
        public int Count
        {
            get;
            set;
        }
    }
}
