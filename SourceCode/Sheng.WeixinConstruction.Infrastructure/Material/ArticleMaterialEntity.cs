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

namespace Sheng.WeixinConstruction.Infrastructure
{
    [DataContract]
    [Table("ArticleMaterial")]
    public class ArticleMaterialEntity
    {
        private Guid _id = Guid.NewGuid();
        [DataMember(Name = "id")]
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

        [DataMember(Name = "mediaId")]
        public string MediaId
        {
            get;
            set;
        }

        /// <summary>
        /// 素材名
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }

        private DateTime _createTime = DateTime.Now;
        [OrderBy(OrderBy.DESC)]
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }

        public Guid OperatorUser
        {
            get;
            set;
        }

        /// <summary>
        /// 与微信后台的同步状态，0,未提交微信后台，1已提交待取回URL，2已取回URL，完成
        /// </summary>
        [DataMember(Name = "weixinStatus")]
        public int WeixinStatus
        {
            get;
            set;
        }

        [DataMember(Name = "articles")]
        [NotMapped]
        public List<ArticleMaterialItemEntity> ArticleList
        {
            get;
            set;
        }
    }
}
