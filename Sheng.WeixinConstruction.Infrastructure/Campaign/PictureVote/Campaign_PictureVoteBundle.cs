using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class Campaign_PictureVoteBundle
    {
        public CampaignEntity Campaign
        {
            get;
            set;
        }

        public Campaign_PictureVoteEntity PictureVote
        {
            get;
            set;
        }
    }
}
