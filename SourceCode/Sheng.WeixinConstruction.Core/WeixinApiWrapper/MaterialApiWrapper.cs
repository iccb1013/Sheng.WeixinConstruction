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
    public class MaterialApiWrapper
    {
        private static LogService _log = LogService.Instance;

        public static RequestApiResult<WeixinGetNormalMaterialListResult> GetNormalMaterialList(DomainContext domainContext, WeixinGetMaterialListArgs args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinGetNormalMaterialListResult> result = MaterialApi.GetNormalMaterialList(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = MaterialApi.GetNormalMaterialList(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("MaterialApi.GetNormalMaterialList 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("MaterialApi.GetNormalMaterialList 失败",
                               result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult<WeixinAddMaterialResult> AddNormalMaterial(DomainContext domainContext, string file, MaterialType type)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinAddMaterialResult> result = MaterialApi.AddNormalMaterial(accessToken, file, type);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = MaterialApi.AddNormalMaterial(accessToken, file, type);
                    if (result.Success == false)
                    {
                        _log.Write("MaterialApi.AddNormalMaterial 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("MaterialApi.AddNormalMaterial 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult RemoveMaterial(DomainContext domainContext, string mediaId)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult result = MaterialApi.RemoveMaterial(accessToken, mediaId);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = MaterialApi.RemoveMaterial(accessToken, mediaId);
                    if (result.Success == false)
                    {
                        _log.Write("MaterialApi.RemoveMaterial 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("MaterialApi.RemoveMaterial 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult<WeixinGetArticleMaterialListResult> GetArticleMaterialList(DomainContext domainContext, WeixinGetMaterialListArgs args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinGetArticleMaterialListResult> result = MaterialApi.GetArticleMaterialList(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = MaterialApi.GetArticleMaterialList(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("MaterialApi.GetArticleMaterialList 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("MaterialApi.GetArticleMaterialList 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        //WeixinAddArticleMaterialArgs
        public static RequestApiResult<WeixinAddArticleMaterialResult> AddArticleMaterial(DomainContext domainContext, WeixinAddArticleMaterialArgs args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinAddArticleMaterialResult> result = MaterialApi.AddArticleMaterial(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = MaterialApi.AddArticleMaterial(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("MaterialApi.AddArticleMaterial 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("MaterialApi.AddArticleMaterial 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult UpdateArticleMaterial(DomainContext domainContext, WeixinUpdateArticleMaterialArgs args)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult result = MaterialApi.UpdateArticleMaterial(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = MaterialApi.UpdateArticleMaterial(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("MaterialApi.UpdateArticleMaterial 失败",
                             result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("MaterialApi.UpdateArticleMaterial 失败",
                             result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }
        
        public static RequestApiResult<WeixinArticleMaterialListItemContent> GetArticleMaterial(DomainContext domainContext, string mediaId)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinArticleMaterialListItemContent> result = MaterialApi.GetArticleMaterial(accessToken, mediaId);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = MaterialApi.GetArticleMaterial(accessToken, mediaId);
                    if (result.Success == false)
                    {
                        _log.Write("MaterialApi.GetArticleMaterial 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("MaterialApi.GetArticleMaterial 失败",
                           result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

        public static RequestApiResult<WeixinUploadImgResult> UploadImg(DomainContext domainContext, string file)
        {
            string accessToken = domainContext.AccessToken;
            RequestApiResult<WeixinUploadImgResult> result = MaterialApi.UploadImg(accessToken, file);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = AccessTokenGetter.Refresh(domainContext.AppId, accessToken);
                    }

                    result = MaterialApi.UploadImg(accessToken, file);
                    if (result.Success == false)
                    {
                        _log.Write("MaterialApi.UploadImg 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("MaterialApi.UploadImg 失败",
                            result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }

    }
}
