using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetMemberListArgs : GetItemListArgs
    {
        /// <summary>
        /// GroupId:-1 全部，-2 已取消关注
        /// </summary>
        public int GroupId
        {
            get;
            set;
        }

        public string NickName
        {
            get;
            set;
        }

        public string MobilePhone
        {
            get;
            set;
        }

        public string CardNumber
        {
            get;
            set;
        }
    }
}
