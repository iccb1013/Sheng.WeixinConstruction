using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class PointCommodityShoppingCartOperateResult
    {
        /// <summary>
        /// 该商品的最新数量
        /// </summary>
        public int Quantity
        {
            get;
            set;
        }

        /// <summary>
        /// 购物车共有多少 种 商品
        /// </summary>
        public int PointCommodityCount
        {
            get;
            set;
        }
    }
}
