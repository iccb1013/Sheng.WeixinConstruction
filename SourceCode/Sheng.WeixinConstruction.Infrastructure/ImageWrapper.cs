using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    /// <summary>
    /// 图片封装，用于上传多图时封装为json
    /// </summary>
    public class ImageWrapper
    {
        public Guid Id
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }
    }
}
