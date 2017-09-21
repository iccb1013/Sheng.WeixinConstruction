using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetCampaign_DonationLogListArgs : GetItemListArgs
    {
        public Guid CampaignId
        {
            get;
            set;
        }

        public Guid? Member
        {
            get;
            set;
        }

        public bool Finished
        {
            get;
            set;
        }
    }

}
