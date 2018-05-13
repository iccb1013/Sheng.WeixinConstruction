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


using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new DefaultExceptionFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
