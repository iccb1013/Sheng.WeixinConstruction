using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class InformationCategoryViewModel
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