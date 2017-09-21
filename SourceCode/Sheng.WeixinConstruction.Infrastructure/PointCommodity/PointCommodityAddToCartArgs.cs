using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class PointCommodityAddToCartArgs
    {
        public Guid Domain
        {
            get;
            set;
        }

        public string AppId
        {
            get;
            set;
        }

        public Guid Member
        {
            get;
            set;
        }

        public Guid PointCommodity
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }

        public DateTime AddedTime
        {
            get;
            set;
        }

    }
}
