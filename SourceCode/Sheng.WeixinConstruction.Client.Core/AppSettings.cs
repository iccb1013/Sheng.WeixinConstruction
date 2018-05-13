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
using System.Configuration;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Core
{
    public static class AppSettings
    {
        private static string _intranetIp;
        public static string IntranetIp
        {
            get
            {
                if (String.IsNullOrEmpty(_intranetIp))
                {
                    _intranetIp = ConfigurationManager.AppSettings["IntranetIp"];
                }
                return _intranetIp;
            }
        }
    }
}