using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetLotteryPeriodLogSignListArgs : GetItemListArgs
    {
        public Guid PeriodId
        {
            get;
            set;
        }

        /// <summary>
        /// 是否只返回中奖者
        /// </summary>
        public bool? Winner
        {
            get;
            set;
        }
    }

}
