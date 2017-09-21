using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class DonationViewModel
    {
        public Campaign_DonationBundle CampaignBundle
        {
            get;
            set;
        }

        //public Campaign_DonationDataReport DataReport
        //{
        //    get;
        //    set;
        //}

       

        public WeixinJsApiConfig JsApiConfig
        {
            get;
            set;
        }
    }
}