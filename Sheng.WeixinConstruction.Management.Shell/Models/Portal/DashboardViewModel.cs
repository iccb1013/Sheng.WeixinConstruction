using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Management.Shell.Models
{
    public class DashboardViewModel
    {
        public MemberStatisticData MemberStatisticData
        {
            get;
            set;
        }

        public PointCommodityOrderStatisticData PointCommodityOrderStatisticData
        {
            get;
            set;
        }
    }
}