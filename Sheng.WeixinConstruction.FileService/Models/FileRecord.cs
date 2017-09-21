using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.FileService
{
    /// <summary>
    /// 数据库 File 表中的对象模型
    /// </summary>
    [Table("File")]
    class FileRecord
    {
        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Guid Domain
        {
            get;
            set;
        }

        /// <summary>
        /// 原文件名
        /// </summary>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// 存储文件名
        /// </summary>
        public string StoredFileName
        {
            get;
            set;
        }

        /// <summary>
        /// 文件大小,KB 
        /// </summary>
        public int Length
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
    }
}