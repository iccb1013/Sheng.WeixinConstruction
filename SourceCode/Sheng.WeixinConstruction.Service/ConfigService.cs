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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Service
{
    public class ConfigService
    {
        private static readonly ConfigService _instance = new ConfigService();
        public static ConfigService Instance
        {
            get { return _instance; }
        }


        public WxConfigurationSection Configuration
        {
            get;
            private set;
        }

        private ConfigService()
        {
            Configuration =
                ConfigurationManager.GetSection("wxConfiguration") as WxConfigurationSection;
        }
    }
}
