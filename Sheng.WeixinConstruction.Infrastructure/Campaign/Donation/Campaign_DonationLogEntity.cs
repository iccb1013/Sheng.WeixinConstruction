using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("Campaign_DonationLog")]
    public class Campaign_DonationLogEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

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

        public Guid PayOrder
        {
            get;
            set;
        }

        public Guid Member
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Contact
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }
    }
}
