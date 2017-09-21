using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class FileUploadParameter
    {
        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Guid UserId
        {
            get;
            set;
        }

        public Guid DomainId
        {
            get;
            set;
        }

        public string FileService
        {
            get;
            set;
        }
    }
}
