using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkup.DataRelationalMapping
{
    public class JustGuidFormStringConvertAttribute : DataHelperConvertAttribute
    {
        public override object CovertTo(object value)
        {
            return value;
        }

        public override object CovertFrom(object value)
        {
            if (value == null)
                return null;

            Guid guid = new Guid();
            Guid.TryParse(value.ToString(), out guid);
            return guid;
        }
    }
}
