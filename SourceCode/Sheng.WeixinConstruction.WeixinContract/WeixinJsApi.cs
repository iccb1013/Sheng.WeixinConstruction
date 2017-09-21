using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    /*
     * http://mp.weixin.qq.com/wiki/7/aaa137b55fb2e0456bf8dd9148dd613f.html
     * 
     */

    public static class WeixinJsApi
    {
        public static WeixinJsApiConfig GetConfig(string jsApiTicket, string url)
        {
            var noncestr = GetNoncestr();
            var timestamp = GetTimesTamp();
            var ticket = jsApiTicket;
            var signature =
                HashAndMd5.Sha1("jsapi_ticket=" + ticket + "&noncestr=" + noncestr + "&timestamp=" + timestamp +
                                "&url=" + url);
            return new WeixinJsApiConfig
            {
                NonceStr = noncestr,
                Timestamp = timestamp,
                Signature = signature
            };
        }

        /// <summary>
        /// 随机字符串
        /// </summary>
        /// <returns></returns>
        private static string GetNoncestr()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static long GetTimesTamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }
    }
}
