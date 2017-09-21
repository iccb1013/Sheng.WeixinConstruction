using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Container.Models
{
    public class ThirdPartyAuthMonitorViewModel
    {
        public string AccessToken
        {
            get;
            set;
        }

        public DateTime? AccessTokenExpiryTime
        {
            get;
            set;
        }

        public List<AuthorizerAccessTokenWrapper> AuthorizerAccessTokenList
        {
            get;
            set;
        }

        public List<AuthorizerJsApiTicketWrapper> AuthorizerJsApiTicketList
        {
            get;
            set;
        }
    }
}