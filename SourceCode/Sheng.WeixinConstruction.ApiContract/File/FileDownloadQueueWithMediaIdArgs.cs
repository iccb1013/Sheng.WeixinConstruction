using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.ApiContract
{
    public class FileDownloadQueueWithMediaIdArgs
    {
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
    }
}
