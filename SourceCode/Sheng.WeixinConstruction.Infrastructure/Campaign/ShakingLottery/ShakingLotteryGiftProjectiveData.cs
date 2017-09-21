using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class ShakingLotteryGiftProjectiveData
    {
        public Campaign_ShakingLotteryPeriodEntity CurrentPeriod
        {
            get;
            set;
        }

        public GetItemListResult WinningList
        {
            get;
            set;
        }
    }
}
