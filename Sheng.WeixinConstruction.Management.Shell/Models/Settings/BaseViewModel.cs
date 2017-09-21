using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Management.Shell.Models
{
    public class BaseViewModel
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

        public List<MemberCardLevelEntity> MemberCardLevelList
        {
            get;
            set;
        }
    }
}