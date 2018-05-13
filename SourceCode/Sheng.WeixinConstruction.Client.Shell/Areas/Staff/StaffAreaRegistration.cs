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

namespace Sheng.WeixinConstruction.Client.Shell.Areas.Staff
{
    public class StaffAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Staff";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Staff_default",
                "Staff/{controller}/{action}/{domainId}",
                defaults: new { controller = "Staff", action = "Portal", domain = UrlParameter.Optional },
                namespaces: new[] { "Sheng.WeixinConstruction.Client.Shell.Areas.Staff.Controllers" }
            );
        }
    }
}