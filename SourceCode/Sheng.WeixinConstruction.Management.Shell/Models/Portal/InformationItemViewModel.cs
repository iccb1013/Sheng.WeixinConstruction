using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Management.Shell.Models
{
    public class InformationItemViewModel
    {
        public InformationEntity Information
        {
            get;
            set;
        }

        public InformationCategoryEntity Category
        {
            get;
            set;
        }
    }
}