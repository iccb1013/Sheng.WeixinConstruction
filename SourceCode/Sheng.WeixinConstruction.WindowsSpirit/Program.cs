using Linkup.Common;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sheng.WeixinConstruction.WindowsSpirit
{
    static class Program
    {
        private static LogService _logService = LogService.Instance;
        private static ExceptionHandlingService _exceptionHandling = ServiceUnity.Instance.ExceptionHandling;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //确保第一时间调用  SetLogWriter 方法，否则如果 ExceptionHandlingService 先于它初始化
            //则会报错
            _logService.Write("Application_Start");
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject;
            if (exception == null)
                return;

            if ((exception is WrappedException) == false)
            {
                _exceptionHandling.HandleException((Exception)exception);
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var exception = e.Exception;
            if (exception == null)
                return;

            if ((exception is WrappedException) == false)
            {
                _exceptionHandling.HandleException(exception);
            }
        }
    }
}
