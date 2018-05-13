/*
********************************************************************
*
*    曹旭升（sheng.c）
*    E-mail: cao.silhouette@msn.com
*    QQ: 279060597
*    https://github.com/iccb1013
*    http://shengxunwei.com
*
*    © Copyright 2016
*
********************************************************************/


using Linkup.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    /*
   * 长链接转短链接接口
   * https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1443433600&token=&lang=zh_CN
   * 
    * 将一条长链接转成短链接。
     * 主要使用场景： 开发者用于生成二维码的原链接（商品、支付二维码等）太长导致扫码速度和成功率下降，
     * 将原长链接通过此接口转成短链接再生成二维码将大大提升扫码速度和成功率。
   */
    public class ShortUrlApi
    {
        private static readonly HttpService _httpService = HttpService.Instance;

        public static RequestApiResult<WeixinCreateShortUrlResult> GetShortUrl(string access_token, string longUrl)
        {
            WeixinCreateShortUrlArgs args = new WeixinCreateShortUrlArgs();
            args.LongUrl = longUrl;

            RequestApiResult<WeixinCreateShortUrlResult> result =
               new RequestApiResult<WeixinCreateShortUrlResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/shorturl?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinCreateShortUrlResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }
    }
}
