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
    public class TypeButton : ButtonBase
    {
        private ButtonType _type = ButtonType.Click;
        public ButtonType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [DataMember(Name = "type")]
        public string TypeString
        {
            get { return EnumHelper.GetEnumMemberValue(_type); }
            set { _type = EnumHelper.GetEnumFieldByMemberValue<ButtonType>(value); }
        }
    }
}
