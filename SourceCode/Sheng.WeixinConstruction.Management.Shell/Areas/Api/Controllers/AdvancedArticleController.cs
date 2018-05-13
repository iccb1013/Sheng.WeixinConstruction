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


using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Areas.Api.Controllers
{
    public class AdvancedArticleController : BasalController
    {
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly AdvancedArticleManager _advancedArticleManager = AdvancedArticleManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        public ActionResult CreateAdvancedArticle()
        {
            AdvancedArticleEntity advancedArticle = RequestArgs<AdvancedArticleEntity>();
            if (advancedArticle == null)
            {
                return RespondResult(false, "参数无效。");
            }

            advancedArticle.Domain = UserContext.User.Domain;
            advancedArticle.AppId = DomainContext.AppId;
            advancedArticle.CreateUser = UserContext.User.Id;
            _advancedArticleManager.CreateAdvancedArticle(advancedArticle);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.AdvancedArticle,
                Description = "创建高级图文"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateAdvancedArticle()
        {
            AdvancedArticleEntity advancedArticle = RequestArgs<AdvancedArticleEntity>();
            if (advancedArticle == null)
            {
                return RespondResult(false, "参数无效。");
            }

            advancedArticle.Domain = UserContext.User.Domain;
            advancedArticle.AppId = DomainContext.AppId;
            _advancedArticleManager.UpdateAdvancedArticle(advancedArticle);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.AdvancedArticle,
                Description = "更新增强图文"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveAdvancedArticle()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _advancedArticleManager.RemoveAdvancedArticle(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.AdvancedArticle,
                Description = "删除增强图文件"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetAdvancedArticle()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            AdvancedArticleEntity advancedArticle = _advancedArticleManager.GetAdvancedArticle(Guid.Parse(id));

            return RespondDataResult(advancedArticle);
        }

        public ActionResult GetAdvancedArticleList()
        {
            GetItemListArgs args = RequestArgs<GetItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _advancedArticleManager.GetAdvancedArticleList(args);
            return RespondDataResult(result);
        }


    }
}