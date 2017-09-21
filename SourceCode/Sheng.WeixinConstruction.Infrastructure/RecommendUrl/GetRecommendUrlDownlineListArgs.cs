using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetRecommendDownlineListArgs : GetItemListArgs
    {

        public Guid MemberId
        {
            get;
            set;
        }

    }
}
