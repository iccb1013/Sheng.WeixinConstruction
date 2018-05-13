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
    /// <summary>
    /// 添加永久素材  图文
    /// </summary>
    [DataContract]
    public class WeixinAddArticleMaterialArgs
    {
        [DataMember(Name = "articles")]
        public List<WeixinArticleMaterial> ArticleList
        {
            get;
            set;
        }
    }
}
