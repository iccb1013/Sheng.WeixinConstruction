using Sheng.WeixinConstruction.Container.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Container.Controllers
{
    public class ControlledCacheController : BasalController
    {
        private static readonly ControlledCachePool _controlledCachePool = ControlledCachePool.Instance;

        public ActionResult Contains()
        {
            string key = Request.QueryString["key"];
            return new ContentResult()
            {
                ContentEncoding = Encoding.ASCII,
                Content = _controlledCachePool.Contains(key).ToString()
            };
        }

        public ActionResult Set()
        {
            string key = Request.QueryString["key"];
            string data = Request.QueryString["data"];
            string seconds = Request.QueryString["seconds"];
            _controlledCachePool.Set(key, data, int.Parse(seconds));
            return new HttpStatusCodeResult(200);
        }

        public ActionResult Get()
        {
            string key = Request.QueryString["key"];
            return new ContentResult()
            {
                ContentEncoding = Encoding.ASCII,
                Content = _controlledCachePool.Get(key).ToString()
            };
        }

        public ActionResult Remove()
        {
            string key = Request.QueryString["key"];
            _controlledCachePool.Remove(key);
            return new HttpStatusCodeResult(200);
        }

        public ActionResult ExtendExpiryTime()
        {
            string key = Request.QueryString["key"];
            string seconds = Request.QueryString["seconds"];
            _controlledCachePool.ExtendExpiryTime(key, int.Parse(seconds));
            return new HttpStatusCodeResult(200);
        }

        public ActionResult Monitor()
        {
            ControlledCacheMonitorViewModel model = new ControlledCacheMonitorViewModel();
            model.List = _controlledCachePool.GetList();
            return View(model);
        }
    }
}