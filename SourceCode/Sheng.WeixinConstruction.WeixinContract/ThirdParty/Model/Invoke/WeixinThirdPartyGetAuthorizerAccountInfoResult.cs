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
     * 注意，这个接口官方页面给出的返回JSON有错误
     * qrcode_url 是 authorizer_info 的属性，而不是根级别的属性
     * authorization_info 下面 是authorizer_appid，而不是文档中的appid
     * 正确如下：
     * {"authorizer_info":{"nick_name":"升讯威软件技术有限公司","head_img":"http:\/\/wx.qlogo.cn\/mmopen\/bFj91MgL38ds7PnEHJWDIPwHOvicfjnXc4IVDWvCia9FI06TibM2QbTicPt5ThWibIa8ObFxvdwEicqzsp1XcexaZr2uBf07Sjc75I\/0","service_type_info":{"id":2},"verify_type_info":{"id":0},"user_name":"gh_2eaaab12bf43","alias":"","qrcode_url":"http:\/\/mmbiz.qpic.cn\/mmbiz\/9wc35jemHx3KKVK300BVC0tybudmOkiaXhKtchvBibcmt2GmT3Uk3bFiaZyoNPtNLbzx5Aexy9Dfg4wCticBOUFpNA\/0","business_info":{"open_pay":1,"open_shake":0,"open_scan":0,"open_card":0,"open_store":0}},"authorization_info":{"authorizer_appid":"wx8c36b3c0000a0a49","func_info":[{"funcscope_category":{"id":1}},{"funcscope_category":{"id":4}},{"funcscope_category":{"id":7}},{"funcscope_category":{"id":2}},{"funcscope_category":{"id":3}},{"funcscope_category":{"id":11}},{"funcscope_category":{"id":6}},{"funcscope_category":{"id":5}},{"funcscope_category":{"id":8}},{"funcscope_category":{"id":13}},{"funcscope_category":{"id":10}},{"funcscope_category":{"id":12}}]}}
     * 
     */

    /// <summary>
    /// 获取授权方的账户信息
    /// </summary>
    [DataContract]
    public class WeixinThirdPartyGetAuthorizerAccountInfoResult
    {
        [DataMember(Name = "authorizer_info")]
        public WeixinThirdPartyAuthorizerAccountInfo AccountInfo
        {
            get;
            set;
        }

        [DataMember(Name = "authorization_info")]
        public WeixinThirdPartyGetAuthorizerAccountInfoResult_AuthorizationInfo AuthorizationInfo
        {
            get;
            set;
        }

    }

    [DataContract]
    public class WeixinThirdPartyGetAuthorizerAccountInfoResult_AuthorizationInfo
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
