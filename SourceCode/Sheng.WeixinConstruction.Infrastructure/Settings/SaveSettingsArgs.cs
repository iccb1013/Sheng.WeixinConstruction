using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class SaveSettingsArgs
    {
        public SettingsEntity Settings
        {
            get;
            set;
        }

        public ThemeStyleSettingsEntity ThemeStyleSettings
        {
            get;
            set;
        }
    }
}
