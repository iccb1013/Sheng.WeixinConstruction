using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("Campaign_Lottery")]
    public class Campaign_LotteryEntity
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

        public int Point
        {
            get;
            set;
        }

        public EnumCampaign_LotteryMode Mode
        {
            get;
            set;
        }

        public int DrawCount
        {
            get;
            set;
        }

    }
}
