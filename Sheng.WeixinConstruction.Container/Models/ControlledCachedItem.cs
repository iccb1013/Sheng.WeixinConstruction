using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Container.Models
{
    public class ControlledCachedItem
    {
        public DateTime ExpiryTime
        {
            get;
            set;
        }

        public string Data
        {
            get;
            set;
        }
    }
}