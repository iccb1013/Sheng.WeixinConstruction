using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("Campaign_LuckyTicketLog")]
    public class Campaign_LuckyTicketLogEntity
    {
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

        public Guid Member
        {
            get;
            set;
        }

        public string TicketNumber
        {
            get;
            set;
        }

        public string FromOpenId
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public bool Win
        {
            get;
            set;
        }

        public DateTime? WinTime
        {
            get;
            set;
        }

        public string WinRemark
        {
            get;
            set;
        }
    }
}
