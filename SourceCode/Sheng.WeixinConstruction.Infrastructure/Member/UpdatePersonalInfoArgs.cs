using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class UpdatePersonalInfoArgs
    {
        public Guid MemberId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public DateTime? Birthday
        {
            get;
            set;
        }

        public string MobilePhone
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }
    }
}
