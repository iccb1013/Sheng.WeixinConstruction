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


using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Areas.Api.Controllers
{
    public class UserController : BasalController
    {
        private static readonly UserManager _userManager = UserManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;
        private static readonly CachingService _cachingService = CachingService.Instance;

        [AllowedAnonymous]
        public ActionResult Register()
        {
            UserRegisterArgs args = RequestArgs<UserRegisterArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            string mobileValidateCode = _cachingService.Get(args.MobilePhone);
            if (String.IsNullOrEmpty(mobileValidateCode))
            {
                return RespondResult(false, "手机验证码已过期，请重新获取。");
            }

            if (mobileValidateCode != args.MobilePhoneValidateCode)
            {
                return RespondResult(false, "手机验证码无效。");
            }

            //if (Session["ValidateCode"] == null ||
            //    Session["ValidateCode"].ToString() != args.ValidateCode)
            //{
            //    return RespondResult(false, "验证码无效。");
            //}

            UserRegisterResult result = _userManager.Register(args);

            if (result.Result == UserRegisterResultEnum.Success)
            {
                //UserContext userContext = new UserContext(result.User, result.Domain);
                UserContext userContext = new UserContext(result.User);
                SessionContainer.SetUserContext(this.HttpContext, userContext);
                return RespondResult();
            }
            else
            {
                ApiResult apiResult = new ApiResult()
                {
                    Success = false
                };
                switch (result.Result)
                {
                    case UserRegisterResultEnum.Unknow:
                        apiResult.Message = "未知错误。";
                        break;
                    case UserRegisterResultEnum.AccountInUse:
                        apiResult.Message = "帐户被占用，请尝试其它帐户名称。";
                        break;
                    case UserRegisterResultEnum.UserInfoInvalid:
                        apiResult.Message = "帐户被占用，用户信息无效。";
                        break;
                    default:
                        Debug.Assert(false, "未捕获的状态。");
                        break;
                }
                return RespondResult(apiResult);
            }
        }

        [AllowedAnonymous]
        /// <summary>
        /// 忘记密码功能
        /// 生成随机密码并发送到邮箱
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetPassword()
        {
            ResetPasswordArgs args = RequestArgs<ResetPasswordArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            if (String.IsNullOrEmpty(args.Account) || String.IsNullOrEmpty(args.Email))
            {
                return RespondResult(false, "登录账户或邮件地址为空。");
            }

            bool successful = _userManager.ResetPassword(args);
            if (successful)
            {
                return RespondResult();
            }
            else
            {
                ApiResult apiResult = new ApiResult()
                {
                    Success = false
                };
                apiResult.Message = "请检查您输入的帐户及电子邮件地址是否正确。";
                return RespondResult(apiResult);
            }
        }

        #region User

        public ActionResult GetUserList()
        {
            GetUserListArgs args = RequestArgs<GetUserListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _userManager.GetUserList(args);
            return RespondDataResult(result);
        }

        public ActionResult GetUser()
        {

            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            UserEntity user = _userManager.GetUser(id);

            return RespondDataResult(user);
        }

        public ActionResult CreateUser()
        {
            UserEntity user = RequestArgs<UserEntity>();
            if (user == null)
            {
                return RespondResult(false, "参数无效。");
            }

            user.Domain = UserContext.User.Domain;
            NormalResult result = _userManager.CreateUser(user);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.User,
                Description = "添加用户"
            });

            #endregion

            return RespondDataResult(result);
        }

        public ActionResult UpdateUser()
        {
            UserEntity user = RequestArgs<UserEntity>();
            if (user == null)
            {
                return RespondResult(false, "参数无效。");
            }

            NormalResult result = _userManager.UpdateUser(user);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.User,
                Description = "更新用户"
            });

            #endregion

            return RespondDataResult(result);
        }

        public ActionResult RemoveUser()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            if (id == UserContext.User.Id)
            {
                return RespondResult(false, "不能删除当前登录的用户自己。");
            }

            NormalResult result = _userManager.RemoveUser(id);

            #region 操作日志
            if (result.Success)
            {
                _operatedLogManager.Create(new OperatedLogEntity()
                {
                    Domain = DomainContext.Domain.Id,
                    AppId = DomainContext.AppId,
                    User = UserContext.User.Id,
                    IP = Request.UserHostAddress,
                    Module = EnumModule.User,
                    Description = "删除用户"
                });
            }

            #endregion

            return RespondDataResult(result);
        }

        public ActionResult UpdatePassword()
        {
            UpdatePasswordArgs args = RequestArgs<UpdatePasswordArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            bool success = _userManager.UpdatePassword(UserContext.User.Id, args);

            return RespondResult(success, String.Empty);
        }

        public ActionResult Update()
        {
            UserEntity args = RequestArgs<UserEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            _userManager.Update(UserContext.User.Id, args);

            UserContext.User = _userManager.GetUser(UserContext.User.Id);

            return RespondResult();
        }

        #endregion
    }
}