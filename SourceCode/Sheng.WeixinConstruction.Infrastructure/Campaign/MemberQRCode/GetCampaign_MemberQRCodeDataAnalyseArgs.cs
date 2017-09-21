using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetCampaign_MemberQRCodeDataAnalyseArgs
    {
        public Guid CampaignId
        {
            get;
            set;
        }

        public DateTime StartDate
        {
            get;
            set;
        }

        public DateTime EndDate
        {
            get;
            set;
        }
    }

    public class GetMemberQRCodeDataAnalyseResult
    {
        /// <summary>
        /// 日生成量
        /// </summary>
        public DataTable DayCreate
        {
            get;
            set;
        }

        /// <summary>
        /// 日落地总量
        /// </summary>
        public DataTable DayLanding
        {
            get;
            set;
        }

        /// <summary>
        /// 日落地人数
        /// </summary>
        public DataTable DayLandingPerson
        {
            get;
            set;
        }

        /// <summary>
        /// 总落地次数
        /// </summary>
        public int LandingCount
        {
            get;
            set;
        }

        /// <summary>
        /// 总落地人数
        /// </summary>
        public int LandingPersonCount
        {
            get;
            set;
        }

    }
}
