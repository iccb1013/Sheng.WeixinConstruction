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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Areas.Api.Controllers
{
    public class SettingsController : BasalController
    {
        private static readonly ManagementDomainPool _domainPool = ManagementDomainPool.Instance;
        private static readonly SettingsManager _settingsManager = SettingsManager.Instance;
        private static readonly CachingService _cachingService = CachingService.Instance;
        private static readonly MaterialManager _materialManager = MaterialManager.Instance;
        private static readonly WxConfigurationSection _configuration = ConfigService.Instance.Configuration;
        private static readonly ThirdPartyManager _thirdPartyManager = ThirdPartyManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;
        private static readonly PortalTemplatePool _portalTemplatePool = PortalTemplatePool.Instance;

        private static readonly string _fileService = ConfigurationManager.AppSettings["FileService"];

        /// <summary>
        /// 作为第三方平台运营时，获取预授权码
        /// </summary>
        /// <returns></returns>
        [AllowedAnonymous]
        public ActionResult GetPreAuthCode()
        {
            RequestApiResult<WeixinThirdPartyGetPreAuthCodeResult> result =
                ThirdPartyApi.GetPreAuthCode(ThirdPartyAccessTokenGetter.Get(), _configuration.ThirdParty.AppId);

            if (result.Success)
            {
                return RespondDataResult(new
                {
                    AppId = _configuration.ThirdParty.AppId,
                    PreAuthCode = result.ApiResult.PreAuthCode
                });
            }
            else
            {
                ApiResult apiResult = new ApiResult();
                apiResult.Message = result.Message;
                return RespondResult(apiResult);
            }
        }

        public ActionResult UpdateAuthorizerAccountInfo()
        {
            //更新公众号帐户详细信息
            NormalResult normalResult =
                _thirdPartyManager.UpdateAuthorizerAccountInfo(DomainContext.Domain.Id, DomainContext.AppId);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Settings,
                Description = "更新公众号帐户详细信息"
            });

            #endregion

            ApiResult result = new ApiResult();
            result.Success = normalResult.Success;
            result.Message = normalResult.Message;
            return RespondResult(result);
        }

        public ActionResult SaveAndPublishMenu()
        {
            StreamReader reader = new StreamReader(HttpContext.Request.InputStream);
            string inputString = reader.ReadToEnd();

            ApiResult result = _settingsManager.SaveAndPublishMenu(inputString, this.UserContext, DomainContext);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Settings,
                Description = "保存并发布公众号菜单"
            });

            #endregion

            return RespondResult(result);
        }

        public ActionResult GetMenu()
        {
            MenuEntity menuEntity = _settingsManager.GetMenu(this.UserContext.User.Domain);
            return RespondDataResult(menuEntity);
        }

        public ActionResult SaveSettings()
        {
            SaveSettingsArgs args = RequestArgs<SaveSettingsArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Settings.Domain = DomainContext.Domain.Id;
            args.Settings.AppId = DomainContext.AppId;
            _settingsManager.SaveSettings(DomainContext.Domain.Id, args.Settings);

            args.ThemeStyleSettings.Domain = DomainContext.Domain.Id;
            args.ThemeStyleSettings.AppId = DomainContext.AppId;
            _settingsManager.SaveThemeStyleSettings(DomainContext.Domain.Id, args.ThemeStyleSettings);

            //确保保存设置后能立马在微信后台保存设置
            _domainPool.Refresh(UserContext.User.Domain);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Settings,
                Description = "保存基本设置"
            });

            #endregion

            return RespondResult();
        }

     

        #region AutoReply

        public ActionResult SaveAutoReplyOnSubscribe()
        {
            AutoReplyOnSubscribeEntity args = RequestArgs<AutoReplyOnSubscribeEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;
            _settingsManager.SaveAutoReplyOnSubscribe(DomainContext, args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Settings,
                Description = "保存关注时自动回复"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetAutoReplyOnSubscribe()
        {
            AutoReplyOnSubscribeEntity result =
                _settingsManager.GetAutoReplyOnSubscribe(DomainContext.Domain.Id);
            return RespondDataResult(result);
        }

        public ActionResult SaveAutoReplyOnMessage()
        {
            AutoReplyOnMessageEntity args = RequestArgs<AutoReplyOnMessageEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;
            _settingsManager.SaveAutoReplyOnMessage(DomainContext, args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Settings,
                Description = "保存消息自动回复"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetAutoReplyOnMessage()
        {
            AutoReplyOnMessageEntity result =
                _settingsManager.GetAutoReplyOnMessage(DomainContext.Domain.Id);
            return RespondDataResult(result);
        }

        public ActionResult AddAutoReplyOnKeyWordsRule()
        {
            AutoReplyOnKeyWordsRuleEntity args = RequestArgs<AutoReplyOnKeyWordsRuleEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;
            _settingsManager.AddAutoReplyOnKeyWordsRule(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Settings,
                Description = "添加关键词自动回复"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateAutoReplyOnKeyWordsRule()
        {
            AutoReplyOnKeyWordsRuleEntity args = RequestArgs<AutoReplyOnKeyWordsRuleEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;
            _settingsManager.UpdateAutoReplyOnKeyWordsRule(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Settings,
                Description = "更新关键词自动回复"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveAutoReplyOnKeyWordsRule()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _settingsManager.RemoveAutoReplyOnKeyWordsRule(DomainContext.Domain.Id, Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Settings,
                Description = "删除关键词自动回复"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetAutoReplyOnKeyWordsRule()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            AutoReplyOnKeyWordsRuleEntity rule = _settingsManager.GetAutoReplyOnKeyWordsRule(Guid.Parse(id));

            return RespondDataResult(rule);
        }

        public ActionResult GetAutoReplyOnKeyWordsRuleList()
        {
            List<AutoReplyOnKeyWordsRuleEntity> result =
                _settingsManager.GetAutoReplyOnKeyWordsRuleList(DomainContext.Domain.Id);
            return RespondDataResult(result);
        }

        #endregion

        public ActionResult GetFileUploadResult()
        {
            HttpRequestBase request = this.HttpContext.Request;

            string strFileId = request.QueryString["fileId"];

            string cachingKey = "fsUploadResult:" + strFileId;

            FileUploadResult fileUploadResult = _cachingService.Get<FileUploadResult>(cachingKey);

            if (fileUploadResult == null)
            {
                return RespondResult(false, "指定的上传信息不存在。");
            }

            if (fileUploadResult.Success)
            {
                return RespondDataResult(fileUploadResult);
            }
            else
            {
                return RespondResult(false, fileUploadResult.Message);
            }
        }

        public ActionResult GetUploadToWeixinMaterialResult()
        {
            HttpRequestBase request = this.HttpContext.Request;

            string strFileId = request.QueryString["fileId"];

            string cachingKey = "fsUploadResult:" + strFileId;

            ApiResult<WeixinAddMaterialResult> addMaterialResult = _cachingService.Get<ApiResult<WeixinAddMaterialResult>>(cachingKey);
            ApiResult apiResult = new ApiResult();

            if (addMaterialResult == null)
            {
                apiResult.Message = "指定的上传信息不存在。";
            }
            else if (addMaterialResult.Success == false)
            {
                apiResult.Message = addMaterialResult.Message;
            }
            else
            {
                apiResult.Success = true;
                NormalMaterialEntity normalMaterial = new NormalMaterialEntity();
                normalMaterial.Domain = DomainContext.Domain.Id;
                normalMaterial.AppId = DomainContext.AppId;
                normalMaterial.MediaId = addMaterialResult.Data.MediaId;
                normalMaterial.WeixinUrl = addMaterialResult.Data.Url;
                normalMaterial.TypeString = request.QueryString["type"];
                normalMaterial.Name = Path.GetFileName(addMaterialResult.Data.FileName);
                normalMaterial.Url = _fileService + addMaterialResult.Message;
                normalMaterial.OperatorUser = UserContext.User.Id;
                _materialManager.AddNormalMaterial(normalMaterial);
            }

            return RespondResult(apiResult);
        }

        public ActionResult GetUploadToWeixinImgResult()
        {
            HttpRequestBase request = this.HttpContext.Request;

            string strFileId = request.QueryString["fileId"];

            string cachingKey = "fsUploadResult:" + strFileId;

            UploadToWeixinImgResult uploadToWeixinImgResult = _cachingService.Get<UploadToWeixinImgResult>(cachingKey);

            if (uploadToWeixinImgResult == null)
            {
                return RespondResult(false, "指定的上传信息不存在。");
            }

            if (uploadToWeixinImgResult.Success)
            {
                return RespondDataResult(uploadToWeixinImgResult);
            }
            else
            {
                return RespondResult(false, uploadToWeixinImgResult.Message);
            }
        }

        public ActionResult SyncMember()
        {
            DomainContext.SyncMember();

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "同步用户信息"
            });

            #endregion

            return RespondResult();
        }
    }
}