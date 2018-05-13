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
    public class WeixinThirdPartyAuthorizerAccountInfo
    {
        /// <summary>
        /// 授权方昵称
        /// </summary>
        [DataMember(Name = "nick_name")]
        public string NickName
        {
            get;
            set;
        }

        /// <summary>
        /// 授权方头像
        /// </summary>
        [DataMember(Name = "head_img")]
        public string HeadImg
        {
            get;
            set;
        }

        /// <summary>
        /// 授权方公众号类型，0代表订阅号，1代表由历史老帐号升级后的订阅号，2代表服务号
        /// </summary>
        [DataMember(Name = "service_type_info")]
        public WeixinThirdPartyAuthorizerAccountInfo_TypeInfo ServiceType
        {
            get;
            set;
        }

        /// <summary>
        /// 授权方认证类型，-1代表未认证，0代表微信认证，1代表新浪微博认证，2代表腾讯微博认证，
        /// 3代表已资质认证通过但还未通过名称认证，4代表已资质认证通过、还未通过名称认证，但通过了新浪微博认证，
        /// 5代表已资质认证通过、还未通过名称认证，但通过了腾讯微博认证
        /// </summary>
        [DataMember(Name = "verify_type_info")]
        public WeixinThirdPartyAuthorizerAccountInfo_TypeInfo VerifyType
        {
            get;
            set;
        }

        /// <summary>
        /// 授权方公众号的原始ID
        /// </summary>
        [DataMember(Name = "user_name")]
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        ///  二维码图片的URL，开发者最好自行也进行保存
        /// </summary>
        [DataMember(Name = "qrcode_url")]
        public string QRCodeUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 用以了解以下功能的开通状况（0代表未开通，1代表已开通）
        /// </summary>
        [DataMember(Name = "business_info")]
        public WeixinThirdPartyAuthorizerAccountInfo_BusinessInfo Business
        {
            get;
            set;
        }

        /// <summary>
        /// 授权方公众号所设置的微信号，可能为空
        /// 实测发现是没有这个字段
        /// </summary>
        [DataMember(Name = "alias")]
        public string Alias
        {
            get;
            set;
        }
    }

    [DataContract]
    public class WeixinThirdPartyAuthorizerAccountInfo_TypeInfo
    {
        [DataMember(Name = "id")]
        public int Id
        {
            get;
            set;
        }
    }

    [DataContract]
    public class WeixinThirdPartyAuthorizerAccountInfo_BusinessInfo
    {
        /// <summary>
        /// 是否开通微信门店功能
        /// </summary>
        [DataMember(Name = "open_store")]
        public int Store
        {
            get;
            set;
        }

        /// <summary>
        /// 是否开通微信扫商品功能
        /// </summary>
        [DataMember(Name = "open_scan")]
        public int Scan
        {
            get;
            set;
        }

        /// <summary>
        /// 是否开通微信支付功能
        /// </summary>
        [DataMember(Name = "open_pay")]
        public int Pay
        {
            get;
            set;
        }

        /// <summary>
        /// 是否开通微信卡券功能
        /// </summary>
        [DataMember(Name = "open_card")]
        public int Card
        {
            get;
            set;
        }

        /// <summary>
        /// 是否开通微信摇一摇功能
        /// </summary>
        [DataMember(Name = "open_shake")]
        public int Shake
        {
            get;
            set;
        }
    }
}
