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
    public class WeixinThirdPartyGetAuthorizerAccessTokenArgs
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

        /// <summary>
        /// 授权方的刷新令牌，刷新令牌主要用于公众号第三方平台获取和刷新已授权用户的access_token，
        /// 只会在授权时刻提供，请妥善保存。一旦丢失，只能让用户重新授权，才能再次拿到新的刷新令牌
        /// </summary>
        [DataMember(Name = "authorizer_refresh_token")]
        public string AuthorizerRefreshToken
        {
            get;
            set;
        }
    }
}
