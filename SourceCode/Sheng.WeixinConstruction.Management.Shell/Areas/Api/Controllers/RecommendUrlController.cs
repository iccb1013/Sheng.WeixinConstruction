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
    public class RecommendUrlController : BasalController
    {
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;
        private static readonly RecommendUrlManager _recommendUrlManager = RecommendUrlManager.Instance;

        public ActionResult SaveSettings()
        {
            RecommendUrlSettingsEntity args = RequestArgs<RecommendUrlSettingsEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            _recommendUrlManager.SaveSettings(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "保存会员多级推广设置"
            });

            #endregion

            return RespondResult();
        }
    }
}