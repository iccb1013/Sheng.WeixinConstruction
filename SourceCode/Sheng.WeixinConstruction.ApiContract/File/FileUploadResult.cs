using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.ApiContract
{
    public class FileUploadResult
    {
        private bool _success = false;
        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        /// <summary>
        /// 失败时为失败原因
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 成功时为文件的相对路径
        /// </summary>
        public string StoreFilePath
        {
            get;
            set;
        }

        /// <summary>
        /// 文件Id
        /// </summary>
        public Guid Id
        {
            get;
            set;
        }
    }
}