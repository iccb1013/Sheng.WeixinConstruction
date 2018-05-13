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
    [Table("PointCommodityOrder")]
    public class PointCommodityOrderEntity
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

        public string OrderNumber
        {
            get;
            set;
        }

        public Guid Member
        {
            get;
            set;
        }

        //public Guid PointCommodity
        //{
        //    get;
        //    set;
        //}

        //public string PointCommodityName
        //{
        //    get;
        //    set;
        //}

        //public int PointCommodityPrice
        //{
        //    get;
        //    set;
        //}

        //public string PointCommodityImageUrl
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 所需积分
        /// </summary>
        public int Point
        {
            get;
            set;
        }

        /// <summary>
        /// 所需现金 金额 分
        /// </summary>
        public int Price
        {
            get;
            set;
        }

        [OrderBy(OrderBy = OrderBy.DESC)]
        public DateTime OrderTime
        {
            get;
            set;
        }

        /// <summary>
        /// 待支付时的过期时间，达到此时间订单自动释放
        /// </summary>
        public DateTime? ExpireTime
        {
            get;
            set;
        }

        public DateTime? DealTime
        {
            get;
            set;
        }

        public PointCommodityOrderStatus Status
        {
            get;
            set;
        }

        [NotMapped]
        public string StatusString
        {
            get { return EnumHelper.GetEnumMemberValue(Status); }
            set { Status = EnumHelper.GetEnumFieldByMemberValue<PointCommodityOrderStatus>(value); }
        }

        /// <summary>
        /// 生成的微信支付订单Id
        /// </summary>
        public Guid? PayOrder
        {
            get;
            set;
        }

        public Guid? OperatorUser
        {
            get;
            set;
        }


    }
}
