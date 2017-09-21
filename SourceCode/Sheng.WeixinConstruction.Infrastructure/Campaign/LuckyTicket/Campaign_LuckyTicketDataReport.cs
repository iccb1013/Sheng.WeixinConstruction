using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class Campaign_LuckyTicketDataReport
    {
        /// <summary>
        /// 参与人数
        /// </summary>
        public int MemberCount
        {
            get;
            set;
        }

        /// <summary>
        /// 总号码数量
        /// </summary>
        public int LuckyTicketCount
        {
            get;
            set;
        }

        /// <summary>
        /// 活动页面PV
        /// </summary>
        public int PageVisitCount
        {
            get;
            set;
        }
    }
}
