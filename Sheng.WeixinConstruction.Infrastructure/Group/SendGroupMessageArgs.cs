using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class SendGroupMessageArgs
    {
        /// <summary>
        /// -1代表全部
        /// </summary>
        public int GroupId
        {
            get;
            set;
        }

        public AutoReplyType Type
        {
            get;
            set;
        }

        public Guid ArticleId
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string MediaId
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
