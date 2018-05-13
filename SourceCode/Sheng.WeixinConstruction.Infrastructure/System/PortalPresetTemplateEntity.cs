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

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("PortalPresetTemplate")]
    public class PortalPresetTemplateEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 效果图URL
        /// </summary>
        public string PreviewImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 默认的背景图片地址
        /// </summary>
        public string BackgroundImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 模版代码
        /// </summary>
        public string Template
        {
            get;
            set;
        }

        /// <summary>
        /// 用于预览的HTML
        /// </summary>
        public string PreviewHtml
        {
            get;
            set;
        }

        /// <summary>
        /// 是否仅供付费帐户使用
        /// </summary>
        public bool PayRequired
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdateTime
        {
            get;
            set;
        }
    }
}
