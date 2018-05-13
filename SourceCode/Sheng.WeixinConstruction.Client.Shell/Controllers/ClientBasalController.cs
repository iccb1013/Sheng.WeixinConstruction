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
using Newtonsoft.Json;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Client.Core;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sheng.WeixinConstruction.Client.Shell
{
    /// <summary>
    /// 用于微信端页面控制器的基类
    /// 访问action时必须有domainId，否则返回404
    /// </summary>
    public class ClientBasalController : BasalController
    {
        //如果是调试模式，则直接模拟一个用户而不请求微信接口
        protected bool _debug = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["debug"]);

        private static readonly LogService _log = LogService.Instance;
        protected static LogService Log
        {
            get
            {
                return _log;
            }
        }

        protected static readonly DomainManager _domainManager = DomainManager.Instance;
        protected static readonly MemberManager _memberManager = MemberManager.Instance;
        protected static readonly UserManager _userManager = UserManager.Instance;
        protected static readonly CachingService _cachingService = CachingService.Instance;
        protected static readonly SettingsManager _settingsManager = SettingsManager.Instance;
        protected static readonly AdvertisingManager _advertisingManager = AdvertisingManager.Instance;
        protected static readonly PortalManager _portalManager = PortalManager.Instance;
        protected static readonly RecommendUrlManager _recommendUrlManager = RecommendUrlManager.Instance;


        protected static readonly WxConfigurationSection _configuration = ConfigService.Instance.Configuration;

        private static TimeSpan _shortUrlCachingTime = new TimeSpan(0, 1, 0);

        public ClientDomainContext DomainContext
        {
            get;
            set;
        }

        /// <summary>
        /// 是有可能为空的，在允许未关注也可浏览的页面中
        /// </summary>
        public MemberContext MemberContext
        {
            get;
            set;
        }

        /// <summary>
        /// 在允许未关注也可浏览的页面中，只能取到 OpenId
        /// 在这种情况下，会没有MemberContext
        /// </summary>
        public string OpenId
        {
            get { return SessionContainer.GetOpenId(HttpContext); }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.Result != null)
                return;

            //避免在没有指定 domainId 时反复进入 OnActionExecuting 形成死循环
            if (filterContext.RouteData.Values["action"].ToString() == "ErrorView")
                return;

            /*
             * 在该域名和符合要求的下级域名内，可以代替旗下授权后公众号发起网页授权。
             * 下级域名必须是$APPID$.wx.abc.com的形式
             * （$APPID$为公众号的AppID的替换符，建议第三方用这种方式，若需可做域名映射），
             * 如果不按这种形式来做，旗下公众号违规将可能导致整个网站被封。
             * 
             * 
             * 此处，使用域名泛解析，DNS上直接设置 *.wxc.shengxunwei.com
             * 要注意的是 wxc.shengxunwei.com 必须是80端口
             * 不论是作为第三方平台运营还是独立运营
             * domainId 还是要放在URL后面
             * 
             */

            //1.从SESSION中取出MemberContext，如果没有，跳转到微信网页授权地址
            //网页授权
            //a.取得用户OpenId，并判断用户当前是否关注当前公众号
            //b.如果用户已关注，但没有用户信息，则新建用户
            //c.如果用户不是已关注用户，跳转到引导关注页面

            Guid domainId = Guid.Empty;
            object objDomainId = filterContext.RouteData.Values["domainId"];
            if (objDomainId == null || Guid.TryParse(objDomainId.ToString(), out domainId) == false)
            {
                //判断是否允许不带domainId
                object[] objAllowedEmptyDomainArray =
                filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowedEmptyDomain), false);
                if (objAllowedEmptyDomainArray.Length > 0)
                {
                    return;
                }

                //重定向到错误页面
                filterContext.Result = new RedirectResult(String.Format(
                            "~/Home/ErrorView/?message={0}", "td7"));
                return;
            }

            DomainContext = ClientDomainPool.Instance.GetDomainContext(domainId);

            if (DomainContext == null || DomainContext.Domain == null)
            {
                Log.Write("指定的 domoainId 不存在", domainId.ToString(), TraceEventType.Warning);
                //重定向到错误页面
                filterContext.Result = new RedirectResult(String.Format(
                            "~/Home/ErrorView/?message={0}", "td6"));
                return;
            }

            if (DomainContext.Authorizer == null)
            {
                Log.Write("指定的 Domain 没有授权公众号信息", domainId.ToString(), TraceEventType.Warning);
                //重定向到错误页面
                filterContext.Result = new RedirectResult(String.Format(
                            "~/Home/ErrorView/{0}?message={1}", DomainContext.Domain.Id, "td5"));
                return;
            }

            ViewBag.Domain = DomainContext.Domain;
            ViewBag.DomainContext = DomainContext;
            ViewBag.Authorizer = DomainContext.Authorizer;

            //获取快捷菜单
            ShortcutMenuEntity shortcutMenuEntity =
                _portalManager.GetShortcutMenu(this.DomainContext.Domain.Id, this.DomainContext.AppId);
            ViewBag.ShortcutMenu = shortcutMenuEntity;

            //使用匿名浏览
            //及时匿名，Domain信息还是要取的
            object[] objAllowedAnonymousArray =
                filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowedAnonymous), false);
            if (objAllowedAnonymousArray.Length > 0)
            {
                return;
            }

            //只有微信认证服务号具备此权限
            //用户管理-网页授权获取用户openid/用户基本信息
            if (DomainContext.Authorizer.AuthorizationType != EnumAuthorizationType.AuthorizedService)
            {
                //重定向到错误页面
                filterContext.Result = new RedirectResult(String.Format(
                            "~/Home/ErrorView/{0}?message={1}", DomainContext.Domain.Id, "td4"));
                return;
            }

            MemberContext = SessionContainer.GetMemberContext(filterContext.HttpContext);

            // Uri uri = new Uri("http://wxctest.shengxunwei.com/WeixinApi/Handler/F6AAD430-CA1F-4AFD-B2B0-6E0D2FABB622");

            //如果有OpenId却没有MemberContext，说明此页面是AllowedOnlyOpenId，已经取到过OpenId了
            //这里要判断此页面是否允许匿名浏览，因为当没有关注的人访问页面时
            //首先跳到微信鉴权页面，然后回调时发现没关系，转到引导关注页面，但是他再点开主个页面时
            //此时Session中就已经有了OpenId，但是没有MemberContext
            if (MemberContext == null) // && String.IsNullOrEmpty(this.OpenId)
            {
                #region 调用微信网页授权接口取用户基本信息

                if (_debug)
                {
                    MemberEntity member = MemberManager.Instance.GetMemberByOpenId(domainId, DomainContext.AppId, "oXCfEwEteoWmCygMuCKqhvqshVnQ");
                    MemberContext = new MemberContext(member);
                    SessionContainer.SetMemberContext(HttpContext, MemberContext);
                    SessionContainer.SetOpenId(HttpContext, "oXCfEwEteoWmCygMuCKqhvqshVnQ");
                }
                else
                {
                    Uri currentUrl = filterContext.HttpContext.Request.Url;
                    string strCurrentUrl = currentUrl.ToString();
                    if (strCurrentUrl.Contains("AllowedOnlyOpenId") && String.IsNullOrEmpty(this.OpenId) == false)
                    {
                        //允许匿名浏览并且已经走过微信WEB鉴权了 //do nothing
                        //这里的逻辑是:
                        //1.第一次打开页面时，URL中是没有AllowedOnlyOpenId的，会被转到微信鉴权，转的时候带上AllowedOnlyOpenId（如果允许）
                        //2.鉴权完毕回来时候，URL上是还会带着 AllowedOnlyOpenId 的
                        //3.这里取到，并判断已经有了OpenId，那肯定是走过微信鉴权了，就不能再走了，否则死循环
                        //在这种情况下啥也不做，走完 OnActionExecuting 正常进入 Action
                    }
                    else
                    {
                        string redirectUrl = String.Format("{0}://{1}", currentUrl.Scheme, currentUrl.Host);
                        if (currentUrl.Port != 80)
                        {
                            redirectUrl += ":" + currentUrl.Port;
                        }

                        //微信鉴权以后回到OAuthCallback
                        redirectUrl += "/ThirdPartyWeixinApi/OAuthCallback?domainId=" + objDomainId.ToString();

                        //当前请求的页面，OAuthCallback中的逻辑走完以后，回到当前请求的页面
                        string state = filterContext.HttpContext.Request.Url.ToString();

                        //判断当前请求的页面是否允许未关注用户浏览，如果允许，在URL后面加参数
                        //以便完成weixin鉴权后判断是否跳转
                        object[] objAllowedOnlyOpenIdArray =
                            filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowedOnlyOpenId), false);
                        bool allowedOnlyOpenId = objAllowedOnlyOpenIdArray.Length > 0;

                        if (allowedOnlyOpenId && state.Contains("AllowedOnlyOpenId") == false)
                        {
                            if (state.IndexOf('?') >= 0)
                            {
                                state += "&AllowedOnlyOpenId=1";
                            }
                            else
                            {
                                state += "?AllowedOnlyOpenId=1";
                            }
                        }

                        //此处如果直接把网址放到 state 中传递，微信oauth鉴权时可能会提示state参数过长
                        //如 http://appid.wxc.shengxunwei.com/Home/QRCode/2a58d820-de07-4c8f-80b9-b5cb5a1028b4?type=member&memberId=ed638a09-f7aa-4d24-850e-a889a95bd454&cardNumber=13600864926982
                        //此处把要跳转的网址放在缓存中，用guid做key
                        //因为鉴权回调时并不是直接去这个state网址，而是去/ThirdPartyWeixinApi/OAuthCallback
                        //所以在回调的OAuthCallback方法中从缓存中取出这个地址即可
                        string stateKey = Guid.NewGuid().ToString();
                        _cachingService.Set(stateKey, state, _shortUrlCachingTime);
                        //TODO:临时解决方案。连调用2次防止写入失败
                        _cachingService.Set(stateKey, state, _shortUrlCachingTime);
                        state = stateKey;

                        //作为第三方平台运营时，多一个 component_appid 参数
                        //跳转到微信网页授权地址，取用户信息
                        string weiXinOAuthUrl = String.Format(
                            "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&state={2}&component_appid={3}#wechat_redirect",
                            DomainContext.AppId, Server.UrlEncode(redirectUrl), state, _configuration.ThirdParty.AppId);//Server.UrlEncode(state)

                        Log.Write("调用微信网页授权接口", weiXinOAuthUrl, TraceEventType.Verbose);

                        filterContext.Result = new RedirectResult(weiXinOAuthUrl);
                        return;
                    }
                }

                #endregion
            }

            //这里 MemberContext 不为空，表示走过 OAuth 鉴权了
            //这里只是更新MemberContext中的数据
            if (MemberContext != null)
            {
                //刷新用户的积分等信息Point，CashAccount,Staff,CardLevel
                //MemberContext.Member.Point = _memberManager.GetMemberPoint(MemberContext.Member.Id);
                //MemberContext.Member.CashAccount = _memberManager.GetMemberCashAccountBalances(MemberContext.Member.Id);
                //MemberContext.Member.Staff = _memberManager.IsStaff(MemberContext.Member.Id);
                MemberContext.Member = _memberManager.GetMember(MemberContext.Member.Id);
                ViewBag.Member = MemberContext.Member;
                ViewBag.ValidCouponCount = CouponManager.Instance.GetValidCouponCountByMember(
                    DomainContext.Domain.Id, DomainContext.AppId, MemberContext.Member.Id);

            }

            //这里不可放初始化相关代码
            //如：取DOMAIN，取快捷菜单，因为当匿名浏览时，走不到这里

        }

    }
}