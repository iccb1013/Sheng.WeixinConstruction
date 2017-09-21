using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkup.DataRelationalMapping
{
    public class StringToGuidConvertAttribute : DataHelperConvertAttribute
    {
        public override object CovertTo(object value)
        {
            if (value == null)
                return null;

            Guid guid = new Guid();
            Guid.TryParse(value.ToString(), out guid);
            return guid;
        }

        public override object CovertFrom(object value)
        {
            if (value == null)
                return null;

            return value.ToString();
        }
    }
}
