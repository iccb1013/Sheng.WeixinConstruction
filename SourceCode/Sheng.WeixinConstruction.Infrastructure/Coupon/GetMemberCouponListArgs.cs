using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetMemberCouponListArgs : GetItemListArgs
    {
        public Guid MemberId
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
