using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class Campaign_LuckyTicketBundle
    {
        public CampaignEntity Campaign
        {
            get;
            set;
        }

        public Campaign_LuckyTicketEntity LuckyTicket
        {
            get;
            set;
        }

        public bool Empty
        {
            get
            {
                if (Campaign == null || LuckyTicket == null)
                    return true;
                else
                    return false;
            }
        }
    }
}
