using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("MovieTimes")]
    public class MovieTimesEntity
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

        public Guid Movie
        {
            get;
            set;
        }

        public DateTime Time
        {
            get;
            set;
        }

        public string EndTime
        {
            get;
            set;
        }

        /// <summary>
        /// 放映类型
        /// 3D/IMAX
        /// </summary>
        public string ShowType
        {
            get;
            set;
        }

        public string Language
        {
            get;
            set;
        }

        /// <summary>
        /// 放映厅
        /// </summary>
        public string ScreeningRoom
        {
            get;
            set;
        }
    }
}
