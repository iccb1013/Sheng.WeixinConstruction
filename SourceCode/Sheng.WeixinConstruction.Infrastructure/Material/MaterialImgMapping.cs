using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class MaterialImgMapping
    {
        /// <summary>
        /// 微信的url
        /// </summary>
        public string WeixinUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 在文件服务器上的完整路径
        /// </summary>
        public string FileUrl
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
