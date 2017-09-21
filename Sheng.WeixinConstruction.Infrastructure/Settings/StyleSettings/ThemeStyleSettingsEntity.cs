using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    /// <summary>
    /// 设置需要独立保存，这样才能兼容独立运行模式和第三方平台运营模式
    /// </summary>
    [Table("StyleSettings")]
    public class ThemeStyleSettingsEntity
    {
        [Key]
        public Guid Domain
        {
            get;
            set;
        }

        [Key]
        public string AppId
        {
            get;
            set;
        }

        public string Theme
        {
            get;
            set;
        }

    }
}
