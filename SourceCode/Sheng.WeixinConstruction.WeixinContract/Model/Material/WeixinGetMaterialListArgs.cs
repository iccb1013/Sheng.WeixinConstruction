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


using Linkup.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    [DataContract]
    public class WeixinGetMaterialListArgs
    {

        public MaterialType Type
        {
            get;
            set;
        }

        [DataMember(Name = "type")]
        public string TypeString
        {
            get { return EnumHelper.GetEnumMemberValue(Type); }
            set { Type = EnumHelper.GetEnumFieldByMemberValue<MaterialType>(value); }
        }

        /// <summary>
        /// 从全部素材的该偏移位置开始返回，0表示从第一个素材 返回
        /// </summary>
        [DataMember(Name = "offset")]
        public int Offset
        {
            get;
            set;
        }

        /// <summary>
        /// 返回素材的数量，取值在1到20之间
        /// </summary>
        [DataMember(Name = "count")]
        public int Count
        {
            get;
            set;
        }
    }
}
