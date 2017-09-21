using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    [DataContract]
    public class WeixinGroup
    {
        private WeixinGroup_Group _group = new WeixinGroup_Group();
        [DataMember(Name = "group")]
        public WeixinGroup_Group Group
        {
            get { return _group; }
            set { _group = value; }
        }
    }

    [DataContract]
    public class WeixinGroup_Group
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
