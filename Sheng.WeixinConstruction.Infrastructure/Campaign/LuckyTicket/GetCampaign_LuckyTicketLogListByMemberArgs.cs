using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetCampaign_LuckyTicketLogListByMemberArgs : GetItemListArgs
    {
        public Guid CampaignId
        {
            get;
            set;
        }

        public Guid MemberId
        {
            get;
            set;
        }
    }

}
