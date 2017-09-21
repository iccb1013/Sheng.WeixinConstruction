using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetCampaign_PictureVoteDataAnalyseArgs
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

    public class GetPictureVoteDataAnalyseResult
    {
        /// <summary>
        /// 日上传量
        /// </summary>
        public DataTable DayUpload
        {
            get;
            set;
        }

        /// <summary>
        /// 日投票量
        /// </summary>
        public DataTable DayVote
        {
            get;
            set;
        }

        public int WaitingCount
        {
            get;
            set;
        }

        public int ApprovedCount
        {
            get;
            set;
        }

        public int RejectedCount
        {
            get;
            set;
        }
    }
}
