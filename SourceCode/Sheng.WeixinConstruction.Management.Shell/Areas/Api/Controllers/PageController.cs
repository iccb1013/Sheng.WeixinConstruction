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
    public class PageController : BasalController
    {
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly PageManager _pageManager = PageManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        public ActionResult CreatePage()
        {
            PageEntity page = RequestArgs<PageEntity>();
            if (page == null)
            {
                return RespondResult(false, "参数无效。");
            }

            page.Domain = UserContext.User.Domain;
            page.AppId = DomainContext.AppId;
            page.CreateUser = UserContext.User.Id;
            _pageManager.CreatePage(page);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Page,
                Description = "创建自定义页面"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdatePage()
        {
            PageEntity page = RequestArgs<PageEntity>();
            if (page == null)
            {
                return RespondResult(false, "参数无效。");
            }

            page.Domain = UserContext.User.Domain;
            page.AppId = DomainContext.AppId;
            _pageManager.UpdatePage(page);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Page,
                Description = "更新自定义页面"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemovePage()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _pageManager.RemovePage(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Page,
                Description = "删除自定义页面"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetPage()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            PageEntity page = _pageManager.GetPage(Guid.Parse(id));

            return RespondDataResult(page);
        }

        public ActionResult GetPageList()
        {
            GetItemListArgs args = RequestArgs<GetItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _pageManager.GetPageList(args);
            return RespondDataResult(result);
        }


    }
}