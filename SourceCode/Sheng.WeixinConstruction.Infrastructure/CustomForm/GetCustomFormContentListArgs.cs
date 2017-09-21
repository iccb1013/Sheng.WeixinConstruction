using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetCustomFormContentListArgs : GetItemListArgs
    {
        public Guid Form
        {
            get;
            set;
        }

        public string NickName
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

    }
}