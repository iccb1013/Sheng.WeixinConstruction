using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Container
{
    /// <summary>
    /// 只有添加了这个属性的方法才允许远程访问，其它方法只允许localhost访问
    /// </summary>
    public class PublishAction : ActionFilterAttribute
    {

    }
}