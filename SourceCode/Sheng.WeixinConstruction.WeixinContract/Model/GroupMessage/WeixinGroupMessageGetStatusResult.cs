using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    [DataContract]
    public class WeixinGroupMessageGetStatusResult
    {
        [DataMember(Name = "msg_id")]
        public int MsgId
        {
            get;
            set;
        }

        [DataMember(Name = "msg_status")]
        public string Status
        {
            get;
            set;
        }
    }
}
