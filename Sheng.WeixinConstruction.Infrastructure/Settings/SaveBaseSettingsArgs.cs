using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class SaveBaseSettingsArgs
    {
        public string Name
        {
            get;
            set;
        }

        public string PortalImageUrl
        {
            get;
            set;
        }

        public string GuideSubscribeUrl
        {
            get;
            set;
        }

        public int InitialMemberPoint
        {
            get;
            set;
        }

        public int SignInPoint
        {
            get;
            set;
        }

        public string PointCommodityGetWay
        {
            get;
            set;
        }
    }
}
