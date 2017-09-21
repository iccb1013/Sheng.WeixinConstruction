using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class Campaign_ShakingLotteryDataReport
    {
        /// <summary>
        /// 总参与人次数
        /// </summary>
        public int MemberCount
        {
            get;
            set;
        }

        /// <summary>
        /// 总中奖人数
        /// </summary>
        public int LuckyMemberCount
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
