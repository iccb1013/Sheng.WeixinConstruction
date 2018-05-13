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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell
{
    /// <summary>
    /// 可不指定domainId，但是如果指定了，还是会取出来的
    /// 若是没有 domainId，自然domain及用户信息等一切信息都不会有
    /// </summary>
    public class AllowedEmptyDomain : ActionFilterAttribute
    {

    }
}