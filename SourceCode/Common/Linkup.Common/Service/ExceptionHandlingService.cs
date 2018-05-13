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


using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkup.Common
{
    public class ExceptionHandlingService
    {
        private static ExceptionHandlingService _instance;
        public static ExceptionHandlingService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ExceptionHandlingService();

                return _instance;
            }
        }

        private ExceptionManager _exceptionManager;
        /// <summary>
        /// HandleException 后，会自动写日志，并调用自定义处理程序
        /// </summary>
        public ExceptionManager ExceptionManager
        {
            get { return _exceptionManager; }
        }

        private ExceptionHandlingService()
        {
            ExceptionHandlingSettings section = (ExceptionHandlingSettings)ConfigurationManager
                .GetSection(ExceptionHandlingSettings.SectionName);
            _exceptionManager = section.BuildExceptionManager();
        }

        /// <summary>
        /// JustLog
        /// </summary>
        /// <param name="exceptionToHandle"></param>
        /// <returns></returns>
        public bool HandleException(Exception exceptionToHandle)
        {
            if (exceptionToHandle==null)
            {
                return true;
            }

            if (exceptionToHandle is WrappedException)
            {
                return true;
            }

            return _exceptionManager.HandleException(exceptionToHandle, ExceptionPolicyNames.JustLog);
        }

        //public bool HandleException(Exception exceptionToHandle, string policyName)
        //{
        //    return _exceptionManager.HandleException(exceptionToHandle, policyName);
        //}

        /// <summary>
        /// LogAndWrap
        /// 如果 exceptionToHandle 是 WrappedException ,表示已经是企业库处理过的了
        /// 直接返回不再交给企业库处理
        /// exceptionToHandle 会被包装到 WrappedException 的 InnerException 中 放到 exceptionToThrow 中
        /// </summary>
        /// <param name="exceptionToHandle"></param>
        /// <param name="exceptionToThrow"></param>
        /// <returns></returns>
        public bool HandleException(Exception exceptionToHandle, out Exception exceptionToThrow)
        {
            if (exceptionToHandle is WrappedException)
            {
                exceptionToThrow = exceptionToHandle;
                return true;
            }

            return _exceptionManager.HandleException(exceptionToHandle, ExceptionPolicyNames.LogAndWrap,
                out exceptionToThrow);
        }
    }

    public static class ExceptionPolicyNames
    {
        /// <summary>
        /// 记录日志，并对异常进行包装后继续抛出
        /// </summary>
        public const string LogAndWrap = "LogAndWrap";

        /// <summary>
        /// 只记录日志，不继续抛出
        /// </summary>
        public const string JustLog = "JustLog";
    }
}
