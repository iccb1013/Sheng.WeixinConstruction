using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("MoviePlan")]
    public class MoviePlanEntity
    {
        public Guid Domain
        {
            get;
            set;
        }

        public string AppId
        {
            get;
            set;
        }

        public DateTime Date
        {
            get;
            set;
        }

        public Guid Movie
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
