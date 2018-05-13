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


using Sheng.WeixinConstruction.Client.Shell.Models;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Controllers
{
    public class HomeController : ClientBasalController
    {
        private static readonly CampaignManager _campaignManager = CampaignManager.Instance;
        private static readonly PointCommodityManager _pointCommodityManager = PointCommodityManager.Instance;
        private static readonly InformationManager _informationManager = InformationManager.Instance;
        private static readonly CustomFormManager _customFormManager = CustomFormManager.Instance;
        private static readonly MovieManager _movieManager = MovieManager.Instance;
        private static readonly ErrorMessage _errorMessage = ErrorMessage.Instance;
        private static readonly PortalTemplatePool _portalTemplatePool = PortalTemplatePool.Instance;

        [AllowedEmptyDomain]
        [AllowedAnonymous]
        public ActionResult WebSite()
        {
            return new RedirectResult("http://wx.shengxunwei.com");
        }

        public ActionResult Portal(string domainId)
        {
            PortalViewModel model = new PortalViewModel();
            if (DomainContext.StyleSettings.PortalMode == Infrastructure.EnumPortalMode.Template)
            {
                if (DomainContext.StyleSettings.PortalPresetTemplate == null)
                {
                    model.ViewHtml = "模版配置异常";
                }
                else
                {
                    model.ViewHtml = _portalTemplatePool.Render(
                        DomainContext.StyleSettings.PortalPresetTemplate, DomainContext, MemberContext);
                }
            }
            else
            {
                model.ViewHtml = _portalTemplatePool.Render(
                    DomainContext.StyleSettings.PortalCustomTemplate, DomainContext, MemberContext);
            }
            return View(model);
        }

        public ActionResult PointAccount(string domainId)
        {
            return View();
        }

        public ActionResult MemberCenter(string domainId)
        {
            MemberCenterViewModel model = new MemberCenterViewModel();
            if (MemberContext.Member.CardLevel.HasValue)
            {
                model.MemberCard = _memberManager.GetMemberCard(MemberContext.Member.CardLevel.Value);
            }
            else
            {
                SettingsEntity settings = DomainContext.Settings;
                if (settings != null && settings.DefaultMemberCardLevel.HasValue)
                {
                    model.MemberCard = _memberManager.GetMemberCard(settings.DefaultMemberCardLevel.Value);
                }
            }

            return View(model);
        }

        public ActionResult PersonalInfo(string domainId)
        {
            PersonalInfoViewModel model = new PersonalInfoViewModel();
            model.Member = MemberContext.Member;
            return View(model);
        }

        [AllowedAnonymous]
        public ActionResult MovieTimes(string domainId)
        {
            DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
            string strDate = Request.QueryString["date"];
            if (String.IsNullOrEmpty(strDate) == false)
            {
                date = DateTime.Parse(strDate);
            }

            MovieTimesViewModel model = new MovieTimesViewModel();
            model.MovieList = _movieManager.GetMovieTimes(DomainContext.Domain.Id, DomainContext.AppId, date);
            model.Settings = _movieManager.GetSettings(DomainContext.Domain.Id, DomainContext.AppId);
            model.JsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            model.JsApiConfig.JsApiList.Add("onMenuShareTimeline");
            model.JsApiConfig.JsApiList.Add("onMenuShareAppMessage");

            return View(model);
        }

        [AllowedAnonymous]
        public ActionResult Information(string domainId)
        {
            string strInformationId = Request.QueryString["informationId"];
            Guid informationId = Guid.Empty;
            if (String.IsNullOrEmpty(strInformationId) || Guid.TryParse(strInformationId, out informationId) == false)
            {
                return RedirectToAction("Portal", new { domainId = domainId });
            }

            InformationViewModel model = new InformationViewModel();
            model.Information = _informationManager.GetInformation(informationId);
            return View(model);
        }

        [AllowedAnonymous]
        public ActionResult InformationItemList(string domainId)
        {
            string strInformationId = Request.QueryString["informationId"];
            Guid informationId = Guid.Empty;
            if (String.IsNullOrEmpty(strInformationId) || Guid.TryParse(strInformationId, out informationId) == false)
            {
                return RedirectToAction("Portal", new { domainId = domainId });
            }

            string strCategoryId = Request.QueryString["categoryId"];
            Guid categoryId = Guid.Empty;
            if (String.IsNullOrEmpty(strCategoryId) || Guid.TryParse(strCategoryId, out categoryId) == false)
            {
                return RedirectToAction("Portal", new { domainId = domainId });
            }

            InformationCategoryViewModel model = new InformationCategoryViewModel();
            model.Information = _informationManager.GetInformation(informationId);
            model.Category = _informationManager.GetCategory(categoryId);
            return View(model);
        }

        [AllowedAnonymous]
        public ActionResult InfomationItem(string domainId)
        {
            string strItemId = Request.QueryString["itemId"];
            Guid itemId = Guid.Empty;
            if (String.IsNullOrEmpty(strItemId) || Guid.TryParse(strItemId, out itemId) == false)
            {
                return RedirectToAction("Portal", new { domainId = domainId });
            }

            InfomationItemViewModel model = new InfomationItemViewModel();
            model.Item = _informationManager.GetInformationItem(itemId);

            if (model.Item == null)
            {
                return RedirectToAction("Portal", new { domainId = domainId });
            }

            model.Information = _informationManager.GetInformation(model.Item.Information);
            return View(model);
        }

        public ActionResult CustomForm(string domainId)
        {
            string strFormId = Request.QueryString["formId"];
            Guid formId = Guid.Empty;
            if (String.IsNullOrEmpty(strFormId) || Guid.TryParse(strFormId, out formId) == false)
            {
                return RedirectToAction("Portal", new { domainId = domainId });
            }

            CustomFormViewModel model = new CustomFormViewModel();
            model.CustomForm = _customFormManager.GetCustomForm(formId);
            model.Content = _customFormManager.GetCustomFormContentByMember(formId, this.OpenId);
            model.Member = MemberContext.Member;

            model.JsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            model.JsApiConfig.JsApiList.Add("onMenuShareTimeline");
            model.JsApiConfig.JsApiList.Add("onMenuShareAppMessage");

            return View(model);
        }

        public ActionResult Coupon(string domainId)
        {
            return View();
        }

        public ActionResult CouponDetail(string domainId)
        {
            string strRecordId = Request.QueryString["recordId"];
            Guid recordId = Guid.Empty;
            if (String.IsNullOrEmpty(strRecordId) || Guid.TryParse(strRecordId, out recordId) == false)
            {
                return RedirectToAction("Portal", new { domainId = domainId });
            }

            CouponDetailViewModel model = new CouponDetailViewModel();
            model.Record = CouponManager.Instance.CouponRecord(recordId);
            if (model.Record == null)
            {
                return RedirectToAction("Portal", new { domainId = domainId });
            }
            model.Coupon = CouponManager.Instance.GetCoupon(model.Record.Coupon);
            return View(model);
        }

        /// <summary>
        /// 自定义页面落地
        /// </summary>
        /// <returns></returns>
        [AllowedOnlyOpenId]
        public ActionResult Page(string domainId)
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RedirectToAction("Portal", new { domainId = domainId });
            }

            PageEntity page = PageManager.Instance.GetPage(id);

            if (page == null)
            {
                return RedirectToAction("Portal", new { domainId = domainId });
            }

            PageManager.Instance.PageVisit(id);

            PageViewModel model = new PageViewModel();
            model.Page = page;
            model.JsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            model.JsApiConfig.JsApiList.Add("onMenuShareTimeline");
            model.JsApiConfig.JsApiList.Add("onMenuShareAppMessage");

            return View(model);
        }

        /// <summary>
        /// 增强图文
        /// </summary>
        /// <param name="domainId"></param>
        /// <returns></returns>
        [AllowedOnlyOpenId]
        public ActionResult AdvancedArticle(string domainId)
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RedirectToAction("Portal", new { domainId = domainId });
            }

            AdvancedArticleEntity advancedArticle = AdvancedArticleManager.Instance.GetAdvancedArticle(id);

            if (advancedArticle == null)
            {
                return RedirectToAction("Portal", new { domainId = domainId });
            }

            AdvancedArticleManager.Instance.AdvancedArticleVisit(id);

            AdvancedArticleViewModel model = new AdvancedArticleViewModel();
            model.Article = advancedArticle;

            if (advancedArticle.Advertising.HasValue)
            {
                model.Advertising = _advertisingManager.GetAdvertising(advancedArticle.Advertising.Value);
            }

            WeixinJsApiConfig jsApiConfig = new WeixinJsApiConfig();
            jsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            jsApiConfig.JsApiList.Add("onMenuShareTimeline");
            jsApiConfig.JsApiList.Add("onMenuShareAppMessage");
            ViewBag.JsApiConfig = jsApiConfig;

            return View(model);
        }

        public ActionResult RecommendUrl(string domainId)
        {
            RecommendUrlViewModel model = new RecommendUrlViewModel();
            model.RecommendUrl = _recommendUrlManager.Get(DomainContext, MemberContext.Member.Id);
            model.Level1DownlineCount = _recommendUrlManager.GetLevel1DownlineCount(MemberContext.Member.Id);
            model.Level2DownlineCount = _recommendUrlManager.GetLevel2DownlineCount(MemberContext.Member.Id);
            model.Settings = _recommendUrlManager.GetSettings(DomainContext.Domain.Id, DomainContext.AppId);

            WeixinJsApiConfig jsApiConfig = new WeixinJsApiConfig();
            jsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            jsApiConfig.JsApiList.Add("onMenuShareTimeline");
            jsApiConfig.JsApiList.Add("onMenuShareAppMessage");
            ViewBag.JsApiConfig = jsApiConfig;

            return View(model);
        }

        public ActionResult Redpack(string domainId)
        {
            return View();
        }

        /*
         * 关于引导关注页面
         * 有三个级别，默认引导关注页面，域引导关注页面，活动引导关注页
         * 如果在后台代码中统一处理，有一个问题在于，前端页面使用的地址和后台redirect需要的地址是不一样的
         * 前端如果要用相对地址跳转，类似于“/Home/Index” ，后台 Redirect 则是 “~/Home/Index”
         * 所以无法统一处理，考虑前后端统一全部跳到 Home/GuideSubscribe 或 Home/CampaignGuideSubscribe ，
         * 再在这个方法中用 redirect 重定向
         * 但这样依然有一个问题，就是对于活动的引导页面，如果活动页面必须关注才能访问
         * 则后面action直接在基类就跳到默认引导关注页面了，这一点考虑在控制器基类看url后面有没有campaignid这个参数
         * 有的话跳到活动专用的引导关注页
         */

        /// <summary>
        /// 提示关注页面
        /// 跳转到默认引导关注页面或域引导关注页面
        /// </summary>
        /// <returns></returns>
        [AllowedAnonymous]
        public ActionResult GuideSubscribe(string domainId)
        {
            //如果没有设置引导页面则显示一个公共引导关注页面
            //否则引导到用户设置的引导关注页面
            if (String.IsNullOrEmpty(DomainContext.GuideSubscribeUrl))
            {
                GuideSubscribeViewModel model = new GuideSubscribeViewModel();
                model.Authorizer = DomainContext.Authorizer;
                return View(model);
            }
            else
            {
                return new RedirectResult(DomainContext.GuideSubscribeUrl);
            }
        }

        /// <summary>
        /// 提示关注页面
        /// 先尝试跳转到活动引导关注页面，如果没有，则重定向到 GuideSubscribe 这个 Action
        /// </summary>
        /// <param name="domainId"></param>
        /// <returns></returns>
        [AllowedAnonymous]
        public ActionResult CampaignGuideSubscribe(string domainId)
        {
            string strCampaignId = Request.QueryString["campaignId"];

            if (String.IsNullOrEmpty(strCampaignId))
            {
                return RedirectToAction("GuideSubscribe", new { domainId = DomainContext.Domain.Id });
            }

            Guid campaignId = Guid.Parse(strCampaignId);
            CampaignEntity campaign = _campaignManager.GetCampaign(campaignId);
            if (campaign == null || campaign.Status == EnumCampaignStatus.Preparatory || String.IsNullOrEmpty(campaign.GuideSubscribeUrl))
            {
                return RedirectToAction("GuideSubscribe", new { domainId = DomainContext.Domain.Id });
            }

            return new RedirectResult(campaign.GuideSubscribeUrl);
        }

        /// <summary>
        /// 错误页面
        /// </summary>
        /// <returns></returns>
        [AllowedAnonymous]
        public ActionResult ErrorView(string domainId)
        {
            //存在没有domainId的可能

            ErrorViewModel model = new ErrorViewModel();

            string messageKey = Request.QueryString["message"];
            if (String.IsNullOrEmpty(messageKey))
                model.Message = "未知错误";
            else
                model.Message = _errorMessage.GetMessage(messageKey);

            return View(model);
        }

        /// <summary>
        /// 无实际功能，菜单中的管理后台Action如果不是管理员点了跳到这里来
        /// 作为一个广告页面进行展示
        /// </summary>
        /// <returns></returns>
        [AllowedAnonymous]
        public ActionResult StaffLogin()
        {
            return View();
        }

        /// <summary>
        /// 生成的二维码将跳转到此页面，通过此页面做统一的二次跳转
        /// http://appid.wxc.shengxunwei.com/Home/QRCode/domainId?type=member&memberId=1
        /// type=member 是必须的后面的参与根据 type 的不同有所不同
        /// </summary>
        /// <returns></returns>
        public ActionResult QRCode(string domainId, string type)
        {
            switch (type)
            {
                case "member":
                    //string memberId = Request.QueryString["memberId"];
                    string cardNumber = Request.QueryString["cardNumber"];
                    return new RedirectResult(String.Format(
                        "~/Staff/Home/MemberInfo/{0}?cardNumber={1}", DomainContext.Domain.Id, cardNumber));
                case "coupon":
                    string serialNumber = Request.QueryString["serialNumber"];
                    return new RedirectResult(String.Format(
                        "~/Staff/Home/CouponRecordDetail/{0}?serialNumber={1}", DomainContext.Domain.Id, serialNumber));
                default:
                    return RedirectToAction("Portal", new { domainId = domainId });
            }
        }

        [AllowedAnonymous]
        public ActionResult AdvertisingLanding(string domainId)
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RedirectToAction("Portal", new { domainId = domainId });
            }

            _advertisingManager.AdvertisingClick(id);

            string strUrl = Request.QueryString["url"];
            if (String.IsNullOrEmpty(strUrl))
            {
                return RedirectToAction("Portal", new { domainId = domainId });
            }

            return new RedirectResult(Server.UrlDecode(strUrl));
        }

        #region PointCommondity

        //如果不保留这几个 Action ，BasalController 中的 OnActionExecuting 是根本不会进去的
        //因为找不到 Action 自然也就不存在 OnActionExecuting 这样的逻辑

        public ActionResult PointCommodity(string domainId)
        {
            return new HttpStatusCodeResult(301);
        }

        public ActionResult PointCommodityDetail(string domainId)
        {
            return new HttpStatusCodeResult(301);
        }

        public ActionResult PointCommodityOrderList(string domainId)
        {
            return new HttpStatusCodeResult(301);
        }

        public ActionResult PointCommodityOrderDetail(string domainId)
        {
            return new HttpStatusCodeResult(301);
        }

        #endregion
    }
}