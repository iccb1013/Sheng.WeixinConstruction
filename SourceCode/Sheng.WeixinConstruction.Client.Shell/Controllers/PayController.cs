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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Controllers
{
    /// <summary>
    /// 现金帐户
    /// </summary>
    public class PayController : ClientBasalController
    {
        private static readonly PayManager _payManager = PayManager.Instance;

        public ActionResult PayOrderList(string domainId)
        {
            return View();
        }

        /// <summary>
        /// action:1 页面加载后直接发起支付
        /// </summary>
        /// <param name="domainId"></param>
        /// <returns></returns>
        public ActionResult PayOrderDetail(string domainId)
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RedirectToAction("PayOrderList", new { domainId = domainId });
            }

            PayOrderEntity payOrder = _payManager.GetPayOrder(id);
            if (payOrder == null)
            {
                return RedirectToAction("PayOrderList", new { domainId = domainId });
            }
            return View(payOrder);
        }

        public ActionResult CashAccountTrack(string domainId)
        {
            return View();
        }

        public ActionResult Deposit(string domainId)
        {
            PayViewModel model = new PayViewModel();
            return View(model);
        }
    }
}