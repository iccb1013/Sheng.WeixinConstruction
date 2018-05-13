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
    * 用户标签管理
    * https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421140837&token=&lang=zh_CN
    * 
    * 开发者可以使用用户标签管理的相关接口，实现对公众号的标签进行创建、查询、修改、删除等操作，也可以对用户进行打标签、取消标签等操作。
    * 一个公众号，最多可以创建100个标签。
     * 标签名（30个字符以内）
    */
    public class TagsApi
    {
        private static readonly HttpService _httpService = HttpService.Instance;

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinTag> Create(string access_token, WeixinCreateTagArgs args)
        {
            RequestApiResult<WeixinTag> result =
               new RequestApiResult<WeixinTag>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/tags/create?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinTag>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 获取公众号已创建的标签
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinGetTagListResult> GetTagList(string access_token)
        {
            RequestApiResult<WeixinGetTagListResult> result =
               new RequestApiResult<WeixinGetTagListResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "GET";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/tags/get?access_token={0}", access_token);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinGetTagListResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 编辑标签
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult Update(string access_token, WeixinTag args)
        {
            RequestApiResult result = new RequestApiResult();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/tags/update?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiError = WeixinApiHelper.Parse(result.HttpRequestResult.Content);
            }

            return result;
        }

        /// <summary>
        /// 删除标签
        /// 请注意，当某个标签下的粉丝超过10w时，后台不可直接删除标签。
        /// 此时，开发者可以对该标签下的openid列表，先进行取消标签的操作，直到粉丝数不超过10w后，才可直接删除该标签。
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static RequestApiResult Remove(string access_token, int tagId)
        {
            RequestApiResult result = new RequestApiResult();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/tags/delete?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(new { tag = new { id = tagId } });

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiError = WeixinApiHelper.Parse(result.HttpRequestResult.Content);
            }

            return result;
        }

        /// <summary>
        /// 获取标签下粉丝列表
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinGetTagUserListResult> GetTagUserList(
            string access_token, int tagId, string next_openid)
        {
            RequestApiResult<WeixinGetTagUserListResult> result =
               new RequestApiResult<WeixinGetTagUserListResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "GET";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/user/tag/get?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(new { tagid = tagId, next_openid = next_openid });

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinGetTagUserListResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 批量为用户打标签
        /// 标签功能目前支持公众号为用户打上最多三个标签。
        /// 每次传入的openid列表个数不能超过50个
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult BatchTagging(string access_token, WeixinTagBatchTaggingArgs args)
        {
            RequestApiResult result = new RequestApiResult();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/tags/members/batchtagging?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiError = WeixinApiHelper.Parse(result.HttpRequestResult.Content);
            }

            return result;
        }

        /// <summary>
        /// 批量为用户取消标签
        /// 标签功能目前支持公众号为用户打上最多三个标签。
        /// 每次传入的openid列表个数不能超过50个
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult BatchUntagging(string access_token, WeixinTagBatchTaggingArgs args)
        {
            RequestApiResult result = new RequestApiResult();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/tags/members/batchuntagging?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiError = WeixinApiHelper.Parse(result.HttpRequestResult.Content);
            }

            return result;
        }

        /// <summary>
        /// 获取用户身上的标签列表
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinGetUserTagListResult> GetUserTagList(string access_token, string openId)
        {
            RequestApiResult<WeixinGetUserTagListResult> result =
               new RequestApiResult<WeixinGetUserTagListResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/tags/getidlist?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(new { openid = openId });

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinGetUserTagListResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }
    }
}
