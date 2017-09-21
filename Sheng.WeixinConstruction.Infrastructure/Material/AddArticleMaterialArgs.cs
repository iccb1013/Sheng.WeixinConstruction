using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [DataContract]
    public class AddArticleMaterialArgs
    {
        /// <summary>
        /// 素材名
        /// </summary>
         [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }

        public Guid OperatorUser
        {
            get;
            set;
        }


        [DataMember(Name = "articles")]
        public List<ArticleMaterialItemEntity> ArticleList
        {
            get;
            set;
        }
    }
}
