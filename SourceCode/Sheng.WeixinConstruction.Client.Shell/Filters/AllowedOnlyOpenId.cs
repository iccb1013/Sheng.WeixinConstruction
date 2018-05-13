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
    /*
     * 在 Action 上加上此属性，访问此页面时，在跳转到微信WEB鉴权时，URL后面会加上 AllowedOnlyOpenId=1
     * 回调到 OAuthCallback 时，判断如果有 AllowedOnlyOpenId，那么在未取得用户详细信息时，也将转到页面
     * 对于认证服务号来说，OpenId是一定能取到的，即使访问者未关注
     * 否则会转到引导关注页面
     */

    /// <summary>
    /// 允许未关注者浏览或调用，但是会取到OpenId，用户信息会尝试获取，但是取不到也允许访问
    /// 判断 MemberContext 是否为 null 来判断是否已关注
    /// </summary>
    public class AllowedOnlyOpenId : ActionFilterAttribute
    {

    }
}