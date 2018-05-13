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
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class AutoReply : IAutoReply
    {
        [Key]
        public Guid Domain
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

        public string Url
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string MediaId
        {
            get;
            set;
        }

        //只因要实现 IAutoReply 接口，此处无用，用于 AutoReplyOnKeyWordsContentEntity
        //因为微信后台的 被添加自动回复 和 消息自动回复 都不支持图文消息
        [NotMapped]
        public Guid ArticleId
        {
            get;
            set;
        }

        public Guid? MaterialId
        {
            get;
            set;
        }
    }
}
