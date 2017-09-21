using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkup.DataRelationalMapping
{
    public class ColumnAttribute : RelationalMappingAttribute
    {
        public string Name
        {
            get;
            private set;
        }

        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }
}
