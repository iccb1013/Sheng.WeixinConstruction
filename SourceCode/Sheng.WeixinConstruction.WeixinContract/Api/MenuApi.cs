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
    public static class MenuApi
    {
        private static readonly HttpService _httpService = HttpService.Instance;

        /// <summary>
        /// 自定义菜单创建接口
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="accessToken"></param>
        public static RequestApiResult Create(ButtonWrapper menu, string accessToken)
        {
            RequestApiResult result = new RequestApiResult();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + accessToken;
            requestArgs.Content = JsonConvert.SerializeObject(menu);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiError = WeixinApiHelper.Parse(result.HttpRequestResult.Content);
            }

            return result;
        }
    }
}
