using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract.PayApi
{
    [Table("PayOrderCoupon")]
    public class WeixinPayNotify_Coupon
    {
        public Guid PayOrderId
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
