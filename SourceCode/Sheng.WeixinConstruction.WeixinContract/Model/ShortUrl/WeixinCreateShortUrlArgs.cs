using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    /// <summary>
    /// 将一条长链接转成短链接。
    /// </summary>
    [DataContract]
    public class WeixinCreateShortUrlArgs
    {
        [DataMember(Name = "action")]
        public string Action
        {
            get { return "long2short"; }
        }

        [DataMember(Name = "long_url")]
        public string LongUrl
        {
            get;
            set;
        }
    }
}
