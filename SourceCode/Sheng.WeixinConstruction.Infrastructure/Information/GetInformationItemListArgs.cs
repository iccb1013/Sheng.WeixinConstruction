using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetInformationItemListArgs : GetItemListArgs
    {

        public Guid CategoryId
        {
            get;
            set;
        }

        /// <summary>
        /// 查询关键字
        /// </summary>
        public string Keyword
        {
            get;
            set;
        }

    }
}
