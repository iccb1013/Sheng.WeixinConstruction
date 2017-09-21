using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetCampaign_LuckyTicketLogListArgs : GetItemListArgs
    {
        public Guid CampaignId
        {
            get;
            set;
        }

        public string NickName
        {
            get;
            set;
        }

        public string MobilePhone
        {
            get;
            set;
        }
    }

}
