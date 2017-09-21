using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class Campaign_PictureVoteDataReport
    {
        /// <summary>
        /// 审核通过的项目数量
        /// </summary>
        public int ApprovedItemsCount
        {
            get;
            set;
        }

        /// <summary>
        /// 总投票数量
        /// </summary>
        public int VoteQuantityCount
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
