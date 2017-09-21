using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetCampaignCountResult
    {
        public int PreparatoryCount
        {
            get;
            set;
        }

        public int OngoingCount
        {
            get;
            set;
        }

        public int EndCount
        {
            get;
            set;
        }
    }
}
