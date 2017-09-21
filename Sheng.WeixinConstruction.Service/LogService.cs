using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Service
{
    /// <summary>
    /// Critical  Error  Warning  Information  Verbose  ActivityTracing 严重程度依次递减
    /// </summary>
    public class LogService
    {
        private static LogService _instance;
        public static LogService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LogService();

                return _instance;
            }
        }

        private LogWriter _writer;

        private LogService()
        {
            IConfigurationSource configurationSource = ConfigurationSourceFactory.Create();
            LogWriterFactory logWriterFactory = new LogWriterFactory(configurationSource);
            _writer = logWriterFactory.Create();

            Logger.SetLogWriter(_writer);
        }

        /*
         * ExceptionHandling 需要先调用SetLogWriter
         * 所以想那边显示调用以确保
         * 但是，SetLogWriter 重复调用会报错，虽然能通过一个bool值让它不要抛出异常
         * 但是，重复调用之后 _writer，就不能用了，一用就报错
         * 现在折衷方案，在Application_Start中调用一下log，确保其最先初始化
         */
        //public void SetLogWriter()
        //{
        //    Logger.SetLogWriter(_writer, false);
        //}

        /// <summary>
        /// Information
        /// </summary>
        /// <param name="message"></param>
        public void Write(string message)
        {
            Write(null, message, TraceEventType.Information);
        }

        public void Write(string message, TraceEventType traceEventType)
        {
            Write(null, message, traceEventType);
        }

        /// <summary>
        /// Information
        /// </summary>
        /// <param name="message"></param>
        public void Write(string title, string message)
        {
            Write(title, message, TraceEventType.Information);
        }

        public void Write(string title, string message, TraceEventType traceEventType)
        {
            LogEntry log = new LogEntry();
            log.Severity = traceEventType;
            log.Title = title;
            log.Message = message;

            _writer.Write(log);
        }
    }
}
