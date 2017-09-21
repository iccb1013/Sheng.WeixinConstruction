using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Linkup.Common
{
    public static class FastReflectionFactories
    {
        static FastReflectionFactories()
        {
            MethodInvokerFactory = new MethodInvokerFactory();
            PropertyAccessorFactory = new PropertyAccessorFactory();
            FieldAccessorFactory = new FieldAccessorFactory();
            ConstructorInvokerFactory = new ConstructorInvokerFactory();
        }

        public static IFastReflectionFactory<MethodInfo, IMethodInvoker> MethodInvokerFactory { get; set; }

        public static IFastReflectionFactory<PropertyInfo, IPropertyAccessor> PropertyAccessorFactory { get; set; }

        public static IFastReflectionFactory<FieldInfo, IFieldAccessor> FieldAccessorFactory { get; set; }

        public static IFastReflectionFactory<ConstructorInfo, IConstructorInvoker> ConstructorInvokerFactory { get; set; }
    }
}
