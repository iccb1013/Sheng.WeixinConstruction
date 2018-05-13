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


using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("RecommendUrlSettings")]
    public class RecommendUrlSettingsEntity
    {

        [Key]
        public Guid Domain
        {
            get;
            set;
        }

        [Key]
        public string AppId
        {
            get;
            set;
        }

        /// <summary>
        /// 自定义落地页
        /// </summary>
        public string LandingUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 引流关注可获得的积分/人
        /// </summary>
        public int AttentionPoint
        {
            get;
            set;
        }

        /// <summary>
        /// 二级引流关注可获得的积分/人
        /// </summary>
        public int Level2AttentionPoint
        {
            get;
            set;
        }
    }
}
