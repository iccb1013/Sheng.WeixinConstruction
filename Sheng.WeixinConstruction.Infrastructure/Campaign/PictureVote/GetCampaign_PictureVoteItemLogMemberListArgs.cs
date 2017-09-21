using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetCampaign_PictureVoteItemLogMemberListArgs : GetItemListArgs
    {
        //public Guid CampaignId
        //{
        //    get;
        //    set;
        //}

        public Guid ItemId
        {
            get;
            set;
        }
    }

   
}
