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

namespace Sheng.WeixinConstruction.WeixinContract.PayApi
{
    /// <summary>
    /// 订单支付时使用的代金券或立减优惠
    /// </summary>
    [Table("PayOrderCoupon")]
    public class WeixinPayOrderQueryResult_Coupon
    {
        public Guid PayOrderId
        {
            get;
            set;
        }

        /// <summary>
        /// 代金券或立减优惠批次ID
        /// coupon_batch_id_$n
        /// </summary>
        public string CouponBatchId
        {
            get;
            set;
        }

        /// <summary>
        /// 代金券类型
        /// CASH--充值代金券 
        /// NO_CASH---非充值代金券
        /// 订单使用代金券时有返回（取值：CASH、NO_CASH）。$n为下标,从0开始编号，举例：coupon_type_$0
        /// coupon_type_$n
        /// </summary>
        public string CouponType
        {
            get;
            set;
        }

        /// <summary>
        /// 代金券或立减优惠ID
        /// coupon_id_$n
        /// </summary>
        public string CouponId
        {
            get;
            set;
        }

        /// <summary>
        /// 单个代金券或立减优惠支付金额
        /// coupon_fee_$n
        /// </summary>
        public int CouponFee
        {
            get;
            set;
        }

    }
}
