using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Admin.Shell.Areas.Api
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
                "Api/{controller}/{action}",
                new { action = "Index" },
                namespaces: new[] { "Sheng.WeixinConstruction.Admin.Shell.Areas.Api.Controllers" }
            );
        }
    }
}