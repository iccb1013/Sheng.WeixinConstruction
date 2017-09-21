using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class RefereeUplineWrapper
    {
        public Guid MemberId
        {
            get;
            set;
        }

        public Guid RefereeMemberId
        {
            get;
            set;
        }

        public bool Attention
        {
            get;
            set;
        }

        public int Level
        {
            get;
            set;
        }

        /// <summary>
        /// 上线
        /// </summary>
        [NotMapped]
        public RefereeUplineWrapper Upline
        {
            get;
            set;
        }
    }
}
