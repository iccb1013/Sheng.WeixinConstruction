using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Areas.Staff.Models
{
    public class MemberInfoViewModel
    {
        public List<MemberCardLevelEntity> MemberCardLevelList
        {
            get;
            set;
        }
    }
}