using Linkup.Common;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Sheng.WeixinConstruction.Container
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //确保log在ExceptionHandlingService之前初始化，否则出错
        private LogService _logService = LogService.Instance;
        private ExceptionHandlingService _exceptionHandling = ExceptionHandlingService.Instance;

        private ThirdPartyAccessToken _thirdPartyAccessToken = ThirdPartyAccessToken.Instance;
        //初始化数据
        private AuthorizerAccessTokenPool _authorizerAccessTokenPool = AuthorizerAccessTokenPool.Instance;

        protected void Application_Start()
        {
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

            if ((exception is WrappedException) == false)
            {
                _exceptionHandling.HandleException(exception);
            }
        }
    }
}
