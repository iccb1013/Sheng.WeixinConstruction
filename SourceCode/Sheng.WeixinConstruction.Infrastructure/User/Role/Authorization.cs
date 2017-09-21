using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    /// <summary>
    /// 表示一个权限key
    /// </summary>
    [Table("RoleAuthorization")]
    public class Authorization
    {
        [Column("AuthorizationKey")]
        public string Key
        {
            get;
            set;
        }
    }
}
