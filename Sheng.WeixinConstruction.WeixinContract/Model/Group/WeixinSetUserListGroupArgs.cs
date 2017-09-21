using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    [DataContract]
    public class WeixinSetUserListGroupArgs
    {
        private List<string> _openIdList = new List<string>();
        [DataMember(Name = "openid_list")]
        public List<string> OpenIdList
        {
            get { return _openIdList; }
            set { _openIdList = value; }
        }

        [DataMember(Name = "to_groupid")]
        public int GroupId
        {
            get;
            set;
        }
    }
}
