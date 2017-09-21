using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    /*
     * 创建分组
     * POST数据例子：{"group":{"name":"test"}}
     */
    [DataContract]
    public class WeixinCreateGroupArgs
    {
        private WeixinCreateGroupArgs_Group _group = new WeixinCreateGroupArgs_Group();
        [DataMember(Name = "group")]
        public WeixinCreateGroupArgs_Group Group
        {
            get { return _group; }
            set { _group = value; }
        }
    }

    [DataContract]
    public class WeixinCreateGroupArgs_Group
    {
        [DataMember(Name="name")]
        public string Name
        {
            get;
            set;
        }
    }
}
