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

namespace Sheng.WeixinConstruction.WeixinContract
{
    //http://mp.weixin.qq.com/wiki/9/01f711493b5a02f24b04365ac5d8fd95.html
    //通过code换取网页授权access_token

    [DataContract]
    public class WeixinWebAccessTokenResult
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

        [DataMember(Name = "refresh_token")]
        public string RefreshToken
        {
            get;
            set;
        }

        [DataMember(Name = "openid")]
        public string OpenId
        {
            get;
            set;
        }

        [DataMember(Name = "scope")]
        public string Scope
        {
            get;
            set;
        }

        [DataMember(Name = "unionid")]
        public string Unionid
        {
            get;
            set;
        }
    }
}
