using Linkup.Common;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sheng.WeixinConstruction.WindowsSpirit
{
    public partial class Form1 : Form
    {
        private LogService _logService = LogService.Instance;
        private ExceptionHandlingService _exceptionHandling = ServiceUnity.Instance.ExceptionHandling;

        private Timer _timer;

        private CinemaSyncJob _cinemaSyncJob = new CinemaSyncJob();

        public Form1()
        {
            InitializeComponent();

            _cinemaSyncJob.Running += _cinemaSyncJob_Running;
            _cinemaSyncJob.End += _cinemaSyncJob_End;

            _timer = new Timer();
            _timer.Interval = 1000 * 60;
            _timer.Tick += _timer_Tick;
        }

        void _cinemaSyncJob_End(object sender, EventArgs e)
        {
            labelMsg.Text = "同步结束：" + DateTime.Now.ToString();
        }

        void _cinemaSyncJob_Running(object sender, EventArgs e)
        {
            labelMsg.Text = "开始同步：" + DateTime.Now.ToString();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _cinemaSyncJob.Execute();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _cinemaSyncJob.Execute();
            _timer.Enabled = true;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            _cinemaSyncJob.Execute();
        }
    }
}
