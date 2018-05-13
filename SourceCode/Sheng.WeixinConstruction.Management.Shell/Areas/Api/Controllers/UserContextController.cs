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
using Sheng.WeixinConstruction.Management.Core;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Areas.Api.Controllers
{
    public class UserContextController : BasalController
    {
        private static readonly UserManager _userManager = UserManager.Instance;
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;
        private static readonly ControlledCachingService _controlledCachingService = ControlledCachingService.Instance;
        private static readonly ManagementDomainPool _domainPool = ManagementDomainPool.Instance;
        private static readonly string _fileService = ConfigurationManager.AppSettings["FileService"];

        [AllowedAnonymous]
        public ActionResult Login()
        {
            LoginArgs args = RequestArgs<LoginArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            UserEntity user = _userManager.Verify(args.Account, args.Password);
            if (user == null)
            {
                return RespondResult(false, "帐号或密码错误。");
            }

            UserContext userContext = new UserContext(user);
            SessionContainer.SetUserContext(this.HttpContext, userContext);
            this.UserContext = userContext;
            this.DomainContext = _domainPool.GetDomainContext(UserContext.User.Domain);

            //操作日志
            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = userContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.System,
                Description = "用户登陆"
            });

            return RespondResult();
        }


        [AllowedAnonymous]
        public ActionResult GetValidateCode()
        {
            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateValidateCode(5);
            Session["ValidateCode"] = code;
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }

        /// <summary>
        /// 心跳，用于填写界面维持session
        /// </summary>
        /// <returns></returns>
        public ActionResult Heartbeat()
        {
            return RespondResult();
        }

        /// <summary>
        /// 获取用于文件上传的参数
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFileUploadParameter()
        {
            //注意，必须返回一个新的GUID用作文件ID
            FileUploadParameter parameter = new FileUploadParameter();
            parameter.UserId = UserContext.User.Id;
            parameter.DomainId = UserContext.User.Domain;
            parameter.FileService = _fileService;
            return RespondDataResult(parameter);
        }
    }
}