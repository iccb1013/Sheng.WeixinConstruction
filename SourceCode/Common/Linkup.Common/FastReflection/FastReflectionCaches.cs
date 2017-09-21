using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Linkup.Common
{
    public static class FastReflectionCaches
    {
        static FastReflectionCaches()
        {
            MethodInvokerCache = new MethodInvokerCache();
            PropertyAccessorCache = new PropertyAccessorCache();
            FieldAccessorCache = new FieldAccessorCache();
            ConstructorInvokerCache = new ConstructorInvokerCache();
        }

        public static IFastReflectionCache<MethodInfo, IMethodInvoker> MethodInvokerCache { get; set; }

        public static IFastReflectionCache<PropertyInfo, IPropertyAccessor> PropertyAccessorCache { get; set; }

        public static IFastReflectionCache<FieldInfo, IFieldAccessor> FieldAccessorCache { get; set; }

        public static IFastReflectionCache<ConstructorInfo, IConstructorInvoker> ConstructorInvokerCache { get; set; }
    }
}
