using Linkup.Common;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.FileService
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

            //如果是 WrappedException ，就不再重复记录日志了
            if ((exception is WrappedException) == false)
            {
                _exceptionHandling.HandleException(exception);
            }

            filterContext.Result = new HttpStatusCodeResult(500);

            filterContext.ExceptionHandled = true;
        }
    }
}