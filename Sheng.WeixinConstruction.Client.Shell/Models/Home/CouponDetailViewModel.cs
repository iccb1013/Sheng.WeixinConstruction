using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class CouponDetailViewModel
    {
        public CouponEntity Coupon
        {
            get;
            set;
        }

        public CouponRecordEntity Record
        {
            get;
            set;
        }
    }
}