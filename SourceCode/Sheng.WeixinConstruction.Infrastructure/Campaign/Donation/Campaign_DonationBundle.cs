using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class Campaign_DonationBundle
    {
        public CampaignEntity Campaign
        {
            get;
            set;
        }

        public Campaign_DonationEntity Donation
        {
            get;
            set;
        }

        public bool Empty
        {
            get
            {
                if (Campaign == null || Donation == null)
                    return true;
                else
                    return false;
            }
        }
    }
}
