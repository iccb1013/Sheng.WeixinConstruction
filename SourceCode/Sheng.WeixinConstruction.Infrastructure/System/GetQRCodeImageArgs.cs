using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetQRCodeImageArgs
    {
        public Guid Domain
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

    public class GetQRCodeImageResult
    {
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 文件服务器上的相对路径
        /// </summary>
        public string FileName
        {
            get;
            set;
        }
    }
}
