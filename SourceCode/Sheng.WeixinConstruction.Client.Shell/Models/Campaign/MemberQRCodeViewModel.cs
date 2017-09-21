using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class MemberQRCodeViewModel
    {
        public CampaignEntity Campaign
        {
            get;
            set;
        }

        public Campaign_MemberQRCodeDataReport DataReport
        {
            get;
            set;
        }

        public WeixinJsApiConfig JsApiConfig
        {
            get;
            set;
        }
    }
}