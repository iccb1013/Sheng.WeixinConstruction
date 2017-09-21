using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Areas.Api
{
    public class ApiAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Api";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Api_default",
                "Api/{controller}/{action}/{domainId}",
                defaults: new { controller = "Home", action = "Portal", domain = UrlParameter.Optional },
                namespaces: new[] { "Sheng.WeixinConstruction.Client.Shell.Areas.Api.Controllers" }
            );
        }
    }
}