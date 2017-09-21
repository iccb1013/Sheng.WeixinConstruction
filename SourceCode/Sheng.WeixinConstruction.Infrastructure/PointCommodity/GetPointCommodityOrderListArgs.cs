using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetPointCommodityOrderListArgs : GetItemListArgs
    {
        public string OrderNumber
        {
            get;
            set;
        }

        public string MemberNickName
        {
            get;
            set;
        }

        public int Status
        {
            get;
            set;
        }
    }
}
