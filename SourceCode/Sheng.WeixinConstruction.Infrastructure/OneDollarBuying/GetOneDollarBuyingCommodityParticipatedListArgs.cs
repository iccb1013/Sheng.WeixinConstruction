using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetOneDollarBuyingCommodityParticipatedListArgs : GetItemListArgs
    {
        public Guid Member
        {
            get;
            set;
        }

        /// <summary>
        /// 是否只取幸运记录
        /// </summary>
        public bool Lucky
        {
            get;
            set;
        }
    }
}