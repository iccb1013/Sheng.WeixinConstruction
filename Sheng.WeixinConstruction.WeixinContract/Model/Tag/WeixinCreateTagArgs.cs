using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    /*
     * 创建标签
     * POST数据例子：
        {
          "tag" : {
            "name" : "广东"//标签名
          }
        }
     */
    [DataContract]
    public class WeixinCreateTagArgs
    {
        private WeixinCreateTagArgs_Tag _tag = new WeixinCreateTagArgs_Tag();
        [DataMember(Name = "tag")]
        public WeixinCreateTagArgs_Tag Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
    }

    [DataContract]
    public class WeixinCreateTagArgs_Tag
    {
        [DataMember(Name="name")]
        public string Name
        {
            get;
            set;
        }
    }
}
