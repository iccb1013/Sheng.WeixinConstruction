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
using Sheng.WeixinConstruction.Client.Core;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sheng.WeixinConstruction.Client.Shell
{
    public class BasalController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            string url = Request.Url.ToString().ToLower();

            //域名变更
            if (url.Contains("zkebao"))
            {
                filterContext.Result = new RedirectResult(url.Replace("zkebao", "shengxunwei"));
                return;
            }

            //积分商城几个页面从 HomeController 下面移到 PointCommodityController 下
            if (url.Contains(@"home/pointcommoditydetail"))
            {
                filterContext.Result = new RedirectResult(url.Replace(@"home/pointcommoditydetail", @"PointCommodity/PointCommodityDetail"));
                return;
            }

            if (url.Contains(@"home/pointcommodityorderlist"))
            {
                filterContext.Result = new RedirectResult(url.Replace(@"home/pointcommodityorderlist", @"PointCommodity/OrderList"));
                return;
            }

            if (url.Contains(@"home/pointcommodityorderdetail"))
            {
                filterContext.Result = new RedirectResult(url.Replace(@"home/pointcommodityorderdetail", @"PointCommodity/OrderDetail"));
                return;
            }

            //这条必须放在最后，否则上面的任意一条都能匹配上这一条
            if (url.Contains(@"home/pointcommodity"))
            {
                filterContext.Result = new RedirectResult(url.Replace(@"home/pointcommodity", @"PointCommodity/PointCommodity"));
                return;
            }
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
            // result.Content = Newtonsoft.Json.JsonConvert.SerializeObject(apiResult);
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