using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class Campaign_PictureVoteItemRejectedArgs
    {
        public Guid CampaignId
        {
            get;
            set;
        }

        public Guid? MemberId
        {
            get;
            set;
        }

        public Guid ItemId
        {
            get;
            set;
        }

        //public EnumCampaignPictureVoteItemApproveStatus ApproveStatus
        //{
        //    get;
        //    set;
        //}

        public string Message
        {
            get;
            set;
        }
    }
}
