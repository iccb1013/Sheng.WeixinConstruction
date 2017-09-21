using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell
{
    /// <summary>
    /// 使用完全匿名的浏览，连openId都不取
    /// </summary>
    public class AllowedAnonymous : ActionFilterAttribute
    {

    }
}