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


using Linkup.Common;
using Newtonsoft.Json.Linq;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class DomainContext //: IWeixinApp
    {
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly SettingsManager _settingsManager = SettingsManager.Instance;
        private static readonly RecommendUrlManager _recommendUrlManager = RecommendUrlManager.Instance;
        protected static readonly MemberManager _memberManager = MemberManager.Instance;
        protected static readonly LogService _logService = LogService.Instance;

        public DomainEntity Domain
        {
            get;
            set;
        }

        /// <summary>
        /// 作为第三方平台运营时的授权方信息
        /// </summary>
        public AuthorizerEntity Authorizer
        {
            get;
            set;
        }

        /// <summary>
        /// 微信支付相关配置
        /// </summary>
        public AuthorizerPayConfig AuthorizerPay
        {
            get;
            set;
        }

        /// <summary>
        /// 与当前 Authorizer 关联的设置
        /// </summary>
        public SettingsEntity Settings
        {
            get;
            set;
        }

        /// <summary>
        /// 样式相关设置
        /// </summary>
        public StyleSettingsEntity StyleSettings
        {
            get;
            set;
        }

        /// <summary>
        /// RecommendUrl相关设置
        /// </summary>
        public RecommendUrlSettingsEntity RecommendUrlSettings
        {
            get;
            set;
        }

        public bool Online
        {
            get
            {
                return Authorizer != null && Authorizer.Online;
            }
        }

        /// <summary>
        /// 是否开通并对接了微信支付
        /// </summary>
        public bool Pay
        {
            get
            {

                return Authorizer != null && Authorizer.Pay && AuthorizerPay != null;
            }
        }

        /// <summary>
        /// 获取授权公众号的authorizer_access_token
        /// 用于代公众号调用接口
        /// </summary>
        /// <returns></returns>
        public string AccessToken
        {
            get
            {
                if (Authorizer == null || Authorizer.Online == false)
                    return null;

                return AccessTokenGetter.Get(Authorizer.AppId);
            }
        }

        //为了兼容独立运行模式
        //封装授权公众号中的一些信息

        public string AppId
        {
            get
            {
                if (Authorizer == null)
                    return null;
                else
                    return Authorizer.AppId;
            }
        }

        /// <summary>
        /// 公众号的原始ID
        /// </summary>
        public string UserName
        {
            get
            {
                if (Authorizer == null)
                    return null;
                else
                    return Authorizer.UserName;
            }
        }

        public string PortalImageUrl
        {
            get
            {
                if (StyleSettings == null)
                    return null;
                else
                    return StyleSettings.PortalImageUrl;
            }
        }

        public string GuideSubscribeUrl
        {
            get
            {
                if (Settings == null || String.IsNullOrEmpty(Settings.GuideSubscribeUrl))
                    return null;
                else
                    return Settings.GuideSubscribeUrl;
            }
        }

        public int InitialMemberPoint
        {
            get
            {
                if (Settings == null)
                    return 0;
                else
                    return Settings.InitialMemberPoint;
            }
        }

        public int SignInPoint
        {
            get
            {
                if (Settings == null)
                    return 0;
                else
                    return Settings.SignInPoint;
            }
        }

        public string PointCommodityGetWay
        {
            get
            {
                if (Settings == null)
                    return null;
                else
                    return Settings.PointCommodityGetWay;
            }
        }

        /////////////////

        public DomainContext(DomainEntity domain)
        {
            Domain = domain;

            Refresh();
        }

        public void Refresh()
        {
            //LastUpdateDate也要同步！
            Domain = _domainManager.GetDomain(Domain.Id);

            Authorizer = _domainManager.GetOnlineAuthorizer(Domain.Id);
            AuthorizerPay = _domainManager.GetAuthorizerPayConfig(Domain.Id, AppId);

            Settings = _settingsManager.GetSettings(Domain.Id, AppId);
            StyleSettings = _settingsManager.GetStyleSettings(Domain.Id, AppId);
            RecommendUrlSettings = _recommendUrlManager.GetSettings(Domain.Id, AppId);

            OnRefresh();
        }

        protected virtual void OnRefresh()
        {
        }
    }
}