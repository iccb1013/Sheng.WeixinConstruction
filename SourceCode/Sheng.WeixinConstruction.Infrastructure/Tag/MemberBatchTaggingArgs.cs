using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class MemberBatchTaggingArgs
    {
        public string[] OpenIdList
        {
            get;
            set;
        }

        public int TagId
        {
            get;
            set;
        }
    }
}
