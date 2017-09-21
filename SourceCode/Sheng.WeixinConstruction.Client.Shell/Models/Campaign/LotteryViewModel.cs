using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class LotteryViewModel
    {
        public CampaignEntity Campaign
        {
            get;
            set;
        }

        public Campaign_LotteryEntity Lottery
        {
            get;
            set;
        }

        public Campaign_LotteryDataReport DataReport
        {
            get;
            set;
        }

        public WeixinJsApiConfig JsApiConfig
        {
            get;
            set;
        }

        public bool Attention
        {
            get;
            set;
        }
    }
}