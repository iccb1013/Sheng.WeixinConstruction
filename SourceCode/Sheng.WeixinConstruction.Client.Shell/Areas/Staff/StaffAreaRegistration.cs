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