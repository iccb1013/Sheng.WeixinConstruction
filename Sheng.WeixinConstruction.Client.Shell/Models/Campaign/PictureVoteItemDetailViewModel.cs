using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class PictureVoteItemDetailViewModel
    {
        public CampaignEntity Campaign
        {
            get;
            set;
        }

        public Campaign_PictureVoteItemEntity PictureVoteItem
        {
            get;
            set;
        }

        public WeixinJsApiConfig JsApiConfig
        {
            get;
            set;
        }

        public string ShareUrl
        {
            get;
            set;
        }
    }
}