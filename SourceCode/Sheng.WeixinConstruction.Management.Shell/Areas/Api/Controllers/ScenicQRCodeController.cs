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
    public class ScenicQRCodeController : BasalController
    {
        private static readonly ScenicQRCodeManager _scenicQRCodeManager = ScenicQRCodeManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        public ActionResult Create()
        {
            ScenicQRCodeEntity scenicQRCodeEntity = RequestArgs<ScenicQRCodeEntity>();
            if (scenicQRCodeEntity == null)
            {
                return RespondResult(false, "参数无效。");
            }

            scenicQRCodeEntity.Domain = UserContext.User.Domain;
            scenicQRCodeEntity.AppId = DomainContext.AppId;
            scenicQRCodeEntity.CreateTime = DateTime.Now;
            scenicQRCodeEntity.CreateUser = UserContext.User.Id;
            NormalResult result = _scenicQRCodeManager.Create(this.DomainContext, scenicQRCodeEntity);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.ScenicQRCode,
                Description = "添加场景二维码"
            });

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult Update()
        {
            ScenicQRCodeEntity scenicQRCodeEntity = RequestArgs<ScenicQRCodeEntity>();
            if (scenicQRCodeEntity == null)
            {
                return RespondResult(false, "参数无效。");
            }

            scenicQRCodeEntity.Domain = UserContext.User.Domain;
            scenicQRCodeEntity.AppId = DomainContext.AppId;
            NormalResult result = _scenicQRCodeManager.Update(scenicQRCodeEntity);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.ScenicQRCode,
                Description = "更新场景二维码"
            });

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult Remove()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _scenicQRCodeManager.Remove(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.ScenicQRCode,
                Description = "删除场景二维码"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetScenicQRCodeList()
        {
            GetScenicQRCodeListArgs args = RequestArgs<GetScenicQRCodeListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _scenicQRCodeManager.GetScenicQRCodeList(args);
            return RespondDataResult(result);
        }
	}
}