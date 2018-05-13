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
using Sheng.WeixinConstruction.Client.Core;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Areas.Api.Controllers
{
    public class MemberController : ApiBasalController
    {
        private static readonly FileService _fileService = FileService.Instance;
        private static readonly LogService _log = LogService.Instance;

        /// <summary>
        /// 每日签到
        /// </summary>
        /// <returns></returns>
        public ActionResult SignIn()
        {
            SignInResult result = _memberManager.SignIn(MemberContext.Member.Domain, DomainContext.AppId, MemberContext.Member.Id);
            if (result.Success)
            {
                MemberContext.Member.SignInDate = DateTime.Now;
            }
            return RespondDataResult(result);
        }

        public ActionResult GetPointAccount()
        {
            GetPointAccountArgs args = RequestArgs<GetPointAccountArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.MemberId = MemberContext.Member.Id;

            GetItemListResult result = _memberManager.GetPointAccount(args);
            return RespondDataResult(result);
        }

        public ActionResult UpdatePersonalInfo()
        {
            UpdatePersonalInfoArgs args = RequestArgs<UpdatePersonalInfoArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.MemberId = MemberContext.Member.Id;

            _memberManager.UpdatePersonalInfo(args);
            SessionContainer.ClearMemberContext(this.HttpContext);

            return RespondResult();
        }

        public ActionResult GetMemberCenterQRCode()
        {
            string url = MemberContext.Member.MemberCenterQRCodeImageUrl;

            if (String.IsNullOrEmpty(url))
            {
                url = String.Format(_settingsManager.GetClientAddress(DomainContext) +
                    "Home/QRCode/{0}?type=member&memberId={1}&cardNumber={2}",
                    DomainContext.Domain.Id, MemberContext.Member.Id, MemberContext.Member.CardNumber);

                //GetQRCodeImageArgs getQRCodeImageArgs = new GetQRCodeImageArgs();
                //getQRCodeImageArgs.Content = content;
                //getQRCodeImageArgs.Domain = DomainContext.Domain.Id;
                //GetQRCodeImageResult getQRCodeImageResult = _fileService.GetQRCodeImage(getQRCodeImageArgs);

                //if (getQRCodeImageResult.Success == false)
                //{
                //    return RespondResult(false, "生成二维码失败：" + getQRCodeImageResult.Message);
                //}

                //url = _fileService.FileServiceUri + getQRCodeImageResult.FileName;

                //生成短网址，然后用前端JS库生成二维码

                RequestApiResult<WeixinCreateShortUrlResult> getShortUrlResult = ShortUrlApiWrapper.GetShortUrl(DomainContext, url);
                if (getShortUrlResult.Success)
                {
                    url = getShortUrlResult.ApiResult.ShortUrl;
                }

                _memberManager.UpdateMemberCenterQRCodeImageUrl(MemberContext.Member.Id, url);
                MemberContext.Member.MemberCenterQRCodeImageUrl = url;
            }

            return RespondDataResult(url);
        }

        #region 管理功能

        public ActionResult GetMemberByCardNumber()
        {
            string cardNumber = Request.QueryString["cardNumber"];

            if (String.IsNullOrEmpty(cardNumber))
            {
                return RespondResult(false, "参数无效。");
            }

            MemberEntity member =
                _memberManager.GetMemberByCardNumber(DomainContext.Domain.Id, DomainContext.AppId, cardNumber);

            if (member == null)
            {
                return RespondResult(false, "会员不存在。");
            }

            MemberCardLevelEntity memberCard = null;
            if (member.CardLevel.HasValue)
            {
                memberCard = _memberManager.GetMemberCard(member.CardLevel.Value);
            }
            else
            {
                SettingsEntity settings = DomainContext.Settings;
                if (settings != null && settings.DefaultMemberCardLevel.HasValue)
                {
                    memberCard = _memberManager.GetMemberCard(settings.DefaultMemberCardLevel.Value);
                }
            }

            return RespondDataResult(
                new
                {
                    Member = member,
                    MemberCard = memberCard
                });
        }

        public ActionResult PointIncrement()
        {
            PointTrackArgs args = RequestArgs<PointTrackArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.Type = MemberPointTrackType.UserOperate;
            args.OperatorUser = MemberContext.User.Id;

            PointTrackResult pointTrackResult = _memberManager.PointTrack(args);

            #region 操作日志

            if (pointTrackResult.Success)
            {
                _operatedLogManager.Create(new OperatedLogEntity()
                {
                    Domain = DomainContext.Domain.Id,
                    AppId = DomainContext.AppId,
                    User = MemberContext.User.Id,
                    IP = Request.UserHostAddress,
                    Module = EnumModule.Member,
                    Description = "增加用户积分"
                });
            }

            #endregion

            return RespondDataResult(pointTrackResult);
        }

        public ActionResult PointDecrement()
        {
            PointTrackArgs args = RequestArgs<PointTrackArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.Type = MemberPointTrackType.UserOperate;
            args.OperatorUser = MemberContext.User.Id;

            PointTrackResult pointTrackResult = _memberManager.PointTrack(args);

            #region 操作日志

            if (pointTrackResult.Success)
            {
                _operatedLogManager.Create(new OperatedLogEntity()
                {
                    Domain = DomainContext.Domain.Id,
                    AppId = DomainContext.AppId,
                    User = MemberContext.User.Id,
                    IP = Request.UserHostAddress,
                    Module = EnumModule.Member,
                    Description = "消减用户积分"
                });
            }

            #endregion

            return RespondDataResult(pointTrackResult);
        }

        public ActionResult SetMemberLevel()
        {
            SetMemberLevelArgs args = RequestArgs<SetMemberLevelArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = MemberContext.Member.Domain;
            args.AppId = DomainContext.AppId;

            _memberManager.SetMemberLevel(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = MemberContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "设置会员的会员卡级别"
            });

            #endregion

            return RespondResult();
        }

        #endregion
    }
}