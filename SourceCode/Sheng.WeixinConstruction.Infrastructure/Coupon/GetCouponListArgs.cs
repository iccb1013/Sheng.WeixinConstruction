using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetCouponListArgs : GetItemListArgs
    {
        public string Name
        {
            get;
            set;
        }
    }
}
