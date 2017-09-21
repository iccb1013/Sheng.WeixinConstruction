using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetCampaign_PictureVoteListArgs : GetItemListArgs
    {
        public EnumCampaignStatus Status
        {
            get;
            set;
        }
    }
}
