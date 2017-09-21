using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkup.Common
{
    public class HttpDownloadArgs
    {
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// 要接收数据的本地路径。
        /// 绝对路径
        /// </summary>
        public string TargetDir
        {
            get;
            set;
        }
    }
}
