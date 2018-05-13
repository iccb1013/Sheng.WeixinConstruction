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


using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Client.Core;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Areas.Api.Controllers
{
    public class ApiBasalController : ClientBasalController
    {
        protected static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            /*
             * 在该域名和符合要求的下级域名内，可以代替旗下授权后公众号发起网页授权。
             * 下级域名必须是$APPID$.wx.abc.com的形式
             * （$APPID$为公众号的AppID的替换符，建议第三方用这种方式，若需可做域名映射），
             * 如果不按这种形式来做，旗下公众号违规将可能导致整个网站被封。
             * 
             * 
             * 此处，使用域名泛解析，DNS上直接设置 *.wxc.shengxunwei.com 
             * 要注意的是 wxc.shengxunwei.com 必须是80端口
             * 不论是作为第三方平台运营还是独立运营
             * domainId 还是要放在URL后面
             * 
             */

            //1.从SESSION中取出MemberContext，如果没有，跳转到微信网页授权地址
            //网页授权
            //a.取得用户OpenId，并判断用户当前是否关注当前公众号
            //b.如果用户已关注，但没有用户信息，则新建用户
            //c.如果用户不是已关注用户，跳转到引导关注页面

            Guid domainId = Guid.Empty;
            object objDomainId = filterContext.RouteData.Values["domainId"];
            if (objDomainId == null || Guid.TryParse(objDomainId.ToString(), out domainId) == false)
            {
                //TODO:重定向到错误页面
                //filterContext.Result = new RedirectResult("~/Home/Login");
                filterContext.Result = new HttpStatusCodeResult(404);
                return;
            }

            DomainContext = ClientDomainPool.Instance.GetDomainContext(domainId);

            if (DomainContext == null)
            {
                Log.Write("指定的 domoainId 不存在", domainId.ToString(), TraceEventType.Warning);
                filterContext.Result = new HttpStatusCodeResult(404);
                return;
            }

            //允许匿名浏览，不取用户信息
            object[] objAllowedAnonymousArray =
                filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowedAnonymous), false);
            if (objAllowedAnonymousArray.Length > 0)
                return;

            MemberContext = SessionContainer.GetMemberContext(filterContext.HttpContext);

            // Uri uri = new Uri("http://wxctest.shengxunwei.com/WeixinApi/Handler/F6AAD430-CA1F-4AFD-B2B0-6E0D2FABB622");

            if (MemberContext == null)
            {
                #region 调用微信网页授权接口取用户基本信息

                if (_debug)
                {
                    MemberEntity member = MemberManager.Instance.GetMemberByOpenId(domainId, DomainContext.AppId, "oXCfEwEteoWmCygMuCKqhvqshVnQ");
                    MemberContext = new MemberContext(member);
                    SessionContainer.SetMemberContext(HttpContext, MemberContext);
                    SessionContainer.SetOpenId(HttpContext, "oXCfEwEteoWmCygMuCKqhvqshVnQ");
                }
                else
                {
                    //对于API请求，不存在调用微信网页授权接口取用户基本信息
                    //直接返回会话过期，让页面去刷新
                    //如果该API允许仅OpenId即可浏览且已经有OpenId了，则允许请求
                    object[] objAllowedOnlyOpenIdArray =
                        filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowedOnlyOpenId), false);
                    bool allowedOnlyOpenId = objAllowedOnlyOpenIdArray.Length > 0;
                    if (allowedOnlyOpenId && String.IsNullOrEmpty(this.OpenId) == false)
                    {
                        //do nothing，继续访问API
                    }
                    else
                    {
                        ApiResult apiResult = new ApiResult()
                        {
                            Success = false,
                            Message = "会话已过期",
                            Reason = 7001
                        };
                        ContentResult result = new ContentResult();
                        result.ContentEncoding = Encoding.UTF8;
                        result.Content = Newtonsoft.Json.JsonConvert.SerializeObject(apiResult);
                        filterContext.Result = result;
                        return;
                    }
                }

                #endregion
            }
        }

    }
}