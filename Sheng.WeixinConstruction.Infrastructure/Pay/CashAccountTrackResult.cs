using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    /// <summary>
    /// 现金帐户操作结果
    /// </summary>
    public class CashAccountTrackResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// 0表示成功，其它为失败时的原因代码
        /// </summary>
        public int Reason
        {
            get;
            set;
        }

        /// <summary>
        /// 操作后余额 单位分
        /// </summary>
        public int LeftCashAccount
        {
            get;
            set;
        }
    }

}
