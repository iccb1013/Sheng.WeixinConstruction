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
using Newtonsoft.Json.Linq;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Management.Core;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using Sheng.WeixinConstruction.WeixinContract.ThirdParty;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Areas.Api.Controllers
{
    public class PortalController : BasalController
    {
        private static readonly ManagementDomainPool _domainPool = ManagementDomainPool.Instance;
        private static readonly SettingsManager _settingsManager = SettingsManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;
        private static readonly PortalTemplatePool _portalTemplatePool = PortalTemplatePool.Instance;
        private static readonly PortalManager _portalManager = PortalManager.Instance;


        public ActionResult SavePortalStyleSettings_Template()
        {
            PortalStyleSettingsEntity args = RequestArgs<PortalStyleSettingsEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.PortalMode = EnumPortalMode.Template;
            _portalManager.SavePortalStyleSettings_Template(DomainContext.Domain.Id, args);

            //确保保存设置后能立马在微信后台保存设置
            _domainPool.Refresh(UserContext.User.Domain);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Portal,
                Description = "保存微主页设置"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult SavePortalStyleSettings_Custom()
        {
            PortalStyleSettingsEntity args = RequestArgs<PortalStyleSettingsEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.PortalMode = EnumPortalMode.Custom;
            _portalManager.SavePortalStyleSettings_Custom(DomainContext.Domain.Id, args);

            //确保保存设置后能立马在微信后台保存设置
            _domainPool.Refresh(UserContext.User.Domain);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Portal,
                Description = "保存微主页设置"
            });

            #endregion

            return RespondResult();
        }

        //public ActionResult GetPortalTemplateDigest()
        //{
        //    string id = Request.QueryString["id"];

        //    if (String.IsNullOrEmpty(id))
        //    {
        //        return RespondResult(false, "参数无效。");
        //    }

        //    PortalPresetTemplateEntity template = _portalTemplatePool.GetPortalPresetTemplateDigest(Guid.Parse(id));

        //    return RespondDataResult(template);
        //}

        #region 快捷菜单

        public ActionResult GetShortcutMenu()
        {
            ShortcutMenuEntity menuEntity = _portalManager.GetShortcutMenu(this.DomainContext.Domain.Id, this.DomainContext.AppId);
            return RespondDataResult(menuEntity);
        }

        public ActionResult SaveShortcutMenu()
        {
            StreamReader reader = new StreamReader(HttpContext.Request.InputStream);
            string inputString = reader.ReadToEnd();

            Debug.WriteLine(inputString);

            _portalManager.SaveShortcutMenu(inputString, this.UserContext, DomainContext);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Portal,
                Description = "更新快捷菜单"
            });

            #endregion

            return RespondResult();
        }

        #endregion
    }
}