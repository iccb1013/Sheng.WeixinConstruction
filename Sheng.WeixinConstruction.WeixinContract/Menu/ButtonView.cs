using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    [DataContract]
    public class ButtonView : TypeButton
    {
        /// <summary>
        /// 网页链接，用户点击菜单可打开链接，不超过256字节
        /// </summary>
        [DataMember(Name = "url")]
        public string Url
        {
            get;
            set;
        }

        public ButtonView()
        {
            this.Type = ButtonType.View;
        }
    }
}
