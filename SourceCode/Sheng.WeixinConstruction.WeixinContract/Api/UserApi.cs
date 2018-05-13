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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    public static class  UserApi
    {
        private static readonly HttpService _httpService = HttpService.Instance;

        /// <summary>
        /// 获取用户基本信息(UnionID机制)
        /// http://mp.weixin.qq.com/wiki/17/c807ee0f10ce36226637cebf428a0f6d.html
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinUser> GetUserInfo(string access_token, string openid)
        {
            RequestApiResult<WeixinUser> result =
                new RequestApiResult<WeixinUser>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "GET";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN",
               access_token, openid);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinUser>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 获取用户列表
        /// 关注者列表由一串OpenID（加密后的微信号，每个用户对每个公众号的OpenID是唯一的）组成。
        /// 一次拉取调用最多拉取10000个关注者的OpenID，可以通过多次拉取的方式来满足需求。
        /// http://mp.weixin.qq.com/wiki/11/434109e8de46b3968639217bbcb16c2f.html
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="next_openid">第一个拉取的OPENID，不填默认从头开始拉取</param>
        /// <returns></returns>
        public static RequestApiResult<WeixinGetUserListResult> GetUserList(string access_token, string next_openid)
        {
            RequestApiResult<WeixinGetUserListResult> result =
                new RequestApiResult<WeixinGetUserListResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "GET";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/user/get?access_token={0}&next_openid={1}",
               access_token, next_openid);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinGetUserListResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }
    }
}
