using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkup.Common
{
    public class HttpRequestArgs
    {
        public string Url
        {
            get;
            set;
        }

        private string _method = "POST";
        public string Method
        {
            get { return _method; }
            set { _method = value; }
        }

        //private string _contentType = "";
        //public string ContentType
        //{
        //    get { return _contentType; }
        //    set { _contentType = value; }
        //}

        /// <summary>
        /// 是否使用证书
        /// </summary>
        public bool UseCertificate
        {
            get;
            set;
        }

        public string CertificatePath
        {
            get;
            set;
        }

        public string CertificatePassword
        {
            get;
            set;
        }

        public string File
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }
    }
}
