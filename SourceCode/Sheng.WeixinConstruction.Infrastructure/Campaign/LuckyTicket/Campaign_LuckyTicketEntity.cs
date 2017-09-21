using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("Campaign_LuckyTicket")]
    public class Campaign_LuckyTicketEntity
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

        /// <summary>
        /// 最后一次开奖时间
        /// </summary>
        public DateTime? LastDrawTime
        {
            get;
            set;
        }
    }
}
