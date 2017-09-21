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
    public class WeixinCreateShortUrlResult
    {
        [DataMember(Name = "short_url")]
        public string ShortUrl
        {
            get;
            set;
        }
    }
}
