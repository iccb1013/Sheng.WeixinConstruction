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
    /*
     * 该API用于使用授权码换取授权公众号的授权信息，
     * 并换取authorizer_access_token和authorizer_refresh_token。 授权码的获取，
     * 需要在用户在第三方平台授权页中完成授权流程后，在回调URI中通过URL参数提供给第三方平台方。
     * 
     */
    [DataContract]
    public class WeixinThirdPartyAuthorizationInfo
    {
        /// <summary>
        /// 授权方appid
        /// </summary>
        [DataMember(Name = "authorizer_appid")]
        public string AppId
        {
            get;
            set;
        }

        /// <summary>
        /// 授权方令牌（在授权的公众号具备API权限时，才有此返回值）
        /// </summary>
        [DataMember(Name = "authorizer_access_token")]
        public string AccessToken
        {
            get;
            set;
        }

        /// <summary>
        /// 有效期（在授权的公众号具备API权限时，才有此返回值）
        /// </summary>
        [DataMember(Name = "expires_in")]
        public int ExpiresIn
        {
            get;
            set;
        }

        /// <summary>
        /// 刷新令牌（在授权的公众号具备API权限时，才有此返回值），
        /// 刷新令牌主要用于公众号第三方平台获取和刷新已授权用户的access_token，只会在授权时刻提供，请妥善保存。 
        /// 一旦丢失，只能让用户重新授权，才能再次拿到新的刷新令牌
        /// </summary>
        [DataMember(Name = "authorizer_refresh_token")]
        public string RefreshToken
        {
            get;
            set;
        }

        /// <summary>
        /// 公众号授权给开发者的权限集列表（请注意，当出现用户已经将消息与菜单权限集授权给了某个第三方，
        /// 再授权给另一个第三方时，由于该权限集是互斥的，后一个第三方的授权将去除此权限集，
        /// 开发者可以在返回的func_info信息中验证这一点，避免信息遗漏）
        /// </summary>
        [DataMember(Name = "func_info")]
        public WeixinThirdPartyFuncscopeCategoryCollection FuncScopeCategoryList
        {
            get;
            set;
        }
    }
}
