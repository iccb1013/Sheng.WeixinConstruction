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
    /// 获取（刷新）授权公众号的令牌
    /// </summary>
    [DataContract]
    public class WeixinThirdPartyGetAuthorizerAccessTokenResult
    {
        /// <summary>
        /// 授权方令牌
        /// </summary>
        [DataMember(Name = "authorizer_access_token")]
        public string AccessToken
        {
            get;
            set;
        }

        /// <summary>
        /// 有效期，为2小时
        /// </summary>
        [DataMember(Name = "expires_in")]
        public int ExpiresIn
        {
            get;
            set;
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        [DataMember(Name = "authorizer_refresh_token")]
        public string RefreshToken
        {
            get;
            set;
        }
    }
}
