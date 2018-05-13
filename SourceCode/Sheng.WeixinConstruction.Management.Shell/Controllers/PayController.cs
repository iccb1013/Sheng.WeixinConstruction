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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Controllers
{
    public class PayController : BasalController
    {
        private static readonly OneDollarBuyingManager _oneDollarBuyingManager = OneDollarBuyingManager.Instance;


        #region 1元抢购

        public ActionResult OneDollarBuyingCommodity()
        {
            return View();
        }

        public ActionResult OneDollarBuyingCommodityEdit()
        {
            return View();
        }

        public ActionResult OneDollarBuyingCommodityForSaleList()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                //TODO:ID无效
                //return RedirectToAction("PointCommodity", new { domain = domain });
            }

            OneDollarBuyingCommodityForSaleListViewModel model = new OneDollarBuyingCommodityForSaleListViewModel();
            model.Commodity = _oneDollarBuyingManager.GetCommodity(id);
            return View(model);
        }

        public ActionResult OneDollarBuyingDealInfo()
        {
            string strSaleId = Request.QueryString["saleId"];
            Guid saleId = Guid.Empty;
            if (String.IsNullOrEmpty(strSaleId) || Guid.TryParse(strSaleId, out saleId) == false)
            {
                //TODO:ID无效
                //return RedirectToAction("PointCommodity", new { domain = domain });
            }

            OneDollarBuyingDealInfoViewModel model = new OneDollarBuyingDealInfoViewModel();
            model.DealInfo = _oneDollarBuyingManager.GetDealInfo(saleId);
            return View(model);
        }

        #endregion

       
    }
}