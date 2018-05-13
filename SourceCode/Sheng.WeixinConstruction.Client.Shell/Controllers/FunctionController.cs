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


using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Controllers
{
    public class FunctionController : ClientBasalController
    {
        ///// <summary>
        ///// 临时短网址重定向
        ///// </summary>
        ///// <returns></returns>
        //[AllowedEmptyDomain]
        //public ActionResult ShortUrl()
        //{
        //    string key = Request.QueryString["k"];
        //    if (String.IsNullOrEmpty(key))
        //        return new HttpStatusCodeResult(404);

        //    string url = _cachingService.Get(key);
        //    if (String.IsNullOrEmpty(url))
        //        return new HttpStatusCodeResult(404);

        //    return new RedirectResult(url);
        //}

        /// <summary>
        /// RecomendUrl 落地页
        /// </summary>
        /// <returns></returns>
        [AllowedOnlyOpenId]
        public ActionResult RecommendUrl()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                //ID无效
                return new RedirectResult(String.Format("~/Home/GuideSubscribe/{0}", DomainContext.Domain.Id));
            }

            _recommendUrlManager.Log(id, this.OpenId);

            if (String.IsNullOrEmpty(DomainContext.RecommendUrlSettings.LandingUrl) == false)
            {
                return new RedirectResult(DomainContext.RecommendUrlSettings.LandingUrl);
            }
            else
            {
                return new RedirectResult(String.Format("~/Home/GuideSubscribe/{0}", DomainContext.Domain.Id));
            }
        }
    }
}