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


using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    /// <summary>
    /// 作为第三方平台运营时，授权的公众号信息
    /// </summary>
    [Table("Authorizer")]
    public class AuthorizerEntity
    {
        /// <summary>
        /// 授权方appid
        /// </summary>
        [Key]
        public string AppId
        {
            get;
            set;
        }

        /// <summary>
        /// 一个域下面有可能存在多个 授权过 的公众 号
        /// 但只允许一个是 当前 正在运营的
        /// </summary>
        [Key]
        public Guid Domain
        {
            get;
            set;
        }

        /// <summary>
        /// 最新一次的授权时间
        /// </summary>
        public DateTime AuthorizationTime
        {
            get;
            set;
        }

        /// <summary>
        /// 是否正在运营
        /// 如果取消了授权，则为false
        /// </summary>
        public bool Online
        {
            get;
            set;
        }

        //基本信息配置
        //为了兼容独立运行模式，这些字段通过 DomainContext 封装并使用



        //公众号基本信息

        /// <summary>
        /// 刷新令牌（在授权的公众号具备API权限时，才有此返回值），
        /// 刷新令牌主要用于公众号第三方平台获取和刷新已授权用户的access_token，只会在授权时刻提供，请妥善保存。 
        /// 一旦丢失，只能让用户重新授权，才能再次拿到新的刷新令牌
        /// </summary>
        public string RefreshToken
        {
            get;
            set;
        }

        public DateTime RefreshTokenGetTime
        {
            get;
            set;
        }

        public string AccessToken
        {
            get;
            set;
        }

        public DateTime? AccessTokenExpiryTime
        {
            get;
            set;
        }

        public string JsApiTicket
        {
            get;
            set;
        }

        public DateTime? JsApiTicketExpiryTime
        {
            get;
            set;
        }

        //////////////

        public string NickName
        {
            get;
            set;
        }

        public string HeadImg
        {
            get;
            set;
        }

        /// <summary>
        /// 授权方公众号类型，0代表订阅号，1代表由历史老帐号升级后的订阅号，2代表服务号
        /// </summary>
        public int ServiceType
        {
            get;
            set;
        }

        [NotMapped]
        public string ServiceTypeString
        {
            get
            {
                switch (ServiceType)
                {
                    case 0:
                    case 1:
                        return "订阅号";
                    case 2:
                        return "服务号";
                    default:
                        return "未知";
                }
            }
        }

        /// <summary>
        /// 授权方认证类型，-1代表未认证，0代表微信认证，1代表新浪微博认证，2代表腾讯微博认证，
        /// 3代表已资质认证通过但还未通过名称认证，4代表已资质认证通过、还未通过名称认证，但通过了新浪微博认证，
        /// 5代表已资质认证通过、还未通过名称认证，但通过了腾讯微博认证
        /// </summary>
        public int VerifyType
        {
            get;
            set;
        }

        [NotMapped]
        public string VerifyTypeString
        {
            get
            {
                switch (VerifyType)
                {
                    case -1:
                        return "未认证";
                    case 0:
                        return "微信认证";
                    case 1:
                        return "新浪微博认证";
                    case 2:
                        return "腾讯微博认证";
                    case 3:
                        return "已资质认证通过但还未通过名称认证";
                    case 4:
                        return "已资质认证通过、还未通过名称认证，但通过了新浪微博认证";
                    case 5:
                        return "已资质认证通过、还未通过名称认证，但通过了腾讯微博认证";
                    default:
                        return "未知";
                }
            }
        }

        /// <summary>
        /// 公众号类型的接口权限
        /// 0未认证订阅号 1微信认证订阅号 2未认证服务号 3微信认证服务号
        /// </summary>
        [NotMapped]
        public EnumAuthorizationType AuthorizationType
        {
            get
            {
                switch (ServiceType)
                {
                    case 0:
                    case 1:
                        //订阅号
                        if (QualificationApprobatory)
                            return EnumAuthorizationType.AuthorizedSubscription;
                        else
                            return EnumAuthorizationType.UnauthorizedSubscription;
                    case 2:
                        //服务号
                        if (QualificationApprobatory)
                            return EnumAuthorizationType.AuthorizedService;
                        else
                            return EnumAuthorizationType.UnauthorizedService;
                    default:
                        //未知
                        return EnumAuthorizationType.Unknow;
                }
            }
        }

        /// <summary>
        /// 是否微信资质认证通过，即是否拥有微信认证的接口权限
        /// 微博认证视作未认证,因此微博认证的公众号不会拥有微信认证公众号特有的接口
        /// 微信认证分为资质认证和名称认证两部分，只需要资质认证通过，就可获得接口。
        /// </summary>
        [NotMapped]
        public bool QualificationApprobatory
        {
            get
            {
                if (VerifyType == 0 || VerifyType == 3 || VerifyType == 4 || VerifyType == 5)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 授权方公众号的原始ID
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// 授权方公众号所设置的微信号，可能为空
        /// </summary>
        public string Alias
        {
            get;
            set;
        }

        /// <summary>
        /// 是否开通微信门店功能
        /// </summary>
        public bool Store
        {
            get;
            set;
        }

        /// <summary>
        /// 是否开通微信扫商品功能
        /// </summary>
        public bool Scan
        {
            get;
            set;
        }

        /// <summary>
        /// 是否开通微信支付功能
        /// </summary>
        public bool Pay
        {
            get;
            set;
        }

        /// <summary>
        /// 是否开通微信卡券功能
        /// </summary>
        public bool Card
        {
            get;
            set;
        }

        /// <summary>
        /// 是否开通微信摇一摇功能
        /// </summary>
        public bool Shake
        {
            get;
            set;
        }

        /// <summary>
        /// 二维码
        /// 但是微信接口返回的图片地址居然是不允许引用的！
        /// </summary>
        public string QRCodeUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 1,2,3,4,5
        /// </summary>
        public string FuncScopeCategory
        {
            get;
            set;
        }
    }
}
