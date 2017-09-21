using Linkup.Common;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetMessageListArgs : GetItemListArgs
    {
        public EnumReceivingMessageType? ReceivingMessageType
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public string MemberOpenId
        {
            get;
            set;
        }

        public string MemberNickName
        {
            get;
            set;
        }
    }
}
