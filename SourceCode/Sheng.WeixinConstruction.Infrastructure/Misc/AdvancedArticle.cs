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
    [Table("AdvancedArticle")]
    public class AdvancedArticleEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Guid Domain
        {
            get;
            set;
        }

        public string AppId
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public Guid? Advertising
        {
            get;
            set;
        }

        public string ShareImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 分享到朋友圈标题
        /// </summary>
        public string ShareTimelineTitle
        {
            get;
            set;
        }

        /// <summary>
        /// 分享给好友标题
        /// </summary>
        public string ShareAppMessageTitle
        {
            get;
            set;
        }

        /// <summary>
        /// 分享给好友描述
        /// </summary>
        public string ShareAppMessageDescription
        {
            get;
            set;
        }

        /// <summary>
        /// 分享活动任意页面到朋友圈可得积分
        /// </summary>
        public int ShareTimelinePoint
        {
            get;
            set;
        }

        /// <summary>
        /// 分享活动任意页面给朋友可得积分
        /// </summary>
        public int ShareAppMessagePoint
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public DateTime UpdateTime
        {
            get;
            set;
        }

        public Guid CreateUser
        {
            get;
            set;
        }

        /// <summary>
        /// 活动页面PV
        /// </summary>
        public int PageVisitCount
        {
            get;
            set;
        }
    }
}
