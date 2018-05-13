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
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Management.Core;
using Sheng.WeixinConstruction.Management.Shell.Models;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using Sheng.WeixinConstruction.WeixinContract.ThirdParty;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Controllers
{
    public class SettingsController : BasalController
    {
        private static readonly ManagementDomainPool _domainPool = ManagementDomainPool.Instance;
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly ThirdPartyManager _thirdPartyManager = ThirdPartyManager.Instance;
        private static readonly SettingsManager _settingsManager = SettingsManager.Instance;
        private static readonly PortalTemplatePool _portalTemplatePool = PortalTemplatePool.Instance;
        private static readonly ScenicQRCodeManager _scenicQRCodeManager = ScenicQRCodeManager.Instance;
        private static readonly UserManager _userManager = UserManager.Instance;

        private static readonly LogService _logService = LogService.Instance;

        #region 基本设置

        public ActionResult Base()
        {
            //没有绑定公众号的，初始化一个临时的设置对象用于UI显示
            //此时UI是不允许编辑的
            BaseViewModel model = new BaseViewModel();
            model.MemberCardLevelList = MemberManager.Instance.GetMemberCardList(DomainContext.Domain.Id, DomainContext.AppId);

           // if (String.IsNullOrEmpty(DomainContext.AppId) == false)
            if (DomainContext.Online)
            {
                model.Settings = _settingsManager.GetSettings(UserContext.User.Domain, DomainContext.AppId);
                model.ThemeStyleSettings = _settingsManager.GetThemeStyleSettings(UserContext.User.Domain, DomainContext.AppId);
            }
            else
            {
                model.Settings = new SettingsEntity();
                model.ThemeStyleSettings = new ThemeStyleSettingsEntity();
            }

            return View(model);
        }

        #endregion

        #region 对接

        public ActionResult Docking()
        {
            DockingViewModel model = new DockingViewModel();
            model.Authorizer = _domainManager.GetOnlineAuthorizer(UserContext.User.Domain);
            model.UndockingAuthorizerList = _domainManager.GetUndockingAuthorizerList(UserContext.User.Domain);
            return View(model);
        }

        /// <summary>
        /// 发起授权页的体验URL：用于审核人员前往授权页体验，确认流程可用性。在提交全网发布时，
        /// 务必保证该URL可直接体验。全网发布之前，该项可先填为发起页域名
        /// </summary>
        /// <returns></returns>
        [AllowedAnonymous]
        public ActionResult DockingDemo()
        {
            return View();
        }

        [AllowedAnonymous]
        public ActionResult AuthorizationCallbackDemo(string auth_code)
        {
            RequestApiResult<WeixinThirdPartyGetAuthorizationInfoResult> result =
                ThirdPartyApiWrapper.GetAuthorizationInfo(auth_code);

            AuthorizationCallbackDemoViewModel model = new AuthorizationCallbackDemoViewModel();
            model.Info = result.ApiResult.AuthorizationInfo;
            return View(model);
        }

        /// <summary>
        /// 第三方平台运营时，授权完成时的回调页面
        /// 授权流程完成后，会进入回调URI，并在URL参数中返回授权码和过期时间(redirect_url?auth_code=xxx&expires_in=600)
        /// 一个公众号不能同时授权给两个帐户，因为微信在推送数据时只带一个APPID，我无法判断其属于哪个Domain
        /// 但是允许其在解除授权后得新授权给另一个帐户
        /// </summary>
        /// <returns></returns>
        public ActionResult AuthorizationCallback(string auth_code)
        {
            //防止由于用户长时间停留在微信页面，回来时超时 DomainContext 为空
            //会直接返回到登录页面

            //引处得到授权码，根据授权码获取授权公众号的令牌
            //然后获取其详细信息
            //开始运行此公众号
            ApiResult<CreateAuthorizerResult> result =
                ThirdPartyAccessTokenGetter.CreateAuthorizer(DomainContext.Domain.Id, auth_code);

            if (result.Success == false)
            {
                _logService.Write("授权回调时创建公众号授权信息失败", result.Message, TraceEventType.Error);
                //TODO:创建公众号授权信息失败
                return new HttpStatusCodeResult(500);
            }

            //更新公众号帐户详细信息
            _thirdPartyManager.UpdateAuthorizerAccountInfo(DomainContext.Domain.Id, result.Data.AppId);

            //立即获取帐户信息和配置信息
            DomainContext.Refresh();

            return RedirectToAction("Docking");
        }

        #endregion

        #region 自动回复

        public ActionResult AutoReplyOnSubscribe()
        {
            return View();
        }

        public ActionResult AutoReplyOnMessage()
        {
            return View();
        }

        public ActionResult AutoReplyOnKeywords()
        {
            return View();
        }

        public ActionResult AutoReplyOnKeywordsRule()
        {
            return View();
        }

        public ActionResult AutoReplyText()
        {
            return View();
        }

        public ActionResult AutoReplyOnKeywordsKeywordEdit()
        {
            return View();
        }

        #endregion

        #region 场景二维码

        public ActionResult ScenicQRCode()
        {
            return View();
        }

        public ActionResult ScenicQRCodeCreate()
        {
            return View();
        }

        public ActionResult ScenicQRCodeView()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                //TODO:ID无效
                //return RedirectToAction("PointCommodity", new { domain = domain });
            }

            ScenicQRCodeViewViewModel model = new ScenicQRCodeViewViewModel();
            model.ScenicQRCode = _scenicQRCodeManager.GetScenicQRCode(id);
            if (model.ScenicQRCode == null)
            {
                //TODO:不存在
            }

            UserEntity user = _userManager.GetUser(model.ScenicQRCode.CreateUser);
            if (user != null)
            {
                model.CreateUserName = user.Name;
            }

            return View(model);
        }

        #endregion

        #region 会员卡级别

        public ActionResult MemberCardLevel()
        {
            return View();
        }

        public ActionResult MemberCardLevelEdit()
        {
            return View();
        }

        #endregion

        #region 增强图文

        public ActionResult AdvancedArticle()
        {
            return View();
        }

        public ActionResult AdvancedArticleEdit()
        {
            ViewBag.PageUrl = String.Format("{0}Home/AdvancedArticle/{1}",
                _settingsManager.GetClientAddress(DomainContext), DomainContext.Domain.Id);
            return View();
        }

        public ActionResult AdvertisingSelect()
        {
            ViewBag.Key = Request.QueryString["key"];
            return View();
        }

        #endregion

        #region 广告

        public ActionResult Advertising()
        {
            return View();
        }

        public ActionResult AdvertisingEdit()
        {
            return View();
        }

        #endregion
    }
}