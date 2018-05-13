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
    [Table("CouponRecord")]
    public class CouponRecordEntity
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

        public Guid Coupon
        {
            get;
            set;
        }

        public Guid Member
        {
            get;
            set;
        }

        /// <summary>
        /// 流水号，方便不能扫码的手机直接查询
        /// </summary>
        public string SerialNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 二维码图片url
        /// </summary>
        public string QRCodeImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 分发人
        /// </summary>
        public Guid DistributeUser
        {
            get;
            set;
        }

        public DateTime DistributeTime
        {
            get;
            set;
        }

        /// <summary>
        /// 有效期，在此时间之间使用
        /// </summary>
        public DateTime? LimitedTime
        {
            get;
            set;
        }

        /// <summary>
        /// 核销时间
        /// </summary>
        public DateTime? ChargeTime
        {
            get;
            set;
        }

        public Guid ChargeUser
        {
            get;
            set;
        }

        public EnumCouponStatus Status
        {
            get;
            set;
        }
    }
}
