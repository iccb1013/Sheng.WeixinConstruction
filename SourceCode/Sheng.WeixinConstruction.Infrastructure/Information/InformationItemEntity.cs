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
    [Table("InformationItem")]
    public class InformationItemEntity
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

        public Guid Information
        {
            get;
            set;
        }

        public Guid Category
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Image
        {
            get;
            set;
        }

        [Json]
        public List<ImageWrapper> ImageList
        {
            get;
            set;
        }

        /// <summary>
        /// 简要说明
        /// </summary>
        public string Introduction
        {
            get;
            set;
        }

        /// <summary>
        /// 详细说明
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        public string PhoneNumber
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public Guid CreateUser
        {
            get;
            set;
        }

    }
}
