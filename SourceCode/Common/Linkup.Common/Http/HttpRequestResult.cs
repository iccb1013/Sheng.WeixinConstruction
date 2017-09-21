using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkup.Common
{
    public class HttpRequestResult
    {
        public bool Success
        {
            get
            {
                return Exception == null;
            }
        }

        public Exception Exception
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }
    }
}
