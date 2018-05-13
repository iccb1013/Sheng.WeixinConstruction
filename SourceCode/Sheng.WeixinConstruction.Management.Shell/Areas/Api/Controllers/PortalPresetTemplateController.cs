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
    public class PortalPresetTemplateController : BasalController
    {
        private static readonly PortalPresetTemplateManager _portalPresetTemplateManager = PortalPresetTemplateManager.Instance;

        public ActionResult CreateTemplate()
        {
            PortalPresetTemplateEntity template = RequestArgs<PortalPresetTemplateEntity>();
            if (template == null)
            {
                return RespondResult(false, "参数无效。");
            }

            _portalPresetTemplateManager.CreateTemplate(template);

            return RespondResult();
        }

        public ActionResult UpdateTemplate()
        {
            PortalPresetTemplateEntity template = RequestArgs<PortalPresetTemplateEntity>();
            if (template == null)
            {
                return RespondResult(false, "参数无效。");
            }

            _portalPresetTemplateManager.UpdateTemplate(template);

            return RespondResult();
        }

        public ActionResult RemoveTemplate()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _portalPresetTemplateManager.RemoveTemplate(Guid.Parse(id));

            return RespondResult();
        }

        public ActionResult GetTemplate()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            PortalPresetTemplateEntity template = _portalPresetTemplateManager.GetTemplate(Guid.Parse(id));

            return RespondDataResult(template);
        }

        public ActionResult GetTemplateDigestList()
        {
            GetItemListArgs args = RequestArgs<GetItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult result = _portalPresetTemplateManager.GetTemplateDigestList(args);
            return RespondDataResult(result);
        }
    }
}