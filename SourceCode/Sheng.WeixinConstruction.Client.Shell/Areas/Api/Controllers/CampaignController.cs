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
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Client.Shell.Models;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using Sheng.WeixinConstruction.WeixinContract.PayApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Areas.Api.Controllers
{
    public class CampaignController : ApiBasalController
    {
        private static readonly CampaignManager _campaignManager = CampaignManager.Instance;
        private static readonly FileService _fileService = FileService.Instance;
        private static readonly LogService _log = LogService.Instance;

        public ActionResult GetCampaignList()
        {
            GetCampaignListArgs args = RequestArgs<GetCampaignListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            //args.Status = EnumCampaignStatus.Ongoing;
            args.OrderBy = "StartTime";
            args.Sort = EnumSort.DESC;

            GetItemListResult result = _campaignManager.GetCampaignList(args);
            return RespondDataResult(result);
        }

        [AllowedOnlyOpenId]
        public ActionResult GetCampaignDescription()
        {
            string id = Request.QueryString["id"];

            return RespondDataResult(_campaignManager.GetCampaignDescription(Guid.Parse(id)));
        }

        [AllowedOnlyOpenId]
        public ActionResult ShareTimeline()
        {
            string strCampaignId = Request.QueryString["campaignId"];
            Guid campaignId = Guid.Empty;
            if (String.IsNullOrEmpty(strCampaignId) || Guid.TryParse(strCampaignId, out campaignId) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            ShareResult result;
            if (MemberContext == null)
            {
                result = _campaignManager.ShareTimeline(campaignId, null, this.OpenId);
            }
            else
            {
                result = _campaignManager.ShareTimeline(campaignId, MemberContext.Member.Id, this.OpenId);
            }

            return RespondDataResult(result);
        }

        [AllowedOnlyOpenId]
        public ActionResult ShareAppMessage()
        {
            string strCampaignId = Request.QueryString["campaignId"];
            Guid campaignId = Guid.Empty;
            if (String.IsNullOrEmpty(strCampaignId) || Guid.TryParse(strCampaignId, out campaignId) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            ShareResult result;
            if (MemberContext == null)
            {
                result = _campaignManager.ShareAppMessage(campaignId, null, this.OpenId);
            }
            else
            {
                result = _campaignManager.ShareAppMessage(campaignId, MemberContext.Member.Id, this.OpenId);
            }

            return RespondDataResult(result);
        }

        #region PictureVote

        /// <summary>
        /// 投票
        /// </summary>
        /// <returns></returns>
        [AllowedOnlyOpenId]
        public ActionResult PictureVote()
        {
            string campaignId = Request.QueryString["campaignId"];
            string itemId = Request.QueryString["itemId"];

            if (String.IsNullOrEmpty(campaignId) || String.IsNullOrEmpty(itemId))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_PictureVoteArgs args = new Campaign_PictureVoteArgs();
            args.DomainId = DomainContext.Domain.Id;
            args.CampaignId = Guid.Parse(campaignId);
            args.ItemId = Guid.Parse(itemId);
            args.OpenId = this.OpenId;
            //在允许未关注者参与的情况下，无会员信息
            if (MemberContext != null && MemberContext.Member != null)
            {
                args.Member = MemberContext.Member.Id;
            }

            EnumCampaignPictureVoteResult result = _campaignManager.PictureVote.PictureVote(args);

            return RespondDataResult((int)result);
        }

        [AllowedOnlyOpenId]
        public ActionResult GetPictureVoteItemList()
        {
            GetCampaign_PictureVoteItemListArgs args = RequestArgs<GetCampaign_PictureVoteItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            string campaignId = Request.QueryString["campaignId"];

            args.CampaignId = Guid.Parse(campaignId);
            args.ApproveStatus = EnumCampaignPictureVoteItemApproveStatus.Approved;

            GetItemListResult result = _campaignManager.PictureVote.GetPictureVoteItemList(args);
            return RespondDataResult(result);
        }

        /// <summary>
        /// 提交投票项目
        /// </summary>
        /// <returns></returns>
        public ActionResult CreatePictureVoteItem()
        {
            CreatePictureVoteItemArgs args = RequestArgs<CreatePictureVoteItemArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            if (String.IsNullOrEmpty(args.ImageServerId))
            {
                return RespondResult(false, "参数无效。");
            }

            //判断参与人数是否达到限制
            if (_campaignManager.PictureVote.PictureVoteIsFullParticipant(args.CampaignId))
            {
                return RespondResult(false, "该活动已达最大允许参与人数。");
            }

            //调用文件服务器进行代理下载，把上传到微信后台的文件下载下来
            FileDownloadAgentWithMediaIdArgs downloadArgs = new FileDownloadAgentWithMediaIdArgs();
            downloadArgs.Domain = DomainContext.Domain.Id;
            downloadArgs.MediaId = args.ImageServerId;
            FileDownloadAgentWithMediaIdResult downloadResult = _fileService.DownloadAgentWithMediaId(downloadArgs);
            string imageUrl;
            if (downloadResult.Success)
            {
                _log.Write("下载媒体文件返回：", JsonConvert.SerializeObject(downloadResult), TraceEventType.Verbose);
                imageUrl = _fileService.FileServiceUri + downloadResult.OutputFile;
            }
            else
            {
                return RespondResult(false, "照片上传失败：" + downloadResult.Message);
            }

            Campaign_PictureVoteItemEntity item = new Campaign_PictureVoteItemEntity();
            item.Domain = DomainContext.Domain.Id;
            item.CampaignId = args.CampaignId;
            item.Member = MemberContext.Member.Id;
            item.Title = args.Title;
            item.Description = args.Description;
            item.Url = imageUrl;
            item.UploadTime = DateTime.Now;

            EnumCampaignCreatePictureVoteItemResult result = _campaignManager.PictureVote.CreatePictureVoteItem(item);
            return RespondDataResult(result);
        }

        /// <summary>
        /// 获取指定投票项目的详情
        /// </summary>
        /// <returns></returns>
        [AllowedOnlyOpenId]
        public ActionResult GetPictureVoteItem()
        {
            string itemId = Request.QueryString["itemId"];

            if (String.IsNullOrEmpty(itemId))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_PictureVoteItemEntity item = _campaignManager.PictureVote.GetPictureVoteItem(Guid.Parse(itemId));

            return RespondDataResult(item);
        }

        [AllowedOnlyOpenId]
        public ActionResult GetPictureVoteItemIdBySerialNumber()
        {
            string strSerialNumber = Request.QueryString["serialNumber"];

            if (String.IsNullOrEmpty(strSerialNumber))
            {
                return RespondResult(false, "参数无效。");
            }

            Guid? id = _campaignManager.PictureVote.GetPictureVoteItemIdBySerialNumber(strSerialNumber);

            if (id.HasValue == false)
            {
                return RespondResult(false, "指定的编号不存在。");
            }
            else
            {
                return RespondDataResult(id);
            }
        }

        public ActionResult RemovePictureVoteItemByMember()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.PictureVote.RemovePictureVoteItem(Guid.Parse(id), MemberContext.Member.Id);

            return RespondResult();
        }

        /// <summary>
        /// 分享最美投票到朋友圈
        /// </summary>
        /// <returns></returns>
        [AllowedOnlyOpenId]
        public ActionResult PictureVoteShareTimeline()
        {
            string strCampaignId = Request.QueryString["campaignId"];
            Guid campaignId = Guid.Empty;
            if (String.IsNullOrEmpty(strCampaignId) || Guid.TryParse(strCampaignId, out campaignId) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_PictureVoteShareResult result;
            if (MemberContext == null)
            {
                result = _campaignManager.PictureVote.PictureVoteShareTimeline(campaignId, null, this.OpenId);
            }
            else
            {
                result = _campaignManager.PictureVote.PictureVoteShareTimeline(campaignId, MemberContext.Member.Id, this.OpenId);
            }

            return RespondDataResult(result);
        }

        /// <summary>
        /// 分享最美投票给好友
        /// </summary>
        /// <returns></returns>
        [AllowedOnlyOpenId]
        public ActionResult PictureVoteShareAppMessage()
        {
            string strCampaignId = Request.QueryString["campaignId"];
            Guid campaignId = Guid.Empty;
            if (String.IsNullOrEmpty(strCampaignId) || Guid.TryParse(strCampaignId, out campaignId) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_PictureVoteShareResult result;
            if (MemberContext == null)
            {
                result = _campaignManager.PictureVote.PictureVoteShareAppMessage(campaignId, null, this.OpenId);
            }
            else
            {
                result = _campaignManager.PictureVote.PictureVoteShareAppMessage(campaignId, MemberContext.Member.Id, this.OpenId);
            }

            return RespondDataResult(result);
        }

        #region 管理功能

        /// <summary>
        /// 用于手机端管理后台
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPictureVoteAllItemList()
        {
            GetCampaign_PictureVoteItemListArgs args = RequestArgs<GetCampaign_PictureVoteItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            string campaignId = Request.QueryString["campaignId"];

            args.CampaignId = Guid.Parse(campaignId);
            //args.ApproveStatus = EnumCampaignPictureVoteItemApproveStatus.Approved;

            GetItemListResult result = _campaignManager.PictureVote.GetPictureVoteItemList(args);
            return RespondDataResult(result);
        }

        public ActionResult PictureVoteItemApprove()
        {
            Campaign_PictureVoteItemApproveArgs args = RequestArgs<Campaign_PictureVoteItemApproveArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            NormalResult result = _campaignManager.PictureVote.PictureVoteItemApprove(this.DomainContext, args);

            #region 操作日志

            if (result.Success)
            {
                _operatedLogManager.Create(new OperatedLogEntity()
                {
                    Domain = DomainContext.Domain.Id,
                    AppId = DomainContext.AppId,
                    User = MemberContext.User.Id,
                    IP = Request.UserHostAddress,
                    Module = EnumModule.Campaign,
                    Description = "审核通过最美投票活动中的项目"
                });
            }

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult PictureVoteItemRejected()
        {
            Campaign_PictureVoteItemRejectedArgs args = RequestArgs<Campaign_PictureVoteItemRejectedArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            NormalResult result = _campaignManager.PictureVote.PictureVoteItemRejected(this.DomainContext, args);

            #region 操作日志

            if (result.Success)
            {
                _operatedLogManager.Create(new OperatedLogEntity()
                {
                    Domain = DomainContext.Domain.Id,
                    AppId = DomainContext.AppId,
                    User = MemberContext.User.Id,
                    IP = Request.UserHostAddress,
                    Module = EnumModule.Campaign,
                    Description = "拒绝最美投票活动中的项目，说明：" + args.Message
                });
            }

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult PictureVoteItemLock()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.PictureVote.PictureVoteItemLock(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = MemberContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "锁定最美投票活动中的项目"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult PictureVoteItemUnLock()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.PictureVote.PictureVoteItemUnLock(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = MemberContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "解除锁定最美投票活动中的项目"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemovePictureVoteItem()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.PictureVote.RemovePictureVoteItem(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = MemberContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "删除最美投票活动中的项目"
            });

            #endregion

            return RespondResult();
        }

        #endregion

        #endregion

        #region MemberQRCode

        public ActionResult GetMemberQRCodeItem()
        {
            string campaignId = Request.QueryString["campaignId"];

            if (String.IsNullOrEmpty(campaignId))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_MemberQRCodeItemEntity result =
                _campaignManager.MemberQRCode.GetMemberQRCodeItem(Guid.Parse(campaignId), MemberContext.Member.Id);

            if (result == null)
            {
                //生成一个
                NormalResult<Campaign_MemberQRCodeItemEntity> normalResult = _campaignManager.MemberQRCode.CreateMemberQRCodeItem(
                    DomainContext, Guid.Parse(campaignId), MemberContext.Member.Id);

                if (normalResult.Success == false)
                {
                    return RespondResult(false, normalResult.Message);
                }
                result = normalResult.Data;
            }

            if (result == null)
            {
                return RespondResult(false, "生成二维码失败。");
            }

            return RespondDataResult(result);
        }

        #endregion

        #region Lottery

        /// <summary>
        /// 获取正在进行中的抽奖周期
        /// </summary>
        /// <returns></returns>
        [AllowedAnonymous]
        public ActionResult GetLotteryOngoingPeriodList()
        {
            GetLotteryPeriodListArgs args = RequestArgs<GetLotteryPeriodListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.EndTime = DateTime.Now;

            GetItemListResult result = _campaignManager.Lottery.GetLotteryOngoingPeriodList(args);
            return RespondDataResult(result);
        }

        /// <summary>
        /// 获取正在进行中的抽奖周期
        /// </summary>
        /// <returns></returns>
        [AllowedAnonymous]
        public ActionResult GetLotteryEndedPeriodList()
        {
            GetLotteryPeriodListArgs args = RequestArgs<GetLotteryPeriodListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.EndTime = DateTime.Now;

            GetItemListResult result = _campaignManager.Lottery.GetLotteryEndedPeriodList(args);
            return RespondDataResult(result);
        }

        [AllowedOnlyOpenId]
        public ActionResult GetLotteryPeriod()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_LotteryPeriodEntity period = _campaignManager.Lottery.GetLotteryPeriod(Guid.Parse(id));

            Campaign_LotterySignLogEntity log = null;

            if (MemberContext != null)
            {
                log = _campaignManager.Lottery.GetLotteryPeriodLog(
                    period.CampaignId, period.Id, MemberContext.Member.Id);
            }

            return RespondDataResult(new
            {
                Period = period,
                Log = log
            });
        }

        public ActionResult LotterySign()
        {
            string campaignId = Request.QueryString["campaignId"];
            string periodId = Request.QueryString["periodId"];

            if (String.IsNullOrEmpty(campaignId) || String.IsNullOrEmpty(periodId))
            {
                return RespondResult(false, "参数无效。");
            }

            LotterySignArgs args = new LotterySignArgs();
            args.CampaignId = Guid.Parse(campaignId);
            args.DomainId = DomainContext.Domain.Id;
            args.PeriodId = Guid.Parse(periodId);
            args.MemberId = MemberContext.Member.Id;

            LotterySignResult result = _campaignManager.Lottery.LotterySign(args);

            return RespondDataResult(result);
        }

        /// <summary>
        /// 获取指定周期的中奖者名单
        /// </summary>
        /// <returns></returns>
        [AllowedOnlyOpenId]
        public ActionResult GetLotteryWinnerList()
        {
            string strPeriodId = Request.QueryString["periodId"];

            if (String.IsNullOrEmpty(strPeriodId))
            {
                return RespondResult(false, "参数无效。");
            }

            Guid periodId = Guid.Parse(strPeriodId);


            Campaign_LotteryPeriodEntity period = _campaignManager.Lottery.GetLotteryPeriod(periodId);
            GetLotteryWinnerListResult result = _campaignManager.Lottery.GetLotteryWinnerList(periodId);
            bool isWinner = false;
            if (MemberContext != null)
            {
                isWinner = _campaignManager.Lottery.IsLotteryWinner(periodId, MemberContext.Member.Id);
            }

            return RespondDataResult(new
            {
                Period = period,
                WinnerList = result.ItemList,
                IsWinner = isWinner
            });
        }

        #endregion

        #region LuckyTicket

        public ActionResult GetLuckyTicketLogList()
        {
            GetCampaign_LuckyTicketLogListByMemberArgs args = RequestArgs<GetCampaign_LuckyTicketLogListByMemberArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.MemberId = MemberContext.Member.Id;
            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _campaignManager.LuckyTicket.GetMemberLuckyTicketLogList(args);

            //为自己生成一个
            if (result.Page == 1 && result.TotalCount == 0)
            {
                Campaign_LuckyTicketLogEntity createLuckyTicketLogArgs = new Campaign_LuckyTicketLogEntity();
                createLuckyTicketLogArgs.CampaignId = args.CampaignId;
                createLuckyTicketLogArgs.Domain = DomainContext.Domain.Id;
                createLuckyTicketLogArgs.Member = MemberContext.Member.Id;
                createLuckyTicketLogArgs.FromOpenId = MemberContext.Member.OpenId;
                _campaignManager.LuckyTicket.CreateLuckyTicketLog(createLuckyTicketLogArgs);
                result = _campaignManager.LuckyTicket.GetMemberLuckyTicketLogList(args);
            }

            return RespondDataResult(result);

        }

        public ActionResult GetLuckyTicketWinLogList()
        {
            GetCampaign_LuckyTicketWinLogListArgs args = RequestArgs<GetCampaign_LuckyTicketWinLogListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _campaignManager.LuckyTicket.GetLuckyTicketWinLogList(args);
            return RespondDataResult(result);
        }

        #region 管理功能

        public ActionResult LuckyTicketDraw()
        {
            LuckyTicketDrawArgs args = RequestArgs<LuckyTicketDrawArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;

            _campaignManager.LuckyTicket.LuckyTicketDraw(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = MemberContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "号码抽奖开奖"
            });

            #endregion

            return RespondResult();
        }

        #endregion

        #endregion

        #region ShakingLottery

        public ActionResult ShakeShakingLotteryGift()
        {
            string strCampaignId = Request.QueryString["campaignId"];
            Guid campaignId = Guid.Empty;
            if (String.IsNullOrEmpty(strCampaignId) || Guid.TryParse(strCampaignId, out campaignId) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            string strPeriodId = Request.QueryString["periodId"];
            Guid? periodId = null;
            if (String.IsNullOrEmpty(strPeriodId) == false)
            {
                periodId = Guid.Parse(strPeriodId);
            }

            NormalResult<Campaign_ShakingLotteryGiftEntity> shakingResult =
                _campaignManager.ShakingLottery.Shake(campaignId, periodId,
                MemberContext.Member.Id, DomainContext);

            if (shakingResult.Success == false)
            {
                return RespondResult(false, shakingResult.Message);
            }
            else
            {
                return RespondDataResult(shakingResult.Data);
            }
        }

        public ActionResult StartShaking()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.ShakingLottery.StartShaking(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = MemberContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "开启摇一摇活动参与通道"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult EndShaking()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.ShakingLottery.EndShaking(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = MemberContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "关闭摇一摇活动参与通道"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult ClearShakingLotteryGiftWinningList()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.ShakingLottery.ClearWinning(Guid.Parse(id), DomainContext.Domain.Id);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = MemberContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "清除摇一摇活动中奖结果"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult SetShakingLotteryPeriod()
        {
            string strCampaignId = Request.QueryString["campaignId"];
            Guid campaignId = Guid.Empty;
            if (String.IsNullOrEmpty(strCampaignId) || Guid.TryParse(strCampaignId, out campaignId) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            string strPeriodId = Request.QueryString["periodId"];
            Guid? periodId = null;
            if (String.IsNullOrEmpty(strPeriodId) == false)
            {
                periodId = Guid.Parse(strPeriodId);
            }

            _campaignManager.ShakingLottery.SetPeriod(campaignId, periodId);
            return RespondResult();
        }


        #endregion

        #region Donation

        public ActionResult DonationPay()
        {
            DonationPayArgs args = RequestArgs<DonationPayArgs>();
            if (args == null || args.Fee <= 0)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Fee = args.Fee * 100;

            args.DomainId = DomainContext.Domain.Id;
            args.MemberId = MemberContext.Member.Id;
            args.OpenId = MemberContext.Member.OpenId;
            args.SpbillCreateIp = Request.UserHostAddress;

            NormalResult<CreatePayOrderResult> depositResult =
                _campaignManager.Donation.DonationPay(args, DomainContext.AuthorizerPay);
            if (depositResult.Success)
            {
                return RespondDataResult(depositResult);
            }
            else
            {
                return RespondResult(false, depositResult.Message);
            }
        }

        public ActionResult GetDonationLogList()
        {
            GetCampaign_DonationLogListArgs args = RequestArgs<GetCampaign_DonationLogListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.Member = MemberContext.Member.Id;
            args.Finished = false;

            GetItemListResult result = _campaignManager.Donation.GetDonationLogList(args);
            return RespondDataResult(result);
        }


        #endregion
    }
}