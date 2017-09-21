using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetDistributedCouponListArgs : GetItemListArgs
    {
        public Guid CouponId
        {
            get;
            set;
        }

        public string SerialNumber
        {
            get;
            set;
        }

        public string MemberNickName
        {
            get;
            set;
        }

        public EnumCouponStatus? Status
        {
            get;
            set;
        }
    }
}
