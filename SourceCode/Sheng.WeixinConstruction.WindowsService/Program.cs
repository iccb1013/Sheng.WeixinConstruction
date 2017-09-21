using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WindowsService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        //Webbrowser问题不灵
        //[STAThread]
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new MainService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
