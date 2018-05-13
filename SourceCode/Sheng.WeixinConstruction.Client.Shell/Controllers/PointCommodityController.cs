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
    public class PointCommodityController : ClientBasalController
    {
        private static readonly CampaignManager _campaignManager = CampaignManager.Instance;
        private static readonly PointCommodityManager _pointCommodityManager = PointCommodityManager.Instance;
        private static readonly InformationManager _informationManager = InformationManager.Instance;
        private static readonly CustomFormManager _customFormManager = CustomFormManager.Instance;
        private static readonly MovieManager _movieManager = MovieManager.Instance;
        private static readonly ErrorMessage _errorMessage = ErrorMessage.Instance;
        private static readonly PortalTemplatePool _portalTemplatePool = PortalTemplatePool.Instance;

        public ActionResult PointCommodity(string domainId)
        {
            return View();
        }

        public ActionResult PointCommodityDetail(string domainId)
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RedirectToAction("PointCommodity", new { domainId = domainId });
            }

            PointCommodityDetailViewModel model = new PointCommodityDetailViewModel();
            model.PointCommodity = _pointCommodityManager.GetPointCommodity(id);
            if (model.PointCommodity == null)
            {
                return RedirectToAction("PointCommodity", new { domainId = domainId });
            }
            model.ShoppingCartPointCommodityCount = _pointCommodityManager.GetShoppingCartPointCommodityCount(
                this.DomainContext, MemberContext.Member.Id);

            return View(model);
        }

        public ActionResult OrderList(string domainId)
        {
            return View();
        }

        public ActionResult OrderDetail(string domainId)
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RedirectToAction("PointCommodity", new { domainId = domainId });
            }

            PointCommodityOrderDetailViewModel model = new PointCommodityOrderDetailViewModel();
            model.Order = _pointCommodityManager.GetOrder(id);
            if (model.Order == null)
            {
                return RedirectToAction("PointCommodity", new { domainId = domainId });
            }
            model.ItemList = _pointCommodityManager.GetOrderItemList(id);
            model.LogList = _pointCommodityManager.GetOrderLogList(id);
            return View(model);
        }

        /// <summary>
        /// 购物车
        /// </summary>
        /// <returns></returns>
        public ActionResult ShoppingCart()
        {
            return View();
        }

        /// <summary>
        /// 结算
        /// </summary>
        /// <returns></returns>
        public ActionResult Checkout()
        {
            return View();
        }

       
    }
}