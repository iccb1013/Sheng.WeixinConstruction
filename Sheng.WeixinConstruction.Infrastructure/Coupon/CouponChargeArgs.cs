using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class CouponChargeArgs
    {
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

        public Guid CouponRecordId
        {
            get;
            set;
        }

        public Guid ChargeUser
        {
            get;
            set;
        }

        public string ChargeIP
        {
            get;
            set;
        }
    }
}
