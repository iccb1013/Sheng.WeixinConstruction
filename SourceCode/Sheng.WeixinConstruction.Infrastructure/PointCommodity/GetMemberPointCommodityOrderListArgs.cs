using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetMemberPointCommodityOrderListArgs : GetItemListArgs
    {
       
        
        public Guid Member
        {
            get;
            set;
        }

    }
}
