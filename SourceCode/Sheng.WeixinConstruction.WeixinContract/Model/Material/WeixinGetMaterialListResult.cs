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
    public class WeixinGetMaterialListResult<T>
    {
        /// <summary>
        /// 该类型的素材的总数
        /// </summary>
        [DataMember(Name = "total_count")]
        public string TotalCount
        {
            get;
            set;
        }

        /// <summary>
        /// 本次调用获取的素材的数量
        /// </summary>
        [DataMember(Name = "item_count")]
        public string ItemCount
        {
            get;
            set;
        }

        [DataMember(Name = "item")]
        public List<T> ItemList
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 永久图文消息素材列表的响应
    /// </summary>
    [DataContract]
    public class WeixinGetArticleMaterialListResult : WeixinGetMaterialListResult<WeixinArticleMaterialListItem>
    {

    }

    /// <summary>
    /// 其他类型（图片、语音、视频）的返回
    /// </summary>
    [DataContract]
    public class WeixinGetNormalMaterialListResult : WeixinGetMaterialListResult<WeixinNormalMaterial>
    {

    }
}
