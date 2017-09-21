using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Management.Shell.Models
{
    public class ScenicQRCodeViewViewModel
    {
        public ScenicQRCodeEntity ScenicQRCode
        {
            get;
            set;
        }

        public string CreateUserName
        {
            get;
            set;
        }
    }
}