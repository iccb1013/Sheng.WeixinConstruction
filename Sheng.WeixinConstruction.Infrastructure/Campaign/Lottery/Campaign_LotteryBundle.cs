using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class Campaign_LotteryBundle
    {
        public CampaignEntity Campaign
        {
            get;
            set;
        }

        public Campaign_LotteryEntity Lottery
        {
            get;
            set;
        }

        public bool Empty
        {
            get
            {
                if (Campaign == null || Lottery == null)
                    return true;
                else
                    return false;
            }
        }
    }
}
