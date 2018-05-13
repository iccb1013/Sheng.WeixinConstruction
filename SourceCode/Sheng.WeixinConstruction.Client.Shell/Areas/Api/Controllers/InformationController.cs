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
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Areas.Api.Controllers
{
    public class InformationController : ApiBasalController
    {
        private static readonly InformationManager _informationManager = InformationManager.Instance;

        [AllowedAnonymous]
        public ActionResult GetInformation()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            InformationEntity information = _informationManager.GetInformation(Guid.Parse(id));

            return RespondDataResult(information);
        }

        [AllowedAnonymous]
        public ActionResult GetInformationList()
        {
            GetItemListArgs args = RequestArgs<GetItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _informationManager.GetInformationList(args);
            return RespondDataResult(result);
        }

        [AllowedAnonymous]
        public ActionResult GetCategory()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            InformationCategoryEntity category = _informationManager.GetCategory(Guid.Parse(id));

            return RespondDataResult(category);
        }

        [AllowedAnonymous]
        public ActionResult GetCategoryList()
        {
            GetCategoryListArgs args = RequestArgs<GetCategoryListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult result = _informationManager.GetCategoryList(args);
            return RespondDataResult(result);
        }

        [AllowedAnonymous]
        public ActionResult GetInformationItem()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            InformationItemEntity item = _informationManager.GetInformationItem(Guid.Parse(id));

            return RespondDataResult(item);
        }

        [AllowedAnonymous]
        public ActionResult GetInformationItemList()
        {
            GetInformationItemListArgs args = RequestArgs<GetInformationItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult result = _informationManager.GetInformationItemList(args);
            return RespondDataResult(result);
        }

	}
}