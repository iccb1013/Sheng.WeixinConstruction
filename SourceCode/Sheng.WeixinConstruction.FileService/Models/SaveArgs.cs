using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.FileService
{
    class SaveArgs
    {
        public Guid Id
        {
            get;
            set;
        }

        public Guid Domain
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string FileExtension
        {
            get;
            set;
        }

        public string MD5
        {
            get;
            set;
        }


        public DateTime UploadDate
        {
            get;
            set;
        }

        public string UploadIPAddress
        {
            get;
            set;
        }

        public Stream Stream
        {
            get;
            set;
        }

        ///// <summary>
        ///// 服务器根路径
        ///// </summary>
        //public string ServerRootDir
        //{
        //    get;
        //    set;
        //}
    }

}
