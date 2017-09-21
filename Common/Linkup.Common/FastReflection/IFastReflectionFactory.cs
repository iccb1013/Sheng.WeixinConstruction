using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkup.Common
{
    public interface IFastReflectionFactory<TKey, TValue>
    {
        TValue Create(TKey key);
    }
}
