using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.FileService
{
    public enum FileDownloadQueueItemStatus
    {
        /// <summary>
        /// 未下载
        /// </summary>
        Pending = 0,
        ///// <summary>
        ///// 正在下载
        ///// </summary>
        //Downloading = 1,
        /// <summary>
        /// 已执行（成功或失败）
        /// </summary>
        Done = 1
    }
}
