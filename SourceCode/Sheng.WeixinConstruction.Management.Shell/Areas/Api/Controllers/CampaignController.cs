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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Areas.Api.Controllers
{
    public class CampaignController : BasalController
    {
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly CampaignManager _campaignManager = CampaignManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        public ActionResult GetCampaignList()
        {
            GetCampaignListArgs args = RequestArgs<GetCampaignListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _campaignManager.GetCampaignList(args);
            return RespondDataResult(result);
        }

        public ActionResult StartCampaign()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            EnumCampaignStartResult startResult = _campaignManager.StartCampaign(Guid.Parse(id));

            ApiResult result;

            switch (startResult)
            {
                case EnumCampaignStartResult.Successful:
                    result = new ApiResult(true);
                    break;
                case EnumCampaignStartResult.AlreadyStarted:
                    result = new ApiResult("此活动已经是开始过的活动。");
                    break;
                case EnumCampaignStartResult.AlreadyEnded:
                    result = new ApiResult("此活动是一个结束过的活动。");
                    break;
                //case EnumCampaignStartResult.FreeDomainLimited:
                //    result = new ApiResult("免费帐户同时进行的活动不可超过3个。");
                //    break;
                default:
                    result = new ApiResult("未知错误。");
                    break;
            }

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "开始活动 " + result.Message
            });

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult EndCampaign()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            EnumCampaignEndResult startResult = _campaignManager.EndCampaign(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "结束活动"
            });

            #endregion

            return RespondDataResult((int)startResult);
        }

        public ActionResult GetCampaign()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            CampaignEntity campaign = _campaignManager.GetCampaign(Guid.Parse(id));

            return RespondDataResult(campaign);
        }

        #region PictureVote

        public ActionResult GetCampaign_PictureVoteList()
        {
            GetCampaign_PictureVoteListArgs args = RequestArgs<GetCampaign_PictureVoteListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult result =
                _campaignManager.PictureVote.GetCampaign_PictureVoteList(UserContext.User.Domain, DomainContext.AppId, args);
            return RespondDataResult(result);
        }

        public ActionResult CreatePictureVote()
        {
            Campaign_PictureVoteBundle args = RequestArgs<Campaign_PictureVoteBundle>();
            if (args == null || args.Campaign == null || args.PictureVote == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Campaign.Domain = DomainContext.Domain.Id;
            args.Campaign.AppId = DomainContext.AppId;
            args.Campaign.Type = EnumCampaignType.PictureVote;
            args.Campaign.Status = EnumCampaignStatus.Preparatory;
            args.Campaign.StartTime = null;
            args.Campaign.EndTime = null;
            args.Campaign.AutoOngoing = false;
            args.Campaign.CreateTime = DateTime.Now;
            args.Campaign.CreateUser = UserContext.User.Id;

            args.PictureVote.MaxPublishTimes = 1;

            if (DomainContext.Domain.Type == EnumDomainType.Free)
            {
                args.PictureVote.VoteType = EnumCampaignPictureVoteVoteType.NoRepetition;
                args.PictureVote.AllowedNoAttentionVote = true;
            }

            _campaignManager.PictureVote.CreatePictureVote(args.Campaign, args.PictureVote);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "添加最美投票活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdatePictureVote()
        {
            Campaign_PictureVoteBundle args = RequestArgs<Campaign_PictureVoteBundle>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Campaign.Domain = DomainContext.Domain.Id;
            args.Campaign.AppId = DomainContext.AppId;
            args.Campaign.Type = EnumCampaignType.PictureVote;

            args.PictureVote.MaxPublishTimes = 1;

            //if (DomainContext.Domain.Type == EnumDomainType.Free)
            //{
            //    if (args.Campaign.MaxParticipant <= 0 || args.Campaign.MaxParticipant > 30)
            //    {
            //        args.Campaign.MaxParticipant = 30;
            //    }
            //    args.PictureVote.VoteType = EnumCampaignPictureVoteVoteType.NoRepetition;
            //}

            _campaignManager.PictureVote.UpdatePictureVote(args.Campaign, args.PictureVote);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "更新最美投票活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemovePictureVote()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.PictureVote.RemovePictureVote(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "删除最美投票活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetPictureVoteBundle()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_PictureVoteBundle bundle = _campaignManager.PictureVote.GetPictureVoteBundle(Guid.Parse(id));

            return RespondDataResult(bundle);
        }

        public ActionResult CreatePictureVoteItem()
        {
            Campaign_PictureVoteItemEntity args = RequestArgs<Campaign_PictureVoteItemEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            EnumCampaignCreatePictureVoteItemResult result = _campaignManager.PictureVote.CreatePictureVoteItem(args);
            return RespondDataResult((int)result);
        }

        public ActionResult GetPictureVoteItem()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_PictureVoteItemEntity item = _campaignManager.PictureVote.GetPictureVoteItem(Guid.Parse(id));

            return RespondDataResult(item);
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
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "删除最美投票活动中的项目"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetMemberPictureVoteItemList()
        {
            string id = Request.QueryString["campaignId"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            List<Campaign_PictureVoteItemEntity> list =
                _campaignManager.PictureVote.GetMemberPictureVoteItemList(DomainContext.Domain.Id, Guid.Parse(id));

            return RespondDataResult(list);
        }

        public ActionResult GetPictureVoteItemList()
        {
            GetCampaign_PictureVoteItemListArgs args = RequestArgs<GetCampaign_PictureVoteItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

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
                    User = UserContext.User.Id,
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
                    User = UserContext.User.Id,
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
                User = UserContext.User.Id,
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
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "解除锁定最美投票活动中的项目"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetPictureVoteItemMemberList()
        {
            GetCampaign_PictureVoteItemLogMemberListArgs args = RequestArgs<GetCampaign_PictureVoteItemLogMemberListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult result =
                _campaignManager.PictureVote.GetPictureVoteItemMemberList(args);
            return RespondDataResult(result);
        }

        public ActionResult GetPictureVoteDataAnalyse()
        {
            GetCampaign_PictureVoteDataAnalyseArgs args = RequestArgs<GetCampaign_PictureVoteDataAnalyseArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetPictureVoteDataAnalyseResult result =
                _campaignManager.PictureVote.GetPictureVoteDataAnalyse(args);
            return RespondDataResult(result);
        }

        public ActionResult OpenPictureVoteNewItem()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.PictureVote.OpenPictureVoteNewItem(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "打开最美投票活动的参与通道"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult ClosePictureVoteNewItem()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.PictureVote.ClosePictureVoteNewItem(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "关闭最美投票活动的参与通道"
            });

            #endregion

            return RespondResult();
        }

        #endregion

        #region MemberQRCode

        public ActionResult GetCampaign_MemberQRCodeList()
        {
            GetCampaign_MemberQRCodeListArgs args = RequestArgs<GetCampaign_MemberQRCodeListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult result =
                _campaignManager.MemberQRCode.GetCampaign_MemberQRCodeList(UserContext.User.Domain, DomainContext.AppId, args);
            return RespondDataResult(result);
        }

        public ActionResult CreateMemberQRCode()
        {
            Campaign_MemberQRCodeBundle args = RequestArgs<Campaign_MemberQRCodeBundle>();
            if (args == null || args.Empty)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Campaign.Domain = DomainContext.Domain.Id;
            args.Campaign.AppId = DomainContext.AppId;
            args.Campaign.Type = EnumCampaignType.MemberQRCode;
            args.Campaign.Status = EnumCampaignStatus.Preparatory;
            args.Campaign.StartTime = null;
            args.Campaign.EndTime = null;
            args.Campaign.AutoOngoing = false;
            args.Campaign.CreateTime = DateTime.Now;
            args.Campaign.CreateUser = UserContext.User.Id;

            _campaignManager.MemberQRCode.CreateMemberQRCode(args.Campaign, args.MemberQRCode);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "添加粉丝海报活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateMemberQRCode()
        {
            Campaign_MemberQRCodeBundle args = RequestArgs<Campaign_MemberQRCodeBundle>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Campaign.Domain = DomainContext.Domain.Id;
            args.Campaign.AppId = DomainContext.AppId;
            args.Campaign.Type = EnumCampaignType.MemberQRCode;

            _campaignManager.MemberQRCode.UpdateMemberQRCode(args.Campaign, args.MemberQRCode);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "更新粉丝海报活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveMemberQRCode()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.MemberQRCode.RemoveMemberQRCode(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "删除粉丝海报活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetMemberQRCodeBundle()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_MemberQRCodeBundle bundle = _campaignManager.MemberQRCode.GetMemberQRCodeBundle(Guid.Parse(id));

            return RespondDataResult(bundle);
        }

        public ActionResult GetMemberQRCodeItemList()
        {
            GetCampaign_MemberQRCodeItemListArgs args = RequestArgs<GetCampaign_MemberQRCodeItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult result = _campaignManager.MemberQRCode.GetMemberQRCodeItemList(args);
            return RespondDataResult(result);
        }

        public ActionResult GetMemberQRCodeDataAnalyse()
        {
            GetCampaign_MemberQRCodeDataAnalyseArgs args = RequestArgs<GetCampaign_MemberQRCodeDataAnalyseArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetMemberQRCodeDataAnalyseResult result =
                _campaignManager.MemberQRCode.GetMemberQRCodeDataAnalyse(args);
            return RespondDataResult(result);
        }


        #endregion

        #region Lottery

        public ActionResult GetCampaign_LotteryList()
        {
            GetCampaign_LotteryListArgs args = RequestArgs<GetCampaign_LotteryListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult result =
                _campaignManager.Lottery.GetCampaign_LotteryList(UserContext.User.Domain, DomainContext.AppId, args);
            return RespondDataResult(result);
        }

        public ActionResult CreateLottery()
        {
            Campaign_LotteryBundle args = RequestArgs<Campaign_LotteryBundle>();
            if (args == null || args.Empty)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Campaign.Domain = DomainContext.Domain.Id;
            args.Campaign.AppId = DomainContext.AppId;
            args.Campaign.Type = EnumCampaignType.Lottery;
            args.Campaign.Status = EnumCampaignStatus.Preparatory;
            args.Campaign.StartTime = null;
            args.Campaign.EndTime = null;
            args.Campaign.AutoOngoing = false;
            args.Campaign.CreateTime = DateTime.Now;
            args.Campaign.CreateUser = UserContext.User.Id;

            _campaignManager.Lottery.CreateLottery(args.Campaign, args.Lottery);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "添加定期抽奖活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateLottery()
        {
            Campaign_LotteryBundle args = RequestArgs<Campaign_LotteryBundle>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Campaign.Domain = DomainContext.Domain.Id;
            args.Campaign.AppId = DomainContext.AppId;
            args.Campaign.Type = EnumCampaignType.Lottery;

            _campaignManager.Lottery.UpdateLottery(args.Campaign, args.Lottery);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "更新定期抽奖活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveLottery()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.Lottery.RemoveLottery(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "删除定期抽奖活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetLotteryBundle()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_LotteryBundle bundle = _campaignManager.Lottery.GetLotteryBundle(Guid.Parse(id));

            return RespondDataResult(bundle);
        }

        public ActionResult CreateLotteryPeriod()
        {
            Campaign_LotteryPeriodEntity args = RequestArgs<Campaign_LotteryPeriodEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;

            _campaignManager.Lottery.CreateLotteryPeriod(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "添加定期抽奖活动周期"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateLotteryPeriod()
        {
            Campaign_LotteryPeriodEntity args = RequestArgs<Campaign_LotteryPeriodEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;

            NormalResult result = _campaignManager.Lottery.UpdateLotteryPeriod(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "更新定期抽奖活动周期"
            });

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult RemoveLotteryPeriod()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.Lottery.RemoveLotteryPeriod(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "删除定期抽奖活动周期"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetLotteryPeriod()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_LotteryPeriodEntity period = _campaignManager.Lottery.GetLotteryPeriod(Guid.Parse(id));

            return RespondDataResult(period);
        }

        public ActionResult GetLotteryPeriodList()
        {
            GetLotteryPeriodListArgs args = RequestArgs<GetLotteryPeriodListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult result = _campaignManager.Lottery.GetLotteryPeriodList(args);
            return RespondDataResult(result);
        }

        public ActionResult GetLotteryPeriodSignLogList()
        {
            GetLotteryPeriodLogSignListArgs args = RequestArgs<GetLotteryPeriodLogSignListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult result = _campaignManager.Lottery.GetLotteryPeriodSignLogList(args);
            return RespondDataResult(result);
        }

        #endregion

        #region LuckyTicket

        public ActionResult GetCampaign_LuckyTicketList()
        {
            GetCampaign_LuckyTicketListArgs args = RequestArgs<GetCampaign_LuckyTicketListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _campaignManager.LuckyTicket.GetCampaign_LuckyTicketList(args);
            return RespondDataResult(result);
        }

        public ActionResult CreateLuckyTicket()
        {
            Campaign_LuckyTicketBundle args = RequestArgs<Campaign_LuckyTicketBundle>();
            if (args == null || args.Empty)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Campaign.Domain = DomainContext.Domain.Id;
            args.Campaign.AppId = DomainContext.AppId;
            args.Campaign.Type = EnumCampaignType.LuckyTicket;
            args.Campaign.Status = EnumCampaignStatus.Preparatory;
            args.Campaign.StartTime = null;
            args.Campaign.EndTime = null;
            args.Campaign.AutoOngoing = false;
            args.Campaign.CreateTime = DateTime.Now;
            args.Campaign.CreateUser = UserContext.User.Id;

            _campaignManager.LuckyTicket.CreateLuckyTicket(args.Campaign, args.LuckyTicket);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "添加聚人气抽奖活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateLuckyTicket()
        {
            Campaign_LuckyTicketBundle args = RequestArgs<Campaign_LuckyTicketBundle>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Campaign.Domain = DomainContext.Domain.Id;
            args.Campaign.AppId = DomainContext.AppId;
            args.Campaign.Type = EnumCampaignType.LuckyTicket;

            _campaignManager.LuckyTicket.UpdateLuckyTicket(args.Campaign, args.LuckyTicket);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "更新聚人气抽奖活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveLuckyTicket()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.LuckyTicket.RemoveLuckyTicket(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "删除聚人气抽奖活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetLuckyTicketBundle()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_LuckyTicketBundle bundle = _campaignManager.LuckyTicket.GetLuckyTicketBundle(Guid.Parse(id));

            return RespondDataResult(bundle);
        }

        public ActionResult GetLuckyTicketLogList()
        {
            GetCampaign_LuckyTicketLogListArgs args = RequestArgs<GetCampaign_LuckyTicketLogListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _campaignManager.LuckyTicket.GetLuckyTicketLogList(args);
            return RespondDataResult(result);
        }


        #endregion

        #region ShakingLottery

        public ActionResult GetCampaign_ShakingLotteryList()
        {
            GetCampaignListArgs args = RequestArgs<GetCampaignListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _campaignManager.ShakingLottery.GetList(args);
            return RespondDataResult(result);
        }

        public ActionResult CreateShakingLottery()
        {
            Campaign_ShakingLotteryBundle args = RequestArgs<Campaign_ShakingLotteryBundle>();
            if (args == null || args.Empty)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Campaign.Domain = DomainContext.Domain.Id;
            args.Campaign.AppId = DomainContext.AppId;
            args.Campaign.Type = EnumCampaignType.ShakingLottery;
            args.Campaign.Status = EnumCampaignStatus.Preparatory;
            args.Campaign.StartTime = null;
            args.Campaign.EndTime = null;
            args.Campaign.AutoOngoing = false;
            args.Campaign.CreateTime = DateTime.Now;
            args.Campaign.CreateUser = UserContext.User.Id;

            _campaignManager.ShakingLottery.Create(args.Campaign, args.ShakingLottery);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "添加摇一摇抽奖活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateShakingLottery()
        {
            Campaign_ShakingLotteryBundle args = RequestArgs<Campaign_ShakingLotteryBundle>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Campaign.Domain = DomainContext.Domain.Id;
            args.Campaign.AppId = DomainContext.AppId;
            args.Campaign.Type = EnumCampaignType.ShakingLottery;

            _campaignManager.ShakingLottery.Update(args.Campaign, args.ShakingLottery);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "更新摇一摇抽奖活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveShakingLottery()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.ShakingLottery.Remove(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "删除摇一摇抽奖活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetShakingLotteryBundle()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_ShakingLotteryBundle bundle = _campaignManager.ShakingLottery.GetBundle(Guid.Parse(id));

            return RespondDataResult(bundle);
        }

        #region Gift

        public ActionResult CreateShakingLotteryGift()
        {
            Campaign_ShakingLotteryGiftEntity args = RequestArgs<Campaign_ShakingLotteryGiftEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;

            _campaignManager.ShakingLottery.CreateGift(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "添加摇一摇抽奖活动奖品"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateShakingLotteryGift()
        {
            Campaign_ShakingLotteryGiftEntity args = RequestArgs<Campaign_ShakingLotteryGiftEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;

            _campaignManager.ShakingLottery.UpdateGift(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "更新摇一摇抽奖活动奖品"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveShakingLotteryGift()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.ShakingLottery.RemoveGift(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "删除摇一摇抽奖活动奖品"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetShakingLotteryGift()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_ShakingLotteryGiftEntity gift = _campaignManager.ShakingLottery.GetGift(Guid.Parse(id));

            return RespondDataResult(gift);
        }

        public ActionResult GetShakingLotteryGiftList()
        {
            string strId = Request.QueryString["campaignId"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            DataTable list = _campaignManager.ShakingLottery.GetGiftDataTable(id);
            return RespondDataResult(list);
        }

        #endregion

        #region Period

        public ActionResult CreateShakingLotteryPeriod()
        {
            Campaign_ShakingLotteryPeriodEntity args = RequestArgs<Campaign_ShakingLotteryPeriodEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;

            _campaignManager.ShakingLottery.CreatePeriod(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "添加摇一摇抽奖活动周期"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateShakingLotteryPeriod()
        {
            Campaign_ShakingLotteryPeriodEntity args = RequestArgs<Campaign_ShakingLotteryPeriodEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;

            _campaignManager.ShakingLottery.UpdatePeriod(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "更新摇一摇抽奖活动周期"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveShakingLotteryPeriod()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.ShakingLottery.RemovePeriod(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "删除摇一摇抽奖活动周期"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetShakingLotteryPeriod()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_ShakingLotteryPeriodEntity period = _campaignManager.ShakingLottery.GetPeriod(Guid.Parse(id));

            return RespondDataResult(period);
        }

        public ActionResult GetShakingLotteryPeriodList()
        {
            string strId = Request.QueryString["campaignId"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            List<Campaign_ShakingLotteryPeriodEntity> list = _campaignManager.ShakingLottery.GetPeriodList(id);
            return RespondDataResult(list);
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

        public ActionResult GetShakingLotteryGiftWinningList()
        {
            GetCampaign_ShakingLotteryGiftWinningListArgs args = RequestArgs<GetCampaign_ShakingLotteryGiftWinningListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _campaignManager.ShakingLottery.GetGiftWinningList(args);
            return RespondDataResult(result);
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
                User = UserContext.User.Id,
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
                User = UserContext.User.Id,
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

            _campaignManager.ShakingLottery.ClearWinning(Guid.Parse(id),DomainContext.Domain.Id);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "清除摇一摇活动中奖结果"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetShakingLotteryGiftProjectiveData()
        {
            GetCampaign_ShakingLotteryGiftWinningListArgs args = RequestArgs<GetCampaign_ShakingLotteryGiftWinningListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;

            ShakingLotteryGiftProjectiveData result = _campaignManager.ShakingLottery.GetProjectiveData(args);
            return RespondDataResult(result);
        }

        #endregion

        #region Donation

        public ActionResult GetCampaign_DonationList()
        {
            GetCampaign_DonationListArgs args = RequestArgs<GetCampaign_DonationListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _campaignManager.Donation.GetCampaign_DonationList(args);
            return RespondDataResult(result);
        }

        public ActionResult CreateDonation()
        {
            Campaign_DonationBundle args = RequestArgs<Campaign_DonationBundle>();
            if (args == null || args.Empty)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Campaign.Domain = DomainContext.Domain.Id;
            args.Campaign.AppId = DomainContext.AppId;
            args.Campaign.Type = EnumCampaignType.Donation;
            args.Campaign.Status = EnumCampaignStatus.Preparatory;
            args.Campaign.StartTime = null;
            args.Campaign.EndTime = null;
            args.Campaign.AutoOngoing = false;
            args.Campaign.CreateTime = DateTime.Now;
            args.Campaign.CreateUser = UserContext.User.Id;

            _campaignManager.Donation.CreateDonation(args.Campaign, args.Donation);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "添加聚人气抽奖活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateDonation()
        {
            Campaign_DonationBundle args = RequestArgs<Campaign_DonationBundle>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Campaign.Domain = DomainContext.Domain.Id;
            args.Campaign.AppId = DomainContext.AppId;
            args.Campaign.Type = EnumCampaignType.Donation;

            _campaignManager.Donation.UpdateDonation(args.Campaign, args.Donation);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "更新聚人气抽奖活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveDonation()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _campaignManager.Donation.RemoveDonation(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "删除聚人气抽奖活动"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetDonationBundle()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_DonationBundle bundle = _campaignManager.Donation.GetDonationBundle(Guid.Parse(id));

            return RespondDataResult(bundle);
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
            args.Finished = true;

            GetItemListResult result = _campaignManager.Donation.GetDonationLogList(args);
            return RespondDataResult(result);
        }


        #endregion
    }
}