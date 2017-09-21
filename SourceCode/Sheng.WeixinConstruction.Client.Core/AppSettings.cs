using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Core
{
    public static class AppSettings
    {
        private static string _intranetIp;
        public static string IntranetIp
        {
            get
            {
                if (String.IsNullOrEmpty(_intranetIp))
                {
                    _intranetIp = ConfigurationManager.AppSettings["IntranetIp"];
                }
                return _intranetIp;
            }
        }
    }
}