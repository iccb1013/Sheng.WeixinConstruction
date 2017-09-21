using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.FileService
{
    public static class AppSettings
    {
        private static string _serverPath;
        /// <summary>
        /// 站点所在物理目录
        /// 如 D:\WorkFloder\WeixinConstruction\trunk\SourceCode\Sheng.WeixinConstruction.FileService\
        /// </summary>
        public static string ServerPath
        {
            get
            {
                if (String.IsNullOrEmpty(_serverPath))
                {
                    _serverPath = ConfigurationManager.AppSettings["ServerPath"];
                }
                return _serverPath;
            }
        }
    }
}