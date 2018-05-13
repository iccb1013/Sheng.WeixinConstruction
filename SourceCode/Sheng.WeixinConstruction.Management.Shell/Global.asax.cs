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
using Newtonsoft.Json;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Management.Core;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Sheng.WeixinConstruction.Management.Shell
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private LogService _logService = LogService.Instance;
        private ExceptionHandlingService _exceptionHandling = ServiceUnity.Instance.ExceptionHandling;
        private Campaign_LotteryDraw _campaign_LotteryDraw = Campaign_LotteryDraw.Instance;

        protected void Application_Start()
        {
            //确保第一时间调用  SetLogWriter 方法，否则如果 ExceptionHandlingService 先于它初始化
            //则会报错
            _logService.Write("Application_Start");

            //初始化微信 返回代码对应的错误信息
            WeixinErrorCode.Instance.GetMessage(0);

            LimitedRequestHelper limitedRequestHelper = LimitedRequestHelper.Instance;
            limitedRequestHelper.AddTarget("SMS", 30, 3); //发送短信

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //TextMessage msg = new TextMessage();
            //msg.Content = "Hellow";
            //string strJson = JsonConvert.SerializeObject(msg);
            //string strXml = MessageHelper.XmlSerialize(msg);
        }

        //ASP.NET MVC3 异常处理 摘抄
        //http://blog.csdn.net/kufeiyun/article/details/8191673
        //为全局提供未捕获的异常处理，和用于 Controller 的过滤器是互补的
        //由MVC控制器调用所引发的异常将由其 Filter 处理，handled以后不会抛到这里
        //而由程序运行过程中自身产生的异常，在没有处理的情况下会一路抛到此处
        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            if (exception == null)
                return;

            Server.ClearError();
            
            _exceptionHandling.HandleException(exception);

        }
    }
}
