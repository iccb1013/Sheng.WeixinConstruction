using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkup.DataRelationalMapping
{
    public class TableAttribute : RelationalMappingAttribute
    {
        public string Name
        {
            get;
            private set;
        }

        public TableAttribute(string name)
        {
            Name = name;
        }
    }
}
