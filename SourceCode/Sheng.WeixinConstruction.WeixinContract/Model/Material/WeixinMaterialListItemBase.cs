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
    //http://mp.weixin.qq.com/wiki/6/0c3186cb09caaa2b3bc614972bb9bfde.html
    //新增素材时，使用的JSON结构有所不同
    /// <summary>
    /// 获取素材列表时，使用的结构
    /// </summary>
    [DataContract]
    public class WeixinMaterialListItemBase
    {
        [DataMember(Name = "media_id")]
        public string MediaId
        {
            get;
            set;
        }

        /// <summary>
        /// 这篇图文消息素材的最后更新时间
        /// </summary>
        [DataMember(Name = "update_time")]
        public int Update_time
        {
            get;
            set;
        }

        public DateTime UpdateTime
        {
            get
            {
                return WeixinApiHelper.ConvertIntToDateTime(Update_time);
            }
        }

    }
}
