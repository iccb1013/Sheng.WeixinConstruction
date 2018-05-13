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
using Sheng.WeixinConstruction.Client.Shell.Controllers;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Sheng.WeixinConstruction.Client.Shell
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private LogService _logService = LogService.Instance;
        private ExceptionHandlingService _exceptionHandling = ExceptionHandlingService.Instance;

        protected void Application_Start()
        {

            //string strUri = "http://wx000.wxc.shengxunwei.com/Campaign/PictureVote/111";
            //Uri uri = new Uri(strUri);
            

            _logService.Write("Application_Start");

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //ASP.NET MVC3 异常处理 摘抄
        //http://blog.csdn.net/kufeiyun/article/details/8191673
        //为全局提供未捕获的异常处理，和用于 Controller 的过滤器是互补的
        //由MVC控制器调用所引发的异常将由其 Filter 处理，handled以后不会抛到这里
        //而由程序运行过程中自身产生的异常，在没有处理的情况下会一路抛到此处
        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            Server.ClearError();

            _exceptionHandling.HandleException(exception);
        }

        //protected void Session_Start(object sender, EventArgs e)
        //{
            
        //}

        //protected void Session_End(Object sender, EventArgs e)
        //{
            
        //}
    }
}
