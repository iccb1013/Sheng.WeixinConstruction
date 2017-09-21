using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public interface IAutoReply
    {
        EnumAutoReplyType Type { get; }

        string Content { get; }

        string Url { get; }

        string Name { get; }

        string MediaId { get; }

        Guid ArticleId { get; }

        /// <summary>
        /// 资源的本地Id
        /// </summary>
        Guid? MaterialId { get; }
    }
}
