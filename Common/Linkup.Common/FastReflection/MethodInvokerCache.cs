using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Linkup.Common
{
    public class MethodInvokerCache : FastReflectionCache<MethodInfo, IMethodInvoker>
    {
        protected override IMethodInvoker Create(MethodInfo key)
        {
            return FastReflectionFactories.MethodInvokerFactory.Create(key);
        }
    }
}
