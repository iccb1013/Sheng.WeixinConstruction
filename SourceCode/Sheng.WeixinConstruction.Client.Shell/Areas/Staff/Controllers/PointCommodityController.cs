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


using Sheng.WeixinConstruction.Client.Shell.Areas.Staff.Models;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Areas.Staff.Controllers
{
    public class PointCommodityController : ClientBasalController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //base中已经处理了
            if (filterContext.Result != null)
                return;

            //获取关联的User对象
            MemberContext.User = _userManager.GetUserByMemberId(
                DomainContext.Domain.Id, MemberContext.Member.Id);

            if (MemberContext == null || MemberContext.User == null)
            {
             //   filterContext.Result = new RedirectResult("~/Home/StaffLogin/" + DomainContext.Domain.Id);
                filterContext.Result = new RedirectResult("http://wxcm.shengxunwei.com/M/Home/Portal");
                return;
            }
        }

        public ActionResult PointCommodityOrder()
        {
            return View();
        }
	}
}