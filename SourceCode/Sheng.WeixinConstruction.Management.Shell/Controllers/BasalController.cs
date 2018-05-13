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


using Linkup.Common;
using Newtonsoft.Json;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Management.Core;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sheng.WeixinConstruction.Management.Shell
{
    public class BasalController : Controller
    {
        private static readonly ControlledCachingService _controlledCachingService = ControlledCachingService.Instance;
        private static readonly ManagementDomainPool _domainPool = ManagementDomainPool.Instance;

        public UserContext UserContext
        {
            get;
            set;
        }

        public ManagementDomainContext DomainContext
        {
            get;
            set;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            object[] objAllowedAnonymousArray =
                filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowedAnonymous), false);
            if (objAllowedAnonymousArray.Length > 0)
                return;

            UserContext = SessionContainer.GetUserContext(filterContext.HttpContext);

            if (UserContext == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
                return;
            }

            _controlledCachingService.Set(UserContext.User.Id.ToString(), null, 1200);
            DomainContext = _domainPool.GetDomainContext(UserContext.User.Domain);

            ViewBag.User = UserContext.User;
            ViewBag.Domain = DomainContext.Domain;
            ViewBag.DomainContext = DomainContext;
        }

        protected TObj RequestArgs<TObj>() where TObj : class
        {
            StreamReader reader = new StreamReader(HttpContext.Request.InputStream);
            //这里不能用HttpUtility.UrlDecode，会把 + 号忽略掉！
            //例如日期时间序列化后会变为：
            //Date(1412957280374+0800)
            string json = reader.ReadToEnd();//HttpUtility.UrlDecode(reader.ReadToEnd());

            //TObj requestObj = JsonConvert.DeserializeObject<TObj>(json);
            TObj requestObj = JsonHelper.NewtonsoftDeserialize<TObj>(json);
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
            //result.Content = Newtonsoft.Json.JsonConvert.SerializeObject(apiResult);
            result.Content = JsonHelper.NewtonsoftSerializer(apiResult);
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