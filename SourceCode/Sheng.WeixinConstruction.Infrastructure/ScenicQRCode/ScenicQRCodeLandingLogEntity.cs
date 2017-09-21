using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("ScenicQRCodeLandingLog")]
    public class ScenicQRCodeLandingLogEntity
    {
        public Guid QRCodeId
        {
            get;
            set;
        }

        public Guid Domain
        {
            get;
            set;
        }

        public string VisitorOpenId
        {
            get;
            set;
        }

        public DateTime LandingTime
        {
            get;
            set;
        }
    }
}
