using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class Campaign_ShakingLotteryBundle
    {
        public CampaignEntity Campaign
        {
            get;
            set;
        }

        public Campaign_ShakingLotteryEntity ShakingLottery
        {
            get;
            set;
        }

        public bool Empty
        {
            get
            {
                if (Campaign == null || ShakingLottery == null)
                    return true;
                else
                    return false;
            }
        }
    }
}
