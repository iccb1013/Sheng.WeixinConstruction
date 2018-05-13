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


using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Areas.M
{
    public class MAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "M";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "M_default",
                "M/{controller}/{action}",
                defaults: new { controller = "Home", action = "Login" },
                namespaces: new[] { "Sheng.WeixinConstruction.Management.Shell.Areas.M.Controllers" }
            );
        }
    }
}