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
using Sheng.WeixinConstruction.Management.Shell.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Controllers
{
    public class CampaignController : BasalController
    {
        private static readonly CampaignManager _campaignManager = CampaignManager.Instance;

        public ActionResult Index()
        {
            return View();
        }

        #region PictureVote

        public ActionResult PictureVote_Preparatory()
        {
            PictureVote_PreparatoryViewModel model = new PictureVote_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.PictureVote);
            return View(model);
        }

        public ActionResult PictureVote_Ongoing()
        {
            PictureVote_PreparatoryViewModel model = new PictureVote_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.PictureVote);
            return View(model);
        }

        public ActionResult PictureVote_End()
        {
            PictureVote_PreparatoryViewModel model = new PictureVote_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.PictureVote);
            return View(model);
        }

        public ActionResult PictureVoteEidt()
        {
            return View();
        }

        public ActionResult PictureVoteDetail()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            PictureVoteDetailViewModel model = new PictureVoteDetailViewModel();
            model.CampaignBundle = _campaignManager.PictureVote.GetPictureVoteBundle(Guid.Parse(id));
            return View(model);
        }

        public ActionResult PictureVoteItemList()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            PictureVoteItemListViewModel model = new PictureVoteItemListViewModel();
            model.CampaignBundle = _campaignManager.PictureVote.GetPictureVoteBundle(Guid.Parse(id));
            return View(model);
        }

        public ActionResult PictureVoteItemDetail()
        {
            string campaignId = Request.QueryString["campaignId"];
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(campaignId) || String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            PictureVoteItemDetailViewModel model = new PictureVoteItemDetailViewModel();
            model.PictureVoteItem = _campaignManager.PictureVote.GetPictureVoteItem(Guid.Parse(id));
            return View(model);
        }

        #endregion

        #region MemberQRCode

        public ActionResult MemberQRCode_Preparatory()
        {
            MemberQRCode_PreparatoryViewModel model = new MemberQRCode_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.MemberQRCode);
            return View(model);
        }

        public ActionResult MemberQRCode_Ongoing()
        {
            MemberQRCode_PreparatoryViewModel model = new MemberQRCode_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.MemberQRCode);
            return View(model);
        }

        public ActionResult MemberQRCode_End()
        {
            MemberQRCode_PreparatoryViewModel model = new MemberQRCode_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.MemberQRCode);
            return View(model);
        }

        public ActionResult MemberQRCodeEidt()
        {
            return View();
        }

        public ActionResult MemberQRCodeDetail()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            MemberQRCodeDetailViewModel model = new MemberQRCodeDetailViewModel();
            model.CampaignBundle = _campaignManager.MemberQRCode.GetMemberQRCodeBundle(Guid.Parse(id));
            return View(model);
        }

        public ActionResult MemberQRCodeItemList()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            MemberQRCodeItemListViewModel model = new MemberQRCodeItemListViewModel();
            model.CampaignBundle = _campaignManager.MemberQRCode.GetMemberQRCodeBundle(Guid.Parse(id));
            return View(model);
        }

        #endregion

        #region Lottery

        public ActionResult Lottery_Preparatory()
        {
            Lottery_PreparatoryViewModel model = new Lottery_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.Lottery);
            return View(model);
        }

        public ActionResult Lottery_Ongoing()
        {
            Lottery_PreparatoryViewModel model = new Lottery_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.Lottery);
            return View(model);
        }

        public ActionResult Lottery_End()
        {
            Lottery_PreparatoryViewModel model = new Lottery_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.Lottery);
            return View(model);
        }

        public ActionResult LotteryEidt()
        {
            return View();
        }

        public ActionResult LotteryDetail()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            LotteryDetailViewModel model = new LotteryDetailViewModel();
            model.CampaignBundle = _campaignManager.Lottery.GetLotteryBundle(Guid.Parse(id));
            return View(model);
        }

        public ActionResult LotteryPeriodList()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            LotteryPeriodListViewModel model = new LotteryPeriodListViewModel();
            model.CampaignBundle = _campaignManager.Lottery.GetLotteryBundle(Guid.Parse(id));
            return View(model);
        }

        public ActionResult LotteryPeriodEdit()
        {
            return View();
        }

        public ActionResult LotteryPeriodSignLogList()
        {
            return View();
        }

        public ActionResult LotteryPeriodWinnerList()
        {
            return View();
        }

        #endregion

        #region LuckyTicket

        public ActionResult LuckyTicket_Preparatory()
        {
            LuckyTicket_PreparatoryViewModel model = new LuckyTicket_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.LuckyTicket);
            return View(model);
        }

        public ActionResult LuckyTicket_Ongoing()
        {
            LuckyTicket_PreparatoryViewModel model = new LuckyTicket_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.LuckyTicket);
            return View(model);
        }

        public ActionResult LuckyTicket_End()
        {
            LuckyTicket_PreparatoryViewModel model = new LuckyTicket_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.LuckyTicket);
            return View(model);
        }

        public ActionResult LuckyTicketEidt()
        {
            return View();
        }

        public ActionResult LuckyTicketDetail()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            LuckyTicketDetailViewModel model = new LuckyTicketDetailViewModel();
            model.CampaignBundle = _campaignManager.LuckyTicket.GetLuckyTicketBundle(Guid.Parse(id));
            return View(model);
        }

        public ActionResult LuckyTicketLogList()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            LuckyTicketLogListViewModel model = new LuckyTicketLogListViewModel();
            model.CampaignBundle = _campaignManager.LuckyTicket.GetLuckyTicketBundle(Guid.Parse(id));
            return View(model);
        }

        #endregion

        #region ShakingLottery

        public ActionResult ShakingLottery_Preparatory()
        {
            CampaignListViewModel model = new CampaignListViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
               DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.ShakingLottery);
            return View(model);
        }

        public ActionResult ShakingLottery_Ongoing()
        {
            CampaignListViewModel model = new CampaignListViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
               DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.ShakingLottery);
            return View(model);
        }

        public ActionResult ShakingLottery_End()
        {
            CampaignListViewModel model = new CampaignListViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
               DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.ShakingLottery);
            return View(model);
        }

        public ActionResult ShakingLotteryEidt()
        {
            return View();
        }

        public ActionResult ShakingLotteryDetail()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            ShakingLotteryDetailViewModel model = new ShakingLotteryDetailViewModel();
            model.CampaignBundle = _campaignManager.ShakingLottery.GetBundle(id);

            if (model.CampaignBundle.ShakingLottery.Mode == EnumCampaign_ShakingLotteryMode.Period)
            {
                model.PeriodList = _campaignManager.ShakingLottery.GetPeriodList(id);
                if (model.CampaignBundle.ShakingLottery.Period.HasValue)
                {
                    model.CurrentPeriod = _campaignManager.ShakingLottery.GetPeriod(model.CampaignBundle.ShakingLottery.Period.Value);
                }
            }

            return View(model);
        }

        public ActionResult ShakingLotteryPeriodList()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_ShakingLotteryBundle model = _campaignManager.ShakingLottery.GetBundle(Guid.Parse(id));
            return View(model);
        }

        public ActionResult ShakingLotteryPeriodEdit()
        {
            return View();
        }

        public ActionResult ShakingLotteryGiftList()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            ShakingLotteryGiftListViewModel model = new ShakingLotteryGiftListViewModel();
            model.CampaignBundle = _campaignManager.ShakingLottery.GetBundle(Guid.Parse(id));
            return View(model);
        }

        public ActionResult ShakingLotteryGiftEdit()
        {
            string strId = Request.QueryString["campaignId"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            ShakingLotteryGiftEditViewModel model = new ShakingLotteryGiftEditViewModel();
            model.CampaignBundle = _campaignManager.ShakingLottery.GetBundle(id);
            model.PeriodList = _campaignManager.ShakingLottery.GetPeriodList(id);
            return View(model);
        }

        public ActionResult ShakingLotteryGiftWinningList()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            Campaign_ShakingLotteryBundle bundle = _campaignManager.ShakingLottery.GetBundle(Guid.Parse(id));
            return View(bundle);
        }

        /// <summary>
        /// 用于投影的中奖结果显示
        /// </summary>
        /// <returns></returns>
        public ActionResult ShakingLotteryGiftWinningProjective()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            ShakingLotteryGiftWinningProjectiveViewModel model = new ShakingLotteryGiftWinningProjectiveViewModel();
            model.CampaignBundle = _campaignManager.ShakingLottery.GetBundle(id);

            if (model.CampaignBundle.ShakingLottery.Mode == EnumCampaign_ShakingLotteryMode.Period)
            {
                if (model.CampaignBundle.ShakingLottery.Period.HasValue)
                {
                    model.CurrentPeriod = _campaignManager.ShakingLottery.GetPeriod(model.CampaignBundle.ShakingLottery.Period.Value);
                }
            }

            return View(model);
        }

        #endregion

        #region Donation

        public ActionResult Donation_Preparatory()
        {
            LuckyTicket_PreparatoryViewModel model = new LuckyTicket_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.Donation);
            return View(model);
        }

        public ActionResult Donation_Ongoing()
        {
            LuckyTicket_PreparatoryViewModel model = new LuckyTicket_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.Donation);
            return View(model);
        }

        public ActionResult Donation_End()
        {
            LuckyTicket_PreparatoryViewModel model = new LuckyTicket_PreparatoryViewModel();
            model.CampaignCount = _campaignManager.GetCampaignCount(
                DomainContext.Domain.Id, DomainContext.AppId, EnumCampaignType.Donation);
            return View(model);
        }

        public ActionResult DonationEidt()
        {
            return View();
        }

        public ActionResult DonationDetail()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            DonationDetailViewModel model = new DonationDetailViewModel();
            model.CampaignBundle = _campaignManager.Donation.GetDonationBundle(Guid.Parse(id));
            return View(model);
        }

        public ActionResult DonationLogList()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            DonationDetailViewModel model = new DonationDetailViewModel();
            model.CampaignBundle = _campaignManager.Donation.GetDonationBundle(Guid.Parse(id));
            return View(model);
        }

        public ActionResult DonationProjective()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            DonationDetailViewModel model = new DonationDetailViewModel();
            model.CampaignBundle = _campaignManager.Donation.GetDonationBundle(Guid.Parse(id));
            return View(model);
        }

        #endregion
    }
}