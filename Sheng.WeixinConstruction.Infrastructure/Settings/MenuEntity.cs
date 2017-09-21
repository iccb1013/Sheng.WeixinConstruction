using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("Menu")]
    public class MenuEntity
    {
        private Guid _domainId = Guid.NewGuid();
        [Key]
        public Guid DomainId
        {
            get { return _domainId; }
            set { _domainId = value; }
        }

        public string Menu
        {
            get;
            set;
        }
    }
}
