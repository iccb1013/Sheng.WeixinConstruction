/*
********************************************************************
*
*    曹旭升（sheng.c）
*    E-mail: cao.silhouette@msn.com
*    QQ: 279060597
*    https://github.com/iccb1013
*    http://shengxunwei.com
*
*    © Copyright 2016
*
********************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract.ThirdParty
{
    /// <summary>
    /// 获取第三方平台access_token
    /// </summary>
    [DataContract]
    public class WeixinThirdPartyGetAccessTokenResult
    {
        [DataMember(Name = "component_access_token")]
        public string AccessToken
        {
            get;
            set;
        }

        private int _expiresIn;
        /// <summary>
        /// 微信API返回的单位是秒
        /// </summary>
        [DataMember(Name = "expires_in")]
        public int ExpiresIn
        {
            get { return _expiresIn; }
            set
            {
                _expiresIn = value;
                AccessTokenExpiryTime = DateTime.Now.AddSeconds(_expiresIn);
            }
        }

        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime AccessTokenExpiryTime
        {
            get;
            set;
        }

        /// <summary>
        /// 快要到期了
        /// </summary>
        public bool WillbeTimeout
        {
            get
            {
                //小于1200秒，即20分钟，就刷新
                //比公众号ACCESSTOKEN的30分钟提前一些
                if ((AccessTokenExpiryTime - DateTime.Now).TotalSeconds <= 1200)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
