using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    [DataContract]
    public class WeixinTag
    {
        private WeixinTag_Tag _tag = new WeixinTag_Tag();
        [DataMember(Name = "tag")]
        public WeixinTag_Tag Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
    }

    [DataContract]
    public class WeixinTag_Tag
    {
        [DataMember(Name = "id")]
        public int Id
        {
            get;
            set;
        }

        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }
    }
}
