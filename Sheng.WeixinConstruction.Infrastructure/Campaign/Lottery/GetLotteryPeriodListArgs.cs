using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetLotteryPeriodListArgs : GetItemListArgs
    {
        public Guid CampaignId
        {
            get;
            set;
        }

        public DateTime? StartTime
        {
            get;
            set;
        }

        public DateTime? EndTime
        {
            get;
            set;
        }

    }

}
