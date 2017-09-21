using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    /// <summary>
    /// 公众号JsApiTicket封装
    /// </summary>
    public class AuthorizerJsApiTicketWrapper
    {
        public string AppId
        {
            get;
            set;
        }

        public string JsApiTicket
        {
            get;
            set;
        }

        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime JsApiTicketExpiryTime
        {
            get;
            set;
        }

        [NotMapped]
        /// <summary>
        /// 快要到期了
        /// </summary>
        public bool WillbeTimeout
        {
            get
            {
                //小于30分钟即准备刷新
                if ((JsApiTicketExpiryTime - DateTime.Now).TotalSeconds <= 1800)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
