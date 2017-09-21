using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    /// <summary>
    /// 抽奖模式
    /// </summary>
    public enum EnumCampaign_ShakingLotteryMode
    {
        /// <summary>
        /// 只可参与一次
        /// </summary>
        Once = 0,
        /// <summary>
        /// 可重复参与
        /// </summary>
        Repeatable = 1,
        /// <summary>
        /// 周期模式
        /// </summary>
        Period = 2
    }
}
