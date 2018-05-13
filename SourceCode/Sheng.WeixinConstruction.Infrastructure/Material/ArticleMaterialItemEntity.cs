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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [DataContract]
    [Table("ArticleMaterialItem")]
    public class ArticleMaterialItemEntity : WeixinArticleMaterial
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

        [DataMember(Name = "articleMaterial")]
        public Guid ArticleMaterial
        {
            get;
            set;
        }

        /// <summary>
        /// 封面图片的本地URL
        /// </summary>
        [DataMember(Name = "thumbUrl")]
        public string ThumbUrl
        {
            get;
            set;
        }

        [DataMember(Name = "thumbName")]
        public string ThumbName
        {
            get;
            set;
        }

        [DataMember(Name = "index")]
        public int Index
        {
            get;
            set;
        }

        [NotMapped]
        [DataMember(Name = "imgMappingList")]
        public List<MaterialImgMapping> ImgMappingList
        {
            get;
            set;
        }

        /// <summary>
        /// 图片映射，本地图片地址和微信图片地址，JSON字符串
        /// </summary>
        public string ImgMapping
        {
            get;
            set;
        }

        ///// <summary>
        ///// 微信接口在修改永久图文素材时，需要提供所在index
        ///// </summary>
        //[DataMember(Name = "oldIndex")]
        //public int OldIndex
        //{
        //    get;
        //    set;
        //}
    }
}
