using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkup.DataRelationalMapping
{
    /// <summary>
    /// To 是指实体类型转数据库字段类型
    /// From 是指数据库字段类型转实体类型
    /// </summary>
    public abstract class DataHelperConvertAttribute : RelationalMappingAttribute
    {
        public abstract object CovertTo(object value);

        public abstract object CovertFrom(object value);
    }
}
