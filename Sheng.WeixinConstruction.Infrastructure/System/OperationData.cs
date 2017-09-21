using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    /// <summary>
    /// 运营数据
    /// </summary>
    public class OperationData
    {
        /// <summary>
        /// 新增人数
        /// </summary>
        public int NewAttentionCount
        {
            get;
            set;
        }

        /// <summary>
        /// 总关注人数
        /// </summary>
        public int TotalAttentionCount
        {
            get;
            set;
        }
    }
}
