using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Concurrent;

namespace Linkup.Common
{
    public abstract class FastReflectionCache<TKey, TValue> : IFastReflectionCache<TKey, TValue>
    {
        //对于Property 访问器： FastReflectionCache<PropertyInfo, IPropertyAccessor>
        //此处在高并发时有Bug
        //发生在：高并发时的 PropertyInfo 是同一个名子，但不是同一个对象，lock是废的
        //换 ConcurrentDictionary 解决 
       // private Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();
        private ConcurrentDictionary<TKey, TValue> _cache = new ConcurrentDictionary<TKey, TValue>();

        public TValue Get(TKey key)
        {
            TValue value = default(TValue);
            if (_cache.TryGetValue(key, out value))
            {
                return value;
            }

            lock (key)
            {
                if (this._cache.TryGetValue(key, out value) == false)
                {
                    value = this.Create(key);
                    _cache[key] = value;
                }
            }

            return value;
        }

        protected abstract TValue Create(TKey key);
    }
}
