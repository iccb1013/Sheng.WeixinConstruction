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
using Sheng.WeixinConstruction.Management.Shell.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Controllers
{
    public class SystemController : BasalController
    {
        public ActionResult DomainType()
        {
            return View();
        }

        public ActionResult CurrentUserInfo()
        {
            CurrentUserInfoViewModel model = new CurrentUserInfoViewModel();
            model.User = UserContext.User;
            return View(model);
        }

        public ActionResult Password()
        {
            return View();
        }

        public ActionResult OperatedLog()
        {
            //

            //string url =
            //    ShortUrlApiWrapper.GetShortUrl(
            //    this.DomainContext,
            //    "http://wx0d5a4bc7bc94dbc5.wxc.shengxunwei.com/Campaign/ShakingLottery/36fac0ed-ce42-4533-a53a-2311c35ca566?campaignId=81513b1a-8f53-478b-b083-f0fa74a911bb").ApiResult.ShortUrl;

            //Sheng.WeixinConstruction.Service.ServiceUnity.Instance.Log.Write("URL", url, System.Diagnostics.TraceEventType.Error);

            return View();
        }

        #region 用户

        /// <summary>
        /// 不叫User因为会隐藏继承原有User成员
        /// </summary>
        /// <returns></returns>
        public ActionResult UserList()
        {
            return View();
        }

        public ActionResult UserEdit()
        {
            return View();
        }

        #endregion
    }
}