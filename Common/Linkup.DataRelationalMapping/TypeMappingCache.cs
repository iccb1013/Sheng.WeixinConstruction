using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Linkup.DataRelationalMapping
{
    static class TypeMappingCache
    {
        private static Dictionary<Type, TypeMappingDescription> _cacheList = new Dictionary<Type, TypeMappingDescription>();

        static TypeMappingCache()
        {

        }

        public static TypeMappingDescription Get(Type type)
        {
            if (type == null)
                return null;

            if (_cacheList.Keys.Contains(type))
                return _cacheList[type];
            else
            {
                TypeMappingDescription cacheCodon = new TypeMappingDescription(type);

                Monitor.Enter(_cacheList);

                if (_cacheList.Keys.Contains(type) == false)
                    _cacheList.Add(type, cacheCodon);

                Monitor.Exit(_cacheList);

                return cacheCodon;

            }
        }

    }
}
