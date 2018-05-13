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
    [Table("MovieSettings")]
    public class MovieSettingsEntity
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

        public string MTimeUrl
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

        public DateTime? SyncTime
        {
            get;
            set;
        }
    }
}
