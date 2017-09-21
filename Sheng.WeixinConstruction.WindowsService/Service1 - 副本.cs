using Quartz;
using Quartz.Impl;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WindowsService
{
    public partial class Service1 : ServiceBase
    {
        private LogService _logService = LogService.Instance;
        private ExceptionHandlingService _exceptionHandling = ServiceUnity.Instance.ExceptionHandling;

        private readonly IScheduler _scheduler;

        public Service1()
        {
           // Debugger.Launch();

            InitializeComponent();

            //确保第一时间调用  SetLogWriter 方法，否则如果 ExceptionHandlingService 先于它初始化
            //则会报错
            _logService.Write("Application_Start");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //Quartz.net开源作业调度框架使用详解
            //http://www.cnblogs.com/knowledgesea/p/4930469.html

            //1.首先创建一个作业调度池
            ISchedulerFactory schedf = new StdSchedulerFactory();
            _scheduler = schedf.GetScheduler();
            //2.创建出来一个具体的作业
            IJobDetail job = JobBuilder.Create<CinemaSyncJob>().Build();
            //3.创建并配置一个触发器
            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create().WithSimpleSchedule(
                x => x.WithIntervalInSeconds(10).WithRepeatCount(-1)).Build();
            //4.加入作业调度池中
            _scheduler.ScheduleJob(job, trigger);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject;
            if (exception == null)
                return;

            if ((exception is WrappedException) == false)
            {
                _exceptionHandling.HandleException((Exception)exception);
            }
        }

        protected override void OnStart(string[] args)
        {
            //5.开始运行
            _scheduler.Start();
        }

        protected override void OnStop()
        {
            _scheduler.Shutdown(false);
        }

        
    }
}
