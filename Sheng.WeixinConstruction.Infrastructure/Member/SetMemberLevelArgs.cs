using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class SetMemberLevelArgs
    {
        public Guid Domain
        {
            get;
            set;
        }

        public string AppId
        {
            get;
            set;
        }


        public Guid MemberId
        {
            get;
            set;
        }

        public Guid MemberCardId
        {
            get;
            set;
        }
    }
}
