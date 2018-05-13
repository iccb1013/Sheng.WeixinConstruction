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
    public class CustomFormController : BasalController
    {
        private static readonly CustomFormManager _customFormManager = CustomFormManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        public ActionResult CreateCustomForm()
        {
            CustomFormEntity customFormEntity = RequestArgs<CustomFormEntity>();
            if (customFormEntity == null)
            {
                return RespondResult(false, "参数无效。");
            }

            customFormEntity.Domain = UserContext.User.Domain;
            customFormEntity.AppId = DomainContext.AppId;
            customFormEntity.CreateTime = DateTime.Now;
            customFormEntity.CreateUser = UserContext.User.Id;
            _customFormManager.CreateCustomForm(customFormEntity);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.CustomForm,
                Description = "创建自定义表单"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateCustomForm()
        {
            CustomFormEntity customFormEntity = RequestArgs<CustomFormEntity>();
            if (customFormEntity == null)
            {
                return RespondResult(false, "参数无效。");
            }

            customFormEntity.Domain = UserContext.User.Domain;
            customFormEntity.AppId = DomainContext.AppId;
            _customFormManager.UpdateCustomForm(customFormEntity);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.CustomForm,
                Description = "更新自定义表单"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveCustomForm()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            _customFormManager.RemoveCustomForm(id);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.CustomForm,
                Description = "删除自定义表单"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetCustomForm()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            CustomFormEntity customForm = _customFormManager.GetCustomForm(id);

            return RespondDataResult(customForm);
        }

        public ActionResult GetCustomFormList()
        {
            GetCustomFormListArgs args = RequestArgs<GetCustomFormListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _customFormManager.GetCustomFormList(args);
            return RespondDataResult(result);
        }

        public ActionResult RemoveCustomFormContent()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

           // _customFormManager.RemoveCustomFormContent(id);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.CustomForm,
                Description = "删除自定义表单内容"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetCustomFormContentList()
        {
            GetCustomFormContentListArgs args = RequestArgs<GetCustomFormContentListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _customFormManager.GetCustomFormContentList(args);
            return RespondDataResult(result);
        }

    }
}