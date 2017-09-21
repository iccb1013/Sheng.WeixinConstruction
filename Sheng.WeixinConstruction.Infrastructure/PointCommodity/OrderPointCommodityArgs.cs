using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class OrderPointCommodityArgs
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid Id
        {
            get;
            set;
        }

        public Guid MemberId
        {
            get;
            set;
        }
    }
}
