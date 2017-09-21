using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Management.Shell.Models
{
    public class DockingViewModel
    {
        public AuthorizerEntity Authorizer
        {
            get;
            set;
        }

        public List<AuthorizerEntity> UndockingAuthorizerList
        {
            get;
            set;
        }
    }
}