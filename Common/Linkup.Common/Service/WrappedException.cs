using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkup.Common
{
    /// <summary>
    /// 企业库处理并包装过的异常，此异常已被记录过日志
    /// </summary>
    public class WrappedException : Exception
    {
        public WrappedException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
