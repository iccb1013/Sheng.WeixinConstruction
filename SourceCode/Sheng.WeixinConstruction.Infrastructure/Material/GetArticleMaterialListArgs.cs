using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetArticleMaterialListArgs : GetItemListArgs
    {
        /// <summary>
        /// 是否排除未发布到微信 后台的
        /// </summary>
        public bool ExceptUnpublished
        {
            get;
            set;
        }
    }
}
