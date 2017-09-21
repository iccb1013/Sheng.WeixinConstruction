using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetOneDollarBuyingCommodityMemberPartNumberArgs : GetItemListArgs
    {
        public Guid SaleId
        {
            get;
            set;
        }
    }
}
