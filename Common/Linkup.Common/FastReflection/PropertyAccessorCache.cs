using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Linkup.Common
{
    public class PropertyAccessorCache : FastReflectionCache<PropertyInfo, IPropertyAccessor>
    {
        protected override IPropertyAccessor Create(PropertyInfo key)
        {
            return new PropertyAccessor(key);
        }
    }
}
