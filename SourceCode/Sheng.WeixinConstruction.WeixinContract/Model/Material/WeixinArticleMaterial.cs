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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    /*
     * 获取素材列表时 和 新增文章素材时的JSON结构是有差异的
     * 取列表时用WeixinArticleMaterial包起来，增加时WeixinArticleMaterialContentItem直接放到数组中
     */

    [DataContract]
    public class WeixinArticleMaterialListItem : WeixinMaterialListItemBase
    {
        [DataMember(Name = "content")]
        public WeixinArticleMaterialListItemContent Content
        {
            get;
            set;
        }
    }

    [DataContract]
    public class WeixinArticleMaterialListItemContent
    {
        [DataMember(Name = "news_item")]
        public List<WeixinArticleMaterial> ItemList
        {
            get;
            set;
        }
    }

    [DataContract]
    public class WeixinArticleMaterial
    {
        /// <summary>
        /// 图文消息的标题
        /// </summary>
        [DataMember(Name = "title")]
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// 图文消息的封面图片素材id（必须是永久mediaID）
        /// </summary>
        [DataMember(Name = "thumb_media_id")]
        public string ThumbMediaId
        {
            get;
            set;
        }

        /// <summary>
        /// 是否显示封面，0为false，即不显示，1为true，即显示
        /// </summary>
        [DataMember(Name = "show_cover_pic")]
        public int ShowCoverPic
        {
            get;
            set;
        }

        /// <summary>
        /// 作者
        /// </summary>
        [DataMember(Name = "author")]
        public string Author
        {
            get;
            set;
        }

        /// <summary>
        /// 图文消息的摘要，仅有单图文消息才有摘要，多图文此处为空
        /// </summary>
        [DataMember(Name = "digest")]
        public string Digest
        {
            get;
            set;
        }

        /// <summary>
        /// 图文消息的具体内容，支持HTML标签，必须少于2万字符，小于1M，且此处会去除JS
        /// </summary>
        [DataMember(Name = "content")]
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// 图文页的URL
        /// </summary>
        [DataMember(Name = "url")]
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// 图文消息的原文地址，即点击“阅读原文”后的URL
        /// </summary>
        [DataMember(Name = "content_source_url")]
        public string ContentSourceUrl
        {
            get;
            set;
        }
    }
}
