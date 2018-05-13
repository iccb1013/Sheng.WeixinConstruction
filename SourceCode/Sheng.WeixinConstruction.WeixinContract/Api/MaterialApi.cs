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
     * http://mp.weixin.qq.com/wiki/0/21cad57369e3652f6be542c1e3a531ec.html
     * 
     */
    public static class MaterialApi
    {
        private static readonly HttpService _httpService = HttpService.Instance;

        /// <summary>
        /// 获取其他类型（图片、语音、视频）
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinGetNormalMaterialListResult> GetNormalMaterialList(string access_token, WeixinGetMaterialListArgs args)
        {
            RequestApiResult<WeixinGetNormalMaterialListResult> result =
                new RequestApiResult<WeixinGetNormalMaterialListResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinGetNormalMaterialListResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 新增永久素材
        /// http://mp.weixin.qq.com/wiki/0/21cad57369e3652f6be542c1e3a531ec.html
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="file"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinAddMaterialResult> AddNormalMaterial(string access_token, string file, MaterialType type)
        {
            RequestApiResult<WeixinAddMaterialResult> result = new RequestApiResult<WeixinAddMaterialResult>();
            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/material/add_material?access_token={0}&type={1}",
                access_token, EnumHelper.GetEnumMemberValue(type));
            requestArgs.File = file;

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinAddMaterialResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 删除永久素材
        /// 可以删除公众号在公众平台官网素材管理模块中新建的图文消息、语音、视频等素材
        /// http://mp.weixin.qq.com/wiki/17/630699b9593f9a53725c26719d522101.html
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public static RequestApiResult RemoveMaterial(string access_token, string mediaId)
        {
            RequestApiResult result = new RequestApiResult();
            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/material/del_material?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(new { media_id = mediaId });

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiError = WeixinApiHelper.Parse(result.HttpRequestResult.Content);
            }

            return result;
        }

        /// <summary>
        /// 获取永久图文消息素材列表
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinGetArticleMaterialListResult> GetArticleMaterialList(string access_token, WeixinGetMaterialListArgs args)
        {
            args.Type = MaterialType.News;

            RequestApiResult<WeixinGetArticleMaterialListResult> result =
                new RequestApiResult<WeixinGetArticleMaterialListResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinGetArticleMaterialListResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 新增永久图文素材
        /// http://mp.weixin.qq.com/wiki/0/21cad57369e3652f6be542c1e3a531ec.htmls
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinAddArticleMaterialResult> AddArticleMaterial(string access_token, WeixinAddArticleMaterialArgs args)
        {
            RequestApiResult<WeixinAddArticleMaterialResult> result = new RequestApiResult<WeixinAddArticleMaterialResult>();
            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/material/add_news?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinAddArticleMaterialResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 修改永久图文素材
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestApiResult UpdateArticleMaterial(string access_token, WeixinUpdateArticleMaterialArgs args)
        {
            RequestApiResult result = new RequestApiResult();
            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/material/update_news?access_token={0}", access_token);
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
        /// 获取永久素材 图文消息
        /// http://mp.weixin.qq.com/wiki/12/3c12fac7c14cb4d0e0d4fe2fbc87b638.html
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinArticleMaterialListItemContent> GetArticleMaterial(string access_token, string mediaId)
        {
            RequestApiResult<WeixinArticleMaterialListItemContent> result =
                new RequestApiResult<WeixinArticleMaterialListItemContent>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/material/get_material?access_token={0}", access_token);
            requestArgs.Content = JsonConvert.SerializeObject(new { media_id = mediaId });

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinArticleMaterialListItemContent>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 上传图文消息内的图片获取URL
        /// 请注意，在图文消息的具体内容中，将过滤外部的图片链接，开发者可以通过下述接口上传图片得到URL，放到图文内容中使用。
        /// http://mp.weixin.qq.com/wiki/10/10ea5a44870f53d79449290dfd43d006.html
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="file"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static RequestApiResult<WeixinUploadImgResult> UploadImg(string access_token, string file)
        {
            RequestApiResult<WeixinUploadImgResult> result = new RequestApiResult<WeixinUploadImgResult>();
            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/media/uploadimg?access_token={0}", access_token);
            requestArgs.File = file;

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinUploadImgResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

    }
}
