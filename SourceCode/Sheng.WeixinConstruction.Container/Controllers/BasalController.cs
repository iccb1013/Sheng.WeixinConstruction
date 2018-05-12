using Newtonsoft.Json;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sheng.WeixinConstruction.Container
{
    public class BasalController : Controller
    {
        private static readonly string allowedIPList = System.Configuration.ConfigurationManager.AppSettings["AllowedIPList"];

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            string host = filterContext.HttpContext.Request.Url.Host;
            if (host != "localhost" && allowedIPList.Contains(host) == false)
            {
                object[] objAllowedAnonymousArray =
                filterContext.ActionDescriptor.GetCustomAttributes(typeof(PublishAction), false);
                if (objAllowedAnonymousArray.Length == 0)
                {
                    filterContext.Result = new HttpStatusCodeResult(404);
                }
            }
        }

        protected TObj RequestArgs<TObj>() where TObj : class
        {
            StreamReader reader = new StreamReader(HttpContext.Request.InputStream);
            //这里不能用HttpUtility.UrlDecode，会把 + 号忽略掉！
            //例如日期时间序列化后会变为：
            //Date(1412957280374+0800)
            string json = reader.ReadToEnd();//HttpUtility.UrlDecode(reader.ReadToEnd());

            TObj requestObj = JsonConvert.DeserializeObject<TObj>(json);
            return requestObj;
        }

        #region RespondResult

        protected virtual ContentResult RespondResult()
        {
            return RespondResult(true, null);
        }

        protected virtual ContentResult RespondResult(bool success, string message)
        {
            ApiResult apiResult = new ApiResult()
            {
                Success = success,
                Message = message
            };
            return RespondResult(apiResult);
        }

        protected virtual ContentResult RespondResult(ApiResult apiResult)
        {
            //JsonResult 无法序列化DataTable
            //JsonResult result = new JsonResult();
            //result.Data = apiResult;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;

            ContentResult result = new ContentResult();
            result.ContentEncoding = Encoding.UTF8;
            result.Content = Newtonsoft.Json.JsonConvert.SerializeObject(apiResult);
            return result;
        }

        protected virtual ContentResult RespondResult<T>(bool success, string message, T data)
        {
            ApiResult<T> apiResult = new ApiResult<T>()
            {
                Success = success,
                Message = message,
                Data = data
            };
            return RespondResult(apiResult);
        }

        protected virtual ContentResult RespondDataResult<T>(T data)
        {
            return RespondResult<T>(true, null, data);
        }

        #endregion
    }
}