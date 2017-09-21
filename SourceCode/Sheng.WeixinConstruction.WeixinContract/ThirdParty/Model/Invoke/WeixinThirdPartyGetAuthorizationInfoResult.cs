using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract.ThirdParty
{
    [DataContract]
    public class WeixinThirdPartyGetAuthorizationInfoResult
    {
        [DataMember(Name = "authorization_info")]
        public WeixinThirdPartyAuthorizationInfo AuthorizationInfo
        {
            get;
            set;
        }
    }
}
