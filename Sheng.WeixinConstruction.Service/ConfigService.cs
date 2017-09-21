using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Service
{
    public class ConfigService
    {
        private static readonly ConfigService _instance = new ConfigService();
        public static ConfigService Instance
        {
            get { return _instance; }
        }


        public WxConfigurationSection Configuration
        {
            get;
            private set;
        }

        private ConfigService()
        {
            Configuration =
                ConfigurationManager.GetSection("wxConfiguration") as WxConfigurationSection;
        }
    }
}
