using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class CustomFormViewModel
    {
        public CustomFormEntity CustomForm
        {
            get;
            set;
        }

        public CustomFormContentEntity Content
        {
            get;
            set;
        }

        public MemberEntity Member
        {
            get;
            set;
        }

        public WeixinJsApiConfig JsApiConfig
        {
            get;
            set;
        }
    }
}