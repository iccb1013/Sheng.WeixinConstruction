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


using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Management.Shell.Models;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Controllers
{
    public class PortalController : BasalController
    {
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly MemberManager _memberManager = MemberManager.Instance;
        private static readonly InformationManager _informationManager = InformationManager.Instance;
        private static readonly CustomFormManager _customFormManager = CustomFormManager.Instance;
        private static readonly SettingsManager _settingsManager = SettingsManager.Instance;
        private static readonly PortalTemplatePool _portalTemplatePool = PortalTemplatePool.Instance;
        private static readonly PortalManager _portalManager = PortalManager.Instance;
        private static readonly MovieManager _movieManager = MovieManager.Instance;
        private static readonly PointCommodityManager _pointCommodityManager = PointCommodityManager.Instance;

        public ActionResult Dashboard()
        {
            DashboardViewModel model = new DashboardViewModel();
            model.MemberStatisticData = _memberManager.GetMemberStatisticData(
                DomainContext.Domain.Id, DomainContext.AppId, DateTime.Now, DateTime.Now);
            model.PointCommodityOrderStatisticData =_pointCommodityManager.GetPointCommodityOrderStatisticData(
                DomainContext.Domain.Id, DomainContext.AppId, DateTime.Now, DateTime.Now);

            return View(model);
        }

        public ActionResult GetVip()
        {
            return View();
        }

        public ActionResult GetPay()
        {
            return View();
        }

        #region 微主页

        public ActionResult PortalStyle()
        {
            if (DomainContext.Online == false)
            {
                return RedirectToAction("Docking", "Settings");
            }

            if (DomainContext.StyleSettings.PortalMode == EnumPortalMode.Template)
            {
                return RedirectToAction("PortalStyle_Template");
            }
            else
            {
                return RedirectToAction("PortalStyle_Custom");
            }
        }

        public ActionResult PortalStyle_Template()
        {
            if (DomainContext.Online == false)
            {
                return RedirectToAction("Docking");
            }

            PortalStyle_TemplateViewModel model = new PortalStyle_TemplateViewModel();
            if (String.IsNullOrEmpty(DomainContext.AppId) == false)
                model.Settings = _portalManager.GetPortalStyleSettings(UserContext.User.Domain, DomainContext.AppId);
            else
            {
                model.Settings = new PortalStyleSettingsEntity();
            }

            //if (model.Settings.PortalPresetTemplateId.HasValue)
            //{
            //    model.Template = _portalTemplatePool.GetPortalPresetTemplateDigest(model.Settings.PortalPresetTemplateId.Value);
            //}
            //else
            //{
            //    model.Template = _portalTemplatePool.GetDefaultPortalPresetTemplate();
            //}

            return View(model);
        }

        public ActionResult PortalStyle_Custom()
        {
            if (DomainContext.Online == false)
            {
                return RedirectToAction("Docking");
            }

            string strImportTemplateId = Request.QueryString["importTemplateId"];
            Guid importTemplateId = Guid.Empty;
            if (String.IsNullOrEmpty(strImportTemplateId) == false)
            {
                Guid.TryParse(strImportTemplateId, out importTemplateId);
            }

            PortalStyle_CustomViewModel model = new PortalStyle_CustomViewModel();
            if (importTemplateId == Guid.Empty)
            {
                PortalStyleSettingsEntity settings =
                   _portalManager.GetPortalStyleSettings(UserContext.User.Domain, DomainContext.AppId);
                model.CustomTemplate = settings.PortalCustomTemplate;
            }
            else
            {
                PortalPresetTemplateEntity templateEntity = _portalTemplatePool.GetPortalPresetTemplate(importTemplateId);
                if (templateEntity != null)
                {
                    model.CustomTemplate = templateEntity.Template;

                    string portalImageUrl = Request.QueryString["portalImageUrl"];
                    if (String.IsNullOrEmpty(portalImageUrl) == false)
                    {
                        model.CustomTemplate = model.CustomTemplate.Replace("{{#PortalImageUrl}}", Server.UrlDecode(portalImageUrl));
                    }
                }
            }

            return View(model);
        }

        public ActionResult PortalTemplateSelect()
        {
            PortalTemplateSelectViewModel model = new PortalTemplateSelectViewModel();
            model.TemplateList = _portalTemplatePool.GetPortalPresetTemplateList();
            return View(model);
        }

        public ActionResult PortalTemplatePreview()
        {
            return View();
        }

        #endregion

        #region 菜单

        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult MenuEdit()
        {
            return View();
        }

        #endregion

        #region 快捷菜单

        public ActionResult ShortcutMenu()
        {
            return View();
        }

        public ActionResult ShortcutMenuEdit()
        {
            return View();
        }

        #endregion

        #region 自定义页面

        public ActionResult Page()
        {
            return View();
        }

        public ActionResult PageEdit()
        {
            ViewBag.PageUrl = String.Format("{0}Home/Page/{1}",
                _settingsManager.GetClientAddress(DomainContext), DomainContext.Domain.Id);
            return View();
        }

        #endregion

        #region 分类信息

        public ActionResult Information()
        {
            return View();
        }

        public ActionResult InformationEdit()
        {
            return View();
        }

        public ActionResult InformationCategory()
        {
            string strInformationId = Request.QueryString["informationId"];
            Guid informationId = Guid.Empty;
            if (String.IsNullOrEmpty(strInformationId) || Guid.TryParse(strInformationId, out informationId) == false)
            {
                //TODO:ID无效
                //return RedirectToAction("PointCommodity", new { domain = domain });
            }

            InformationEntity information = _informationManager.GetInformation(informationId);
            return View(information);
        }

        public ActionResult InformationCategoryEdit()
        {
            return View();
        }

        public ActionResult InformationItem()
        {
            string strInformationId = Request.QueryString["informationId"];
            Guid informationId = Guid.Empty;
            if (String.IsNullOrEmpty(strInformationId) || Guid.TryParse(strInformationId, out informationId) == false)
            {
                //TODO:ID无效
                //return RedirectToAction("PointCommodity", new { domain = domain });
            }

            string strCategoryId = Request.QueryString["categoryId"];
            Guid categoryId = Guid.Empty;
            if (String.IsNullOrEmpty(strCategoryId) || Guid.TryParse(strCategoryId, out categoryId) == false)
            {
                //TODO:ID无效
                //return RedirectToAction("PointCommodity", new { domain = domain });
            }

            InformationItemViewModel model = new InformationItemViewModel();
            model.Information = _informationManager.GetInformation(informationId);
            model.Category = _informationManager.GetCategory(categoryId);
            return View(model);
        }

        public ActionResult InformationItemEdit()
        {
            return View();
        }

        #endregion

        #region 自定义表单

        public ActionResult CustomForm()
        {
            return View();
        }

        public ActionResult CustomFormEdit()
        {
            return View();
        }

        public ActionResult CustomFormContent()
        {
            string strFormId = Request.QueryString["formId"];
            Guid formId = Guid.Empty;
            if (String.IsNullOrEmpty(strFormId) || Guid.TryParse(strFormId, out formId) == false)
            {
                //TODO:ID无效
                //return RedirectToAction("PointCommodity", new { domain = domain });
            }

            CustomFormContentViewModel model = new CustomFormContentViewModel();
            model.Form = _customFormManager.GetCustomForm(formId);
            return View(model);
        }

        #endregion

        #region 电影排片

        public ActionResult MovieTimes()
        {
            //没有绑定公众号的，初始化一个临时的设置对象用于UI显示
            //此时UI是不允许编辑的
            MovieTimesViewModel model = new MovieTimesViewModel();
            if (String.IsNullOrEmpty(DomainContext.AppId) == false)
                model.Settings = _movieManager.GetSettings(UserContext.User.Domain, DomainContext.AppId);
            else
                model.Settings = new MovieSettingsEntity();
            return View(model);
        }


        #endregion
    }
}