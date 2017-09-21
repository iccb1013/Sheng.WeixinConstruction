using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class MoveMemberListToGroupArgs
    {
        public List<String> OpenIdList
        {
            get;
            set;
        }

        public int GroupId
        {
            get;
            set;
        }
    }
}
