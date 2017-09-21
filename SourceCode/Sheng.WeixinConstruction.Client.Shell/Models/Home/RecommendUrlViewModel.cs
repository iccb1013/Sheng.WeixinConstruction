using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class RecommendUrlViewModel
    {
        public RecommendUrlEntity RecommendUrl
        {
            get;
            set;
        }

        public RecommendUrlSettingsEntity Settings
        {
            get;
            set;
        }

        public int Level1DownlineCount
        {
            get;
            set;
        }

        public int Level2DownlineCount
        {
            get;
            set;
        }
    }
}