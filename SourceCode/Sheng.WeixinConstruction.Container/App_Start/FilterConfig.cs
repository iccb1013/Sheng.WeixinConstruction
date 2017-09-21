using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Container
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new DefaultExceptionFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
