using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    [DataContract]
    public class WeixinGetUserGroupIdResult
    {
        [DataMember(Name = "groupid")]
        public int GroupId
        {
            get;
            set;
        }
    }
}
