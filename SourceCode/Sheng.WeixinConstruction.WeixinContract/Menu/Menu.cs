using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    [DataContract]
    public class ButtonWrapper
    {
        private List<ButtonBase> _button = new List<ButtonBase>();
        [DataMember(Name = "button")]
        public List<ButtonBase> Button
        {
            get { return _button; }
            set { _button = value; }
        }
    }
}
