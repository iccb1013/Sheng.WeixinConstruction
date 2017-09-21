using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class LuckyTicketDrawArgs
    {
        public Guid DomainId
        {
            get;
            set;
        }

        public Guid CampaignId
        {
            get;
            set;
        }

        /// <summary>
        /// 抽取数量
        /// </summary>
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// 中奖说明
        /// </summary>
        public string WinRemark
        {
            get;
            set;
        }
    }
}
