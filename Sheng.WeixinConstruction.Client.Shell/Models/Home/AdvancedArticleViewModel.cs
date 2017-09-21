using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class AdvancedArticleViewModel
    {
        public AdvancedArticleEntity Article
        {
            get;
            set;
        }

        public AdvertisingEntity Advertising
        {
            get;
            set;
        }
    }
}