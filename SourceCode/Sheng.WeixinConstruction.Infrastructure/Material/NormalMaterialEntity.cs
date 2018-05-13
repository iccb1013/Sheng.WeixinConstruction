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


using Linkup.Common;
using Linkup.DataRelationalMapping;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("NormalMaterial")]
    public class NormalMaterialEntity
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

        public string MediaId
        {
            get;
            set;
        }

        public string WeixinUrl
        {
            get;
            set;
        }

        [NotMapped]
        public MaterialType Type
        {
            get;
            set;
        }

        [Column("Type")]
        public string TypeString
        {
            get { return EnumHelper.GetEnumMemberValue(Type); }
            set { Type = EnumHelper.GetEnumFieldByMemberValue<MaterialType>(value); }
        }

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
    }
}
