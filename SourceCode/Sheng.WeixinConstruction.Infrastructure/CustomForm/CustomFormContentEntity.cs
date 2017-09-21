using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("CustomFormContent")]
    public class CustomFormContentEntity
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

        public Guid Form
        {
            get;
            set;
        }

        public string MemberOpenId
        {
            get;
            set;
        }

        public DateTime FillinTime
        {
            get;
            set;
        }

        public EnumCustomFormContentStatus Status
        {
            get;
            set;
        }
    }
}
