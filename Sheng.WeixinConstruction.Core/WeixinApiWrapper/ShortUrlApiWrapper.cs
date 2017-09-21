using Linkup.Common;
using Newtonsoft.Json;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class ShortUrlApiWrapper
    {
        private static LogService _log = LogService.Instance;

        public static RequestApiResult<WeixinCreateShortUrlResult> GetShortUrl(DomainContext domainContext, string longUrl)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinCreateShortUrlResult> result = ShortUrlApi.GetShortUrl(accessToken, longUrl);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = ShortUrlApi.GetShortUrl(accessToken, longUrl);
                    if (result.Success == false)
                    {
                        _log.Write("ShortUrlApi.GetShortUrl 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("ShortUrlApi.GetShortUrl 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

    }
}
