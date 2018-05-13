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
    [DataContract]
    public class WeixinThirdPartyGetAuthorizationInfoArgs
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
        /// 授权code,会在授权成功时返回给第三方平台，详见第三方平台授权流程说明
        /// </summary>
        [DataMember(Name = "authorization_code")]
        public string AuthorizationCode
        {
            get;
            set;
        }
    }
}
