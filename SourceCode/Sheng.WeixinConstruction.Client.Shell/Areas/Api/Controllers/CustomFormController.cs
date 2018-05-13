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


using Sheng.WeixinConstruction.Client.Core;
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
    public class CustomFormController : ApiBasalController
    {
        private static readonly CustomFormManager _customFormManager = CustomFormManager.Instance;

        public ActionResult SaveCustomFormContent()
        {
            SaveCustomFormContentArgs args = RequestArgs<SaveCustomFormContentArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.MemberId = MemberContext.Member.Id;

            CustomFormContentEntity contentArgs = new CustomFormContentEntity();
            if (args.ContentId.HasValue)
            {
                contentArgs.Id = args.ContentId.Value;
            }
            contentArgs.Domain = DomainContext.Domain.Id;
            contentArgs.AppId = DomainContext.AppId;
            contentArgs.Form = args.FormId;
            contentArgs.MemberOpenId = this.OpenId;
            contentArgs.FillinTime = DateTime.Now;
            contentArgs.Status = EnumCustomFormContentStatus.Unprocessed;

            _customFormManager.SaveCustomFormContent(contentArgs, args);
            SessionContainer.ClearMemberContext(this.HttpContext);

            return RespondResult();
        }

    }
}