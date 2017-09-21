using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract.ThirdParty
{
    /// <summary>
    /// 获取授权方的账户信息
    /// </summary>
    [DataContract]
    public class WeixinThirdPartyGetAuthorizerAccountInfoArgs
    {
        /// <summary>
        /// 第三方平台appid
        /// </summary>
        [DataMember(Name = "component_appid")]
        public string ComponentAppId
        {
            get;
            set;
        }

        /// <summary>
        /// 授权方appid
        /// </summary>
        [DataMember(Name = "authorizer_appid")]
        public string AuthorizerAppId
        {
            get;
            set;
        }
    }
}
