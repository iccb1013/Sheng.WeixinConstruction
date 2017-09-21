using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkup.DataRelationalMapping
{
    /// <summary>
    /// 只对插入数据，更新数据影响
    /// SELECT 数据时不考虑此属性
    /// </summary>
    public class NotMappedAttribute : RelationalMappingAttribute
    {
    }
}
