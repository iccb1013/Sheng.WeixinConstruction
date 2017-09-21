using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkup.Common
{
    public interface IFastReflectionCache<TKey, TValue>
    {
        TValue Get(TKey key);
    }
}
