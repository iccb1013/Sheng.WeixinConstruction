using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class AddArticleMaterialResult
    {
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
        /// 图文素材的本地Id
        /// </summary>
        public Guid Id
        {
            get;
            set;
        }

        //public string MediaId
        //{
        //    get;
        //    set;
        //}
    }
}
