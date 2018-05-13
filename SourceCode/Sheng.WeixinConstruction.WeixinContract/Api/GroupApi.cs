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
     * 用户分组管理
     * http://mp.weixin.qq.com/wiki/8/d6d33cf60bce2a2e4fb10a21be9591b8.html
     * 
     * 一个公众账号，最多支持创建100个分组。
     * 分组名字（30个字符以内）
     */

    public class GroupApi
    {
        private static readonly HttpService _httpService = HttpService.Instance;

        /// <summary>
        /// 创建分组
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinGroup> Create(string access_token, WeixinCreateGroupArgs args)
        {
            RequestApiResult<WeixinGroup> result =
               new RequestApiResult<WeixinGroup>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/groups/create?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinGroup>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 查询所有分组
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinGetGroupListResult> GetGroupList(string access_token)
        {
            RequestApiResult<WeixinGetGroupListResult> result =
               new RequestApiResult<WeixinGetGroupListResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "GET";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/groups/get?access_token={0}", access_token);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinGetGroupListResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 查询用户所在分组
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinGetUserGroupIdResult> GetUserGroupId(string access_token, string openid)
        {
            RequestApiResult<WeixinGetUserGroupIdResult> result =
               new RequestApiResult<WeixinGetUserGroupIdResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/groups/getid?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(new { openid = openid });

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinGetUserGroupIdResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 修改分组名
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult Update(string access_token, WeixinGroup args)
        {
            RequestApiResult result = new RequestApiResult();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/groups/update?access_token={0}", access_token);
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
        /// 移动用户分组
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult SetUserGroup(string access_token, WeixinSetUserGroupArgs args)
        {
            RequestApiResult result = new RequestApiResult();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/groups/members/update?access_token={0}", access_token);
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
        /// 批量移动用户分组
        /// 用户唯一标识符openid的列表（size不能超过50）
        /// 如果仅传一个不存在的OpenId，微信API会返回空串
        /// 如果用户列表中的OpenId，有存在的，有不存在的，则返回正常OK的json
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult SetUserListGroup(string access_token, WeixinSetUserListGroupArgs args)
        {
            RequestApiResult result = new RequestApiResult();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/groups/members/batchupdate?access_token={0}", access_token);
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
        /// 删除分组
        /// 注意本接口是删除一个用户分组，删除分组后，所有该分组内的用户自动进入默认分组。
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static RequestApiResult Remove(string access_token, int groupId)
        {
            RequestApiResult result = new RequestApiResult();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/groups/delete?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(new { group = new { id = groupId } });

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            //微信接口的分组部分有问题
            //成功删除分组后，有时返回空串，有时返回40050
            //此处暂不判断其返回值了，默认成功
            //if (result.HttpRequestResult.Success)
            //{
            //    result.ApiError = WeixinApiHelper.Parse(result.HttpRequestResult.Content);
            //}
            result.ApiError = new WeixinApiErrorResult();

            return result;
        }

    }
}
