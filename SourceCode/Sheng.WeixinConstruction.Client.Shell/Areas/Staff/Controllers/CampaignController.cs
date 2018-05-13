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


using Sheng.WeixinConstruction.Client.Shell.Areas.Staff.Models;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Areas.Staff.Controllers
{
    public class CampaignController : ClientBasalController
    {
        private static readonly CampaignManager _campaignManager = CampaignManager.Instance;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //base中已经处理了
            if (filterContext.Result != null)
                return;

            //获取关联的User对象
            MemberContext.User = _userManager.GetUserByMemberId(
                DomainContext.Domain.Id, MemberContext.Member.Id);

            if (MemberContext == null || MemberContext.User == null)
            {
                filterContext.Result = new RedirectResult("http://wxcm.shengxunwei.com/M/Home/Portal");
                return;
            }
        }

        public ActionResult CampaignList()
        {
            CampaignListViewModel model = new CampaignListViewModel();
            return View(model);
        }

        public ActionResult PictureVote()
        {
            string strCampaignId = Request.QueryString["campaignId"];
            Guid campaignId = Guid.Parse(strCampaignId);

            PictureVoteViewModel model = new PictureVoteViewModel();
            model.CampaignBundle = _campaignManager.PictureVote.GetPictureVoteBundle(campaignId);
            return View(model);
        }

        public ActionResult PictureVoteItemList()
        {
            string strCampaignId = Request.QueryString["campaignId"];
            Guid campaignId = Guid.Parse(strCampaignId);

            PictureVoteItemListViewModel model = new PictureVoteItemListViewModel();
            model.Campaign = _campaignManager.GetCampaign(campaignId);
            return View(model);
        }

        public ActionResult PictureVoteItemDetail()
        {
            string strItemId = Request.QueryString["itemId"];
            string strCampaignId = Request.QueryString["campaignId"];
            Guid itemId = Guid.Parse(strItemId);
            Guid campaignId = Guid.Parse(strCampaignId);

            Campaign_PictureVoteItemEntity pictureVoteItem = _campaignManager.PictureVote.GetPictureVoteItem(itemId);
            if (pictureVoteItem == null)
            {
                return RedirectToAction("PictureVoteItemList",
                    new { domainId = DomainContext.Domain.Id, campaignId = strCampaignId });
            }

            PictureVoteItemDetailViewModel model = new PictureVoteItemDetailViewModel();
            model.PictureVoteItem = pictureVoteItem;
            return View(model);
        }

        public ActionResult LuckyTicket()
        {
            string strCampaignId = Request.QueryString["campaignId"];
            Guid campaignId = Guid.Parse(strCampaignId);

            LuckyTicketViewModel model = new LuckyTicketViewModel();
            model.CampaignBundle = _campaignManager.LuckyTicket.GetLuckyTicketBundle(campaignId);
            return View(model);
        }

        public ActionResult LuckyTicketDraw()
        {
            string strCampaignId = Request.QueryString["campaignId"];
            Guid campaignId = Guid.Parse(strCampaignId);

            LuckyTicketDrawViewModel model = new LuckyTicketDrawViewModel();
            model.Campaign = _campaignManager.GetCampaign(campaignId);
            return View(model);
        }

        public ActionResult MemberQRCode()
        {
            return View();
        }

        public ActionResult Lottery()
        {
            return View();
        }

        public ActionResult ShakingLottery()
        {

            string strId = Request.QueryString["campaignId"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            ShakingLotteryViewModel model = new ShakingLotteryViewModel();
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
	}
}