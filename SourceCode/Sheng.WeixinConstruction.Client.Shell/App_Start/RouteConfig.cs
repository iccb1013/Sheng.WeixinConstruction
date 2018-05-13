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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sheng.WeixinConstruction.Client.Shell
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //http://www.bitscn.com/pdb/dotnet/201408/308221.html
            //http://www.oschina.net/question/565065_68448

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //作为第三方平台运营时，AppId 会作为URL的一部分带过来
            //http://wxc.shengxunwei.com/ThirdPartyWeixinApi/Handler/$APPID$
            //其中$APPID$在实际推送时会替换成所属的已授权公众号的appid。
            //如果独立运行，微信后台的推送URL把APPID写死
            //routes.MapRoute(
            //    name: "ThirdPartyWeixinApi",
            //    url: "{controller}/{action}/{appId}",
            //    defaults: new { controller = "ThirdPartyWeixinApi", action = "Handler", appId = UrlParameter.Optional },
            //    namespaces: new[] { "Sheng.WeixinConstruction.Client.Shell.Controllers" }
            //);

            //默认路由，后面带是带 domainId，用于所有自有页面
            //如 HomeController
            //如果不加 domain = UrlParameter.Optional，那么在访问所有页面时
            //都必须有 domain 参数，否则404
            //默认转到官网
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{domainId}",
                defaults: new { controller = "Home", action = "WebSite", domainId = UrlParameter.Optional },
                namespaces: new[] { "Sheng.WeixinConstruction.Client.Shell.Controllers" }
            );
        }
    }
}
