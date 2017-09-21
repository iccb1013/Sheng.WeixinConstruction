using Linkup.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Service
{
    public class SmtpService
    {
        private static readonly SmtpService _instance = new SmtpService();
        public static SmtpService Instance
        {
            get { return _instance; }
        }

        private LogService _log = LogService.Instance;

        private SmtpClient _smtpClient = new SmtpClient();

        private string _emailPassword = ConfigurationManager.AppSettings["emailPassword"];

        private SmtpService()
        {
            _smtpClient.Host = "smtp.163.com";
            _smtpClient.Port = 25;
            _smtpClient.UseDefaultCredentials = true;
            _smtpClient.Credentials = new NetworkCredential("linkup_noreply", _emailPassword);
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        public void Send(MailMessage mailMessage)
        {
            if (mailMessage == null)
            {
                Debug.Assert(false, "mailMessage 为 null");
                return;
            }

            try
            {
                _smtpClient.Send(mailMessage);
            }
            catch(Exception ex)
            {
                _log.Write("邮件发送失败", ex.Message, TraceEventType.Error);
            }
        }
    }
}
