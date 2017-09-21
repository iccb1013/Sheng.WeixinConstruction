using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    [DataContract]
    public class WeixinAccessTokenResult
    {
        [DataMember(Name = "access_token")]
        public string AccessToken
        {
            get;
            set;
        }

        /// <summary>
        /// 微信API返回的单位是秒
        /// </summary>
        [DataMember(Name = "expires_in")]
        public int ExpiresIn
        {
            get;
            set;
        }
    }
}
