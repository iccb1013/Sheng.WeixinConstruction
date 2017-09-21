using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class SortDigest
    {
        [Key]
        public Guid Id
        {
            get;
            set;
        }

        public int Sort
        {
            get;
            set;
        }
    }
}
