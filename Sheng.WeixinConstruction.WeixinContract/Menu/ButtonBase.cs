using Linkup.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    [DataContract]
    public class ButtonBase
    {
        /// <summary>
        /// 菜单标题，不超过16个字节，子菜单不超过40个字节
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }
    }
}
