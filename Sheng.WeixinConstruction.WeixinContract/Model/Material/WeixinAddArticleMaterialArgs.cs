using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    /// <summary>
    /// 添加永久素材  图文
    /// </summary>
    [DataContract]
    public class WeixinAddArticleMaterialArgs
    {
        [DataMember(Name = "articles")]
        public List<WeixinArticleMaterial> ArticleList
        {
            get;
            set;
        }
    }
}
