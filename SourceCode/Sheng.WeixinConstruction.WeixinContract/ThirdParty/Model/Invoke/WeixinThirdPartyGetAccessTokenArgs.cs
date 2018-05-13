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
    public class WeixinThirdPartyGetAccessTokenArgs
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
        /// 第三方平台appsecret
        /// </summary>
        [DataMember(Name = "component_appsecret")]
        public string ComponentAppSecret
        {
            get;
            set;
        }

        /// <summary>
        /// 微信后台推送的ticket，此ticket会定时推送，具体请见本页末尾的推送说明
        /// </summary>
        [DataMember(Name = "component_verify_ticket")]
        public string VerifyTicket
        {
            get;
            set;
        }
    }
}
