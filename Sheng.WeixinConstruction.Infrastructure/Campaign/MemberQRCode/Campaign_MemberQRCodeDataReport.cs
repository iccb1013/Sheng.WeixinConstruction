using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class Campaign_MemberQRCodeDataReport
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
        /// 扫码次数
        /// </summary>
        public int LandingCount
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
