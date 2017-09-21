using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    /// <summary>
    /// 购物车
    /// </summary>
    [Table("PointCommodityShoppingCart")]
    public class PointCommodityShoppingCartEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

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
