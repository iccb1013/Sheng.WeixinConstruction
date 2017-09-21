using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    [DataContract]
    public class WeixinGetTagListResult
    {
        private List<WeixinGetTagListResult_Tag> _tagList = new List<WeixinGetTagListResult_Tag>();
        [DataMember(Name = "tags")]
        public List<WeixinGetTagListResult_Tag> TagList
        {
            get { return _tagList; }
            set { _tagList = value; }
        }
    }

    [DataContract]
    public class WeixinGetTagListResult_Tag
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

        [DataMember(Name = "count")]
        public int Count
        {
            get;
            set;
        }
    }
}
