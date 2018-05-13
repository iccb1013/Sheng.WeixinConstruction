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
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Controllers
{
    public class HomeController : BasalController
    {
        private static readonly CachingService _cachingService = CachingService.Instance;

        [AllowedAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            _cachingService.Remove(UserContext.User.Id.ToString());

            SessionContainer.ClearUserContext(this.HttpContext);

            return RedirectToAction("Login");
        }

        [AllowedAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowedAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

      

    }
}