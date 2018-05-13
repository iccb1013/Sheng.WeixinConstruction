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
using Linkup.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Service
{
    public class ServiceUnity
    {
        private static readonly ServiceUnity _instance = new ServiceUnity();
        public static ServiceUnity Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase;
        /// <summary>
        /// 连接Config文件中指定的默认数据库
        /// </summary>
        public DatabaseWrapper Database
        {
            get
            {
                if (_dataBase == null)
                {
                    _dataBase = new DatabaseWrapper();
                }
                return _dataBase;
            }
        }

        private ServiceUnity()
        {

        }

        private LogService _log = LogService.Instance;
        public LogService Log
        {
            get { return _log; }
        }

        private CachingService _cachingService = CachingService.Instance;
        public CachingService CachingService
        {
            get { return _cachingService; }
            set { _cachingService = value; }
        }

        private ExceptionHandlingService _exceptionHandling = ExceptionHandlingService.Instance;
        /// <summary>
        /// HandleException 后，会自动写日志，并调用自定义处理程序
        /// </summary>
        public ExceptionHandlingService ExceptionHandling
        {
            get { return _exceptionHandling; }
        }
    }
}
