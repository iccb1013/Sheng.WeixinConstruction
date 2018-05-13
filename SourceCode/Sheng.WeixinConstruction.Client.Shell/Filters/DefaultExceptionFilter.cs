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
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell
{
    //asp.net MVC3 仿照博客园功能 异常处理
    //http://www.cnblogs.com/jiagoushi/archive/2012/12/20/2827273.html

    /// <summary>
    /// 这个异常处理过滤器是针对 Controller 的
    /// 仅为 Controller 提供默认的异常捕获和处理，不是全局的异常处理与捕获
    /// 用于全局的在Application_Error
    /// </summary>
    public class DefaultExceptionFilter : HandleErrorAttribute, IExceptionFilter
    {
        private static readonly ExceptionHandlingService _exceptionHandling =
            ExceptionHandlingService.Instance;

        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            //UrlHelper url = new UrlHelper(filterContext.RequestContext);
            //filterContext.Result = new RedirectResult(url.Action("AboutError", "AboutError"));

            Exception exception = filterContext.Exception;

            _exceptionHandling.HandleException(exception);

            filterContext.Result = new HttpStatusCodeResult(500);

            filterContext.ExceptionHandled = true;
        }
    }
}