using Linkup.Common;
using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("PointCommodityOrderLog")]
    public class PointCommodityOrderLogEntity
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

        public Guid Order
        {
            get;
            set;
        }

        [OrderBy(OrderBy.DESC)]
        public DateTime Time
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }
    }
}
