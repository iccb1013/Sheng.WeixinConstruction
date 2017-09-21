using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("OperatedLog")]
    public class OperatedLogEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

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

        public DateTime Time
        {
            get;
            set;
        }

        public Guid User
        {
            get;
            set;
        }

        public string IP
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public EnumModule Module
        {
            get;
            set;
        }

        /// <summary>
        /// Json数据包
        /// </summary>
        public string Detail
        {
            get;
            set;
        }
    }
}
