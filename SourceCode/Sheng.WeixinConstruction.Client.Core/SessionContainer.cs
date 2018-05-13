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


using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Core
{
    public static class SessionContainer
    {
        private static SessionPool _sessionPool = SessionPool.Instance;
        
        public static void SetMemberContext(HttpContextBase httpContext, MemberContext memberContext)
        {
            httpContext.Session["MemberContext"] = memberContext;
            _sessionPool.Set(memberContext.Member.OpenId, httpContext.Session);
        }

        public static MemberContext GetMemberContext(HttpContextBase httpContext)
        {
            return httpContext.Session["MemberContext"] as MemberContext;
        }

        public static MemberContext GetMemberContext(HttpContext httpContext)
        {
            return httpContext.Session["MemberContext"] as MemberContext;
        }

        public static void ClearMemberContext(HttpContextBase httpContext)
        {
            httpContext.Session.Abandon();
        }

        /// <summary>
        /// 对于允许匿名浏览的画面，没有 MemberContext，把OpenId放在Session中
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="openId"></param>
        public static void SetOpenId(HttpContextBase httpContext, string openId)
        {
            httpContext.Session["OpenId"] = openId;
            _sessionPool.Set(openId, httpContext.Session);
        }

        public static string GetOpenId(HttpContextBase httpContext)
        {
            return httpContext.Session["OpenId"] as string;
        }


    }
}