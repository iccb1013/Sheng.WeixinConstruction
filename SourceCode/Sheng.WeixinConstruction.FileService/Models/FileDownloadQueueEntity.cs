using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.FileService.Models
{
    [Table("FileDownloadQueue")]
    public class FileDownloadQueueEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
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

        public string AppId
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public DateTime? FinishTime
        {
            get;
            set;
        }

        /// <summary>
        /// MediaId
        /// </summary>
        public string MediaId
        {
            get;
            set;
        }

        /// <summary>
        /// 异步时POST结果
        /// </summary>
        public string CallbackUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 附加数据，返回结果中附带上
        /// </summary>
        public string Tag
        {
            get;
            set;
        }

        public FileDownloadQueueItemStatus Status
        {
            get;
            set;
        }

        public bool Success
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 返回保存的相对路径（包括文件名）
        /// </summary>
        public string OutputFile
        {
            get;
            set;
        }

        public string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// 文件大小（KB）
        /// </summary>
        public int FileLength
        {
            get;
            set;
        }

    }
}