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
    [Table("AutoReplyOnKeyWordsContent")]
    public class AutoReplyOnKeyWordsContentEntity : IAutoReply
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

        public Guid RuleId
        {
            get;
            set;
        }

        public EnumAutoReplyType Type
        {
            get;
            set;
        }

        /// <summary>
        /// 如果是文本回复，这里是内容
        /// 如果是素材内容， MediaId不在这里存储
        /// </summary>
        public string Content
        {
            get;
            set;
        }


        //图片素材时，下面存储其信息

        /// <summary>
        /// 图片URL
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// 图片名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 图片的mediaId
        /// </summary>
        public string MediaId
        {
            get;
            set;
        }

        //图文素材时，下面存储其信息

        public Guid ArticleId
        {
            get;
            set;
        }

        [NotMapped]
        public Guid? MaterialId
        {
            get;
            set;
        }
    }
}
