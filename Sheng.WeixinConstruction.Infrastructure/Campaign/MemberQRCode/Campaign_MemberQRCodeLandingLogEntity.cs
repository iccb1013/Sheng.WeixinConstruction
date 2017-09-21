using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("Campaign_MemberQRCodeLandingLog")]
    public class Campaign_MemberQRCodeLandingLogEntity
    {
        [Key]
        public Guid CampaignId
        {
            get;
            set;
        }

        public Guid Domain
        {
            get;
            set;
        }

        public Guid QRCodeOwnMember
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
