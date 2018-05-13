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
    public class AdvertisingController : BasalController
    {
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly AdvertisingManager _advertisingManager = AdvertisingManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        public ActionResult CreateAdvertising()
        {
            AdvertisingEntity advertising = RequestArgs<AdvertisingEntity>();
            if (advertising == null)
            {
                return RespondResult(false, "参数无效。");
            }

            advertising.Domain = UserContext.User.Domain;
            advertising.AppId = DomainContext.AppId;
            advertising.CreateUser = UserContext.User.Id;
            _advertisingManager.CreateAdvertising(advertising);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Settings,
                Description = "创建广告"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateAdvertising()
        {
            AdvertisingEntity advertising = RequestArgs<AdvertisingEntity>();
            if (advertising == null)
            {
                return RespondResult(false, "参数无效。");
            }

            advertising.Domain = UserContext.User.Domain;
            advertising.AppId = DomainContext.AppId;
            _advertisingManager.UpdateAdvertising(advertising);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Settings,
                Description = "更新广告"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveAdvertising()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _advertisingManager.RemoveAdvertising(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Settings,
                Description = "删除广告"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetAdvertising()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            AdvertisingEntity advertising = _advertisingManager.GetAdvertising(Guid.Parse(id));

            return RespondDataResult(advertising);
        }

        public ActionResult GetAdvertisingList()
        {
            GetGetAdvertisingListArgs args = RequestArgs<GetGetAdvertisingListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _advertisingManager.GetAdvertisingList(args);
            return RespondDataResult(result);
        }
    }
}