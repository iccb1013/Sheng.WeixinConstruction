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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("MemberPointTrack")]
    public class MemberPointTrackEntity
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

        public Guid Member
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }

        public int Point
        {
            get;
            set;
        }

        [OrderBy(OrderBy = OrderBy.DESC)]
        public DateTime ChangeTime
        {
            get;
            set;
        }

        public MemberPointTrackType Type
        {
            get;
            set;
        }

        [NotMapped]
        public string TypeString
        {
            get { return EnumHelper.GetEnumMemberValue(Type); }
            set { Type = EnumHelper.GetEnumFieldByMemberValue<MemberPointTrackType>(value); }
        }

        public string PointCommodityName
        {
            get;
            set;
        }

        public string PointCommodityOrder
        {
            get;
            set;
        }

        public Guid OperatorUser
        {
            get;
            set;
        }

        public string Remark
        {
            get;
            set;
        }
    }
}
