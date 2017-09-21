using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("Campaign_ShakingLotteryLog")]
    public class Campaign_ShakingLotteryLogEntity
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

        public string AppId
        {
            get;
            set;
        }

        public Guid? Period
        {
            get;
            set;
        }

        public Guid Member
        {
            get;
            set;
        }

        public DateTime Time
        {
            get;
            set;
        }

        public bool Win
        {
            get;
            set;
        }

        public Guid? Gift
        {
            get;
            set;
        }
    }
}
