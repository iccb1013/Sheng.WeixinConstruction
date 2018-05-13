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


using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Areas.Api.Controllers
{
    public class MaterialController : BasalController
    {
        private static readonly MaterialManager _materialManager = MaterialManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        public ActionResult GetNormalMaterialList()
        {
            GetMaterialListArgs args = RequestArgs<GetMaterialListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult<NormalMaterialEntity> result =
                _materialManager.GetNormalMaterialList(UserContext.User.Domain,DomainContext.AppId, args);
            return RespondDataResult(result);
        }

        public ActionResult GetNormalMaterial()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            NormalMaterialEntity normalMaterial = _materialManager.GetNormalMaterial(Guid.Parse(id));

            return RespondDataResult(normalMaterial);
        }

        public ActionResult RemoveNormalMaterial()
        {
            string id = Request.QueryString["id"];
            string mediaId = Request.QueryString["mediaId"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            string result = _materialManager.RemoveNormalMaterial(DomainContext, Guid.Parse(id), mediaId);

            ApiResult apiResult = new ApiResult();
            if (String.IsNullOrEmpty(result))
            {
                apiResult.Success = true;
            }
            else
            {
                apiResult.Message = result;
            }

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Material,
                Description = "删除素材"
            });

            #endregion

            return RespondResult(apiResult);
        }

        public ActionResult AddArticleMaterial()
        {
            ArticleMaterialEntity args = RequestArgs<ArticleMaterialEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.OperatorUser = UserContext.User.Id;

            AddArticleMaterialResult result = _materialManager.AddArticleMaterial(DomainContext, args);

            ApiResult<AddArticleMaterialResult> apiResult = new ApiResult<AddArticleMaterialResult>();
            apiResult.Success = result.Success;
            apiResult.Message = result.Message;
            apiResult.Data = result;

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Material,
                Description = "添加图文素材"
            });

            #endregion

            return RespondResult(apiResult);
        }

        public ActionResult PublishArticleMaterial()
        {
            string id = Request.QueryString["id"];
            string mediaId = Request.QueryString["mediaId"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            PublishArticleMaterialResult result = 
                _materialManager.PublishArticleMaterial(DomainContext, Guid.Parse(id));

            ApiResult<PublishArticleMaterialResult> apiResult = new ApiResult<PublishArticleMaterialResult>();
            apiResult.Success = result.Success;
            apiResult.Message = result.Message;
            apiResult.Data = result;

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Material,
                Description = "发布图文素材到微信后台"
            });

            #endregion

            return RespondResult(apiResult);
        }

        public ActionResult UpdateArticleMaterial()
        {
            ArticleMaterialEntity args = RequestArgs<ArticleMaterialEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;
            args.OperatorUser = UserContext.User.Id;

            string result = null;
            if (args.WeixinStatus == 0)
            {
                result = _materialManager.UpdateArticleMaterial(DomainContext, args);
            }
            else if (args.WeixinStatus == 1 || args.WeixinStatus == 2)
            {
                result = _materialManager.UpdateArticleMaterialItem(DomainContext, args);
            }
            else
            {
                Debug.Assert(false, "未知 args.WeixinStatus");
            }

            ApiResult apiResult = new ApiResult();
            if (String.IsNullOrEmpty(result))
            {
                apiResult.Success = true;
            }
            else
            {
                apiResult.Message = result;
            }

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Material,
                Description = "更新图文素材"
            });

            #endregion

            return RespondResult(apiResult);
        }

        public ActionResult GetArticleMaterial()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            ArticleMaterialEntity articleMaterial = _materialManager.GetArticleMaterial(Guid.Parse(id));

            return RespondDataResult(articleMaterial);
        }

        public ActionResult GetArticleMaterialList()
        {
            GetArticleMaterialListArgs args = RequestArgs<GetArticleMaterialListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult<ArticleMaterialEntity> result =
                _materialManager.GetArticleMaterialList(UserContext.User.Domain,DomainContext.AppId, args);
            return RespondDataResult(result);
        }

        public ActionResult RemoveArticleMaterial()
        {
            string id = Request.QueryString["id"];
            string mediaId = Request.QueryString["mediaId"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            string result = _materialManager.RemoveArticleMaterial(DomainContext, Guid.Parse(id), mediaId);

            ApiResult apiResult = new ApiResult();
            if (String.IsNullOrEmpty(result))
            {
                apiResult.Success = true;
            }
            else
            {
                apiResult.Message = result;
            }

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Material,
                Description = "删除图文素材"
            });

            #endregion

            return RespondResult(apiResult);
        }

    }
}