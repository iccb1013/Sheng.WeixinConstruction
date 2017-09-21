using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Management.Shell.Models
{
    public class PointCommodityOrderDetailViewModel
    {
        public PointCommodityOrderEntity Order
        {
            get;
            set;
        }

        public MemberEntity Member
        {
            get;
            set;
        }

        public List<PointCommodityOrderItemEntity> ItemList
        {
            get;
            set;
        }

        public List<PointCommodityOrderLogEntity> LogList
        {
            get;
            set;
        }
    }
}