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


using Sheng.WeixinConstruction.Client.Shell.Models;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Controllers
{
    public class CampaignController : ClientBasalController
    {
        private static readonly CampaignManager _campaignManager = CampaignManager.Instance;
        private static readonly OneDollarBuyingManager _oneDollarBuyingManager = OneDollarBuyingManager.Instance;

        public ActionResult CampaignList()
        {
            return View();
        }

        #region PictureVote

        /// <summary>
        /// 允许未关注的情况下浏览，但要投票，就要判断活动是否允许未关注者投票了
        /// </summary>
        /// <returns></returns>
        [AllowedOnlyOpenId]
        public ActionResult PictureVote()
        {
            string strCampaignId = Request.QueryString["campaignId"];

            if (String.IsNullOrEmpty(strCampaignId))
            {
                //return RespondResult(false, "参数无效。");

                //兼容旧链接
                strCampaignId = Request.QueryString["id"];
                if (String.IsNullOrEmpty(strCampaignId))
                {
                    return RespondResult(false, "参数无效。");
                }
                else
                {
                    return RedirectToAction("PictureVote",
                    new { domainId = DomainContext.Domain.Id, campaignId = strCampaignId });
                }
            }

            Guid campaignId = Guid.Parse(strCampaignId);

            //递增活动页面PV
            _campaignManager.PageVisit(campaignId);

            PictureVoteViewModel model = new PictureVoteViewModel();
            model.CampaignBundle = _campaignManager.PictureVote.GetPictureVoteBundle(campaignId);

            if (model.CampaignBundle == null || model.CampaignBundle.Campaign == null
                || model.CampaignBundle.PictureVote == null)
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                            "~/Home/ErrorView/?message={0}", "td8"));
            }

            model.DataReport = _campaignManager.PictureVote.GetPictureVoteDataReport(campaignId);

            if (MemberContext != null)
            {
                model.PictureVoteItem = _campaignManager.PictureVote.GetPictureVoteItemByMemberId(MemberContext.Member.Id);
            }

            model.JsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            model.JsApiConfig.JsApiList.Add("onMenuShareTimeline");
            model.JsApiConfig.JsApiList.Add("onMenuShareAppMessage");

            if (MemberContext != null)
                model.Attention = true;

            model.FullParticipant = _campaignManager.PictureVote.PictureVoteIsFullParticipant(campaignId);

            return View(model);
        }

        public ActionResult PictureVoteUpload()
        {
            string strCampaignId = Request.QueryString["campaignId"];

            if (String.IsNullOrEmpty(strCampaignId))
            {
                return RespondResult(false, "参数无效。");
            }

            Guid campaignId = Guid.Parse(strCampaignId);

            CampaignEntity campaign = _campaignManager.GetCampaign(campaignId);
            Campaign_PictureVoteEntity pictureVote = _campaignManager.PictureVote.GetPictureVote(campaignId);

            if (campaign == null || pictureVote == null)
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                            "~/Home/ErrorView/?message={0}", "td8"));
            }

            //判断当前用户上传量是否已达限制，达到限制则直接跳转到查看页面
            List<Campaign_PictureVoteItemEntity> uploadList =
                _campaignManager.PictureVote.GetMemberPictureVoteItemList(campaignId, MemberContext.Member.Id);
            if (uploadList.Count >= pictureVote.MaxPublishTimes)
            {
                return RedirectToAction("PictureVoteItemDetail",
                    new { domainId = DomainContext.Domain.Id, itemId = uploadList[0].Id, campaignId = strCampaignId });

                //return RedirectToRoute(new
                //{
                //    controller = "Campaign",
                //    action = "PictureVoteItemDetail",
                //    domainId = DomainContext.Domain.Id,
                //    id = uploadList[0].Id,
                //    campaignId = strCampaignId
                //});
                //return Redirect(String.Format("~/Campaign/PictureVoteItemDetail/{0}?campaignId={1}&id={2}",
                //    DomainContext.Domain.Id, uploadList[0].Id, strCampaignId));
            }

            PictureVoteUploadViewModel model = new PictureVoteUploadViewModel();
            model.Campaign = campaign;
            model.JsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            model.JsApiConfig.JsApiList.Add("chooseImage");
            model.JsApiConfig.JsApiList.Add("previewImage");
            model.JsApiConfig.JsApiList.Add("uploadImage");
            model.JsApiConfig.JsApiList.Add("downloadImage");
            return View(model);
        }

        /// <summary>
        /// 我要投票中显示的已投票详情
        /// </summary>
        /// <returns></returns>
        public ActionResult PictureVoteItemDetail()
        {
            string strId = Request.QueryString["itemId"];
            string strCampaignId = Request.QueryString["campaignId"];

            if (String.IsNullOrEmpty(strId))
            {
                //兼容旧链接
                strId = Request.QueryString["id"];
                if (String.IsNullOrEmpty(strId))
                {
                    return RespondResult(false, "参数无效。");
                }
                else
                {
                    return RedirectToAction("PictureVoteItemDetail",
                    new { domainId = DomainContext.Domain.Id, itemId = strId, campaignId = strCampaignId });
                }
            }

            Guid id = Guid.Parse(strId);
            Guid campaignId = Guid.Parse(strCampaignId);

            Campaign_PictureVoteItemEntity pictureVoteItem = _campaignManager.PictureVote.GetPictureVoteItem(id);
            if (pictureVoteItem == null || pictureVoteItem.Member != MemberContext.Member.Id)
            {
                return RedirectToAction("PictureVote",
                    new { domainId = DomainContext.Domain.Id, campaignId = strCampaignId });
            }

            PictureVoteItemDetailViewModel model = new PictureVoteItemDetailViewModel();
            model.PictureVoteItem = pictureVoteItem;
            model.Campaign = _campaignManager.GetCampaign(model.PictureVoteItem.CampaignId);

            if (model.Campaign == null)
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                            "~/Home/ErrorView/?message={0}", "td8"));
            }

            model.JsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            model.JsApiConfig.JsApiList.Add("onMenuShareTimeline");
            model.JsApiConfig.JsApiList.Add("onMenuShareAppMessage");

            model.ShareUrl = String.Format(
                "http://{0}/Campaign/PictureVote/{1}?campaignId={2}",
                Request.Url.Host, DomainContext.Domain.Id, strCampaignId);

            return View(model);
        }

        #endregion

        #region MemberQRCode

        public ActionResult MemberQRCode()
        {
            string strCampaignId = Request.QueryString["campaignId"];

            if (String.IsNullOrEmpty(strCampaignId))
            {
                //return RespondResult(false, "参数无效。");

                //兼容旧链接
                strCampaignId = Request.QueryString["id"];
                if (String.IsNullOrEmpty(strCampaignId))
                {
                    return RespondResult(false, "参数无效。");
                }
                else
                {
                    return RedirectToAction("MemberQRCode",
                        new { domainId = DomainContext.Domain.Id, campaignId = strCampaignId });
                }
            }

            Guid campaignId = Guid.Parse(strCampaignId);

            //递增活动页面PV
            _campaignManager.PageVisit(campaignId);

            MemberQRCodeViewModel model = new MemberQRCodeViewModel();
            model.Campaign = _campaignManager.GetCampaign(campaignId);

            if (model.Campaign == null)
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                            "~/Home/ErrorView/?message={0}", "td8"));
            }

            model.DataReport = _campaignManager.MemberQRCode.GetMemberQRCodeDataReport(campaignId);

            model.JsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            model.JsApiConfig.JsApiList.Add("onMenuShareTimeline");
            model.JsApiConfig.JsApiList.Add("onMenuShareAppMessage");
            return View(model);
        }

        [AllowedAnonymous]
        public ActionResult MemberQRCodeShare()
        {
            string strCampaignId = Request.QueryString["campaignId"];
            string strMemberId = Request.QueryString["memberId"];

            if (String.IsNullOrEmpty(strCampaignId) || String.IsNullOrEmpty(strMemberId))
            {
                return RespondResult(false, "参数无效。");
            }

            Guid campaignId = Guid.Parse(strCampaignId);
            Guid memberId = Guid.Parse(strMemberId);

            //递增活动页面PV
            _campaignManager.PageVisit(campaignId);

            MemberQRCodeShareViewModel model = new MemberQRCodeShareViewModel();
            model.Campaign = _campaignManager.GetCampaign(campaignId);
            model.QRCodeItem = _campaignManager.MemberQRCode.GetMemberQRCodeItem(campaignId, memberId);

            if (model.Campaign == null)
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                            "~/Home/ErrorView/?message={0}", "td8"));
            }

            if (model.QRCodeItem == null)
            {
                return new RedirectResult(String.Format(
                    "~/Home/CampaignGuideSubscribe/{0}?campaignId={1}", DomainContext.Domain.Id, strCampaignId));
            }

            //model.JsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            //model.JsApiConfig.JsApiList.Add("onMenuShareTimeline");
            //model.JsApiConfig.JsApiList.Add("onMenuShareAppMessage");
            return View(model);
        }

        /// <summary>
        /// 会员二维码落地
        /// </summary>
        /// <returns></returns>
        [AllowedOnlyOpenId]
        public ActionResult MemberQRCodeLanding()
        {
            string strCampaignId = Request.QueryString["campaignId"];
            string strMemberId = Request.QueryString["memberId"];

            if (String.IsNullOrEmpty(strCampaignId) || String.IsNullOrEmpty(strMemberId))
            {
                return RespondResult(false, "参数无效。");
            }

            Guid campaignId = Guid.Parse(strCampaignId);

            //递增活动页面PV
            _campaignManager.PageVisit(campaignId);

            string landingUrl = _campaignManager.MemberQRCode.MemberQRCodeLanding(campaignId, Guid.Parse(strMemberId), this.OpenId);

            return Redirect(landingUrl);
        }

        #endregion

        #region Lottery

        [AllowedOnlyOpenId]
        public ActionResult Lottery()
        {
            string strCampaignId = Request.QueryString["campaignId"];

            if (String.IsNullOrEmpty(strCampaignId))
            {
                //return RespondResult(false, "参数无效。");

                //兼容旧链接
                strCampaignId = Request.QueryString["id"];
                if (String.IsNullOrEmpty(strCampaignId))
                {
                    return RespondResult(false, "参数无效。");
                }
                else
                {
                    return RedirectToAction("Lottery",
                    new { domainId = DomainContext.Domain.Id, campaignId = strCampaignId });
                }
            }

            Guid campaignId = Guid.Parse(strCampaignId);

            //递增活动页面PV
            _campaignManager.PageVisit(campaignId);

            LotteryViewModel model = new LotteryViewModel();
            model.Campaign = _campaignManager.GetCampaign(campaignId);
            model.Lottery = _campaignManager.Lottery.GetLottery(campaignId);

            if (model.Campaign == null || model.Lottery == null)
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                            "~/Home/ErrorView/?message={0}", "td8"));
            }

            model.DataReport = _campaignManager.Lottery.GetLotteryDataReport(campaignId);

            model.JsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            model.JsApiConfig.JsApiList.Add("onMenuShareTimeline");
            model.JsApiConfig.JsApiList.Add("onMenuShareAppMessage");

            if (MemberContext != null)
                model.Attention = true;

            return View(model);
        }

        #endregion

        #region LuckyTicket

        public ActionResult LuckyTicket()
        {
            string strCampaignId = Request.QueryString["campaignId"];

            if (String.IsNullOrEmpty(strCampaignId))
            {
                //兼容旧链接
                strCampaignId = Request.QueryString["id"];
                if (String.IsNullOrEmpty(strCampaignId))
                {
                    return RespondResult(false, "参数无效。");
                }
                else
                {
                    return RedirectToAction("LuckyTicket",
                    new { domainId = DomainContext.Domain.Id, campaignId = strCampaignId });
                }
            }

            Guid campaignId = Guid.Parse(strCampaignId);

            //递增活动页面PV
            _campaignManager.PageVisit(campaignId);

            LuckyTicketViewModel model = new LuckyTicketViewModel();
            model.CampaignBundle = _campaignManager.LuckyTicket.GetLuckyTicketBundle(campaignId);

            if (model.CampaignBundle == null || model.CampaignBundle.Empty)
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                            "~/Home/ErrorView/?message={0}", "td8"));
            }

            model.DataReport = _campaignManager.LuckyTicket.GetLuckyTicketDataReport(campaignId);
            model.WinLogList = _campaignManager.LuckyTicket.GetLuckyTicketWinLogListByMember(campaignId, MemberContext.Member.Id);

            model.JsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            model.JsApiConfig.JsApiList.Add("onMenuShareTimeline");
            model.JsApiConfig.JsApiList.Add("onMenuShareAppMessage");
            return View(model);
        }

        /// <summary>
        /// 抽奖活动分享的目标地址
        /// </summary>
        /// <returns></returns>
        [AllowedOnlyOpenId]
        public ActionResult LuckyTicketShare()
        {
            string strCampaignId = Request.QueryString["campaignId"];
            string strMemberId = Request.QueryString["memberId"];

            if (String.IsNullOrEmpty(strCampaignId) || String.IsNullOrEmpty(strMemberId))
            {
                return RespondResult(false, "参数无效。");
            }

            Guid campaignId = Guid.Parse(strCampaignId);

            //递增活动页面PV
            _campaignManager.PageVisit(campaignId);

            //为原分享人创建一个抽奖号码
            Campaign_LuckyTicketLogEntity createLuckyTicketLogArgs = new Campaign_LuckyTicketLogEntity();
            createLuckyTicketLogArgs.CampaignId = campaignId;
            createLuckyTicketLogArgs.Domain = DomainContext.Domain.Id;
            createLuckyTicketLogArgs.Member = Guid.Parse(strMemberId);
            createLuckyTicketLogArgs.FromOpenId = OpenId;
            _campaignManager.LuckyTicket.CreateLuckyTicketLog(createLuckyTicketLogArgs);

            //判断当前访问者是否已关注
            //已关注引导到活动页面，未关注引导到引导关注页面
            if (MemberContext != null)
            {
                return RedirectToAction("LuckyTicket",
                   new { domainId = DomainContext.Domain.Id, campaignId = strCampaignId });
            }
            else
            {
                return new RedirectResult(String.Format(
                    "~/Home/CampaignGuideSubscribe/{0}?campaignId={1}", DomainContext.Domain.Id, strCampaignId));
            }
        }

        public ActionResult LuckyTicketDrawResult()
        {
            string strCampaignId = Request.QueryString["campaignId"];

            if (String.IsNullOrEmpty(strCampaignId))
            {
                //兼容旧链接
                strCampaignId = Request.QueryString["id"];
                if (String.IsNullOrEmpty(strCampaignId))
                {
                    return RespondResult(false, "参数无效。");
                }
                else
                {
                    return RedirectToAction("LuckyTicketDrawResult",
                    new { domainId = DomainContext.Domain.Id, campaignId = strCampaignId });
                }
            }

            Guid campaignId = Guid.Parse(strCampaignId);

            LuckyTicketDrawResultViewModel model = new LuckyTicketDrawResultViewModel();
            model.CampaignBundle = _campaignManager.LuckyTicket.GetLuckyTicketBundle(campaignId);

            if (model.CampaignBundle == null || model.CampaignBundle.Empty)
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                            "~/Home/ErrorView/?message={0}", "td8"));
            }

            model.JsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            model.JsApiConfig.JsApiList.Add("onMenuShareTimeline");
            model.JsApiConfig.JsApiList.Add("onMenuShareAppMessage");

            return View(model);
        }

        #endregion

        #region OneDollarBuying

        /// <summary>
        /// 1元抢购主画面
        /// </summary>
        /// <param name="domainId"></param>
        /// <returns></returns>
        public ActionResult OneDollarBuying(string domainId)
        {
            return View();
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="domainId"></param>
        /// <returns></returns>
        public ActionResult OneDollarBuyingCommodityDetail(string domainId)
        {
            string strCommodityId = Request.QueryString["commodityId"];
            Guid commodityId = Guid.Empty;
            if (String.IsNullOrEmpty(strCommodityId) || Guid.TryParse(strCommodityId, out commodityId) == false)
            {
                return RedirectToAction("OneDollarBuying", new { domainId = domainId });
            }

            string strSaleId = Request.QueryString["saleId"];
            Guid saleId = Guid.Empty;
            if (String.IsNullOrEmpty(strSaleId) || Guid.TryParse(strSaleId, out saleId) == false)
            {
                return RedirectToAction("OneDollarBuying", new { domainId = domainId });
            }

            OneDollarBuyingCommodityDetailViewModel model = new OneDollarBuyingCommodityDetailViewModel();
            model.Commodity = _oneDollarBuyingManager.GetCommodity(commodityId);
            model.ForSale = _oneDollarBuyingManager.GetCommodityForSale(saleId);
            model.MyPartNumberList = _oneDollarBuyingManager.GetPartNumberListByMember(saleId, MemberContext.Member.Id);

            if (model.ForSale.LuckyMember.HasValue)
            {
                model.LuckyMember = _memberManager.GetMember(model.ForSale.LuckyMember.Value);
                model.LuckyMemberPartNumberCount =
                    _oneDollarBuyingManager.GetPartNumberCountByMember(saleId, model.ForSale.LuckyMember.Value);
            }

            if (model.ForSale.SoldPart >= model.ForSale.TotalPart)
            {
                model.AvailableSaleId = _oneDollarBuyingManager.GetAvailableSaleId(commodityId);
            }

            return View(model);
        }

        /// <summary>
        /// 最新揭晓
        /// </summary>
        /// <param name="domainId"></param>
        /// <returns></returns>
        public ActionResult OneDollarBuyingLuckyList(string domainId)
        {
            return View();
        }

        /// <summary>
        /// 我的参与
        /// </summary>
        /// <param name="domainId"></param>
        /// <returns></returns>
        public ActionResult OneDollarBuyingParticipatedList(string domainId)
        {
            return View();
        }

        #endregion

        #region ShakingLottery

        public ActionResult ShakingLottery()
        {
            string strCampaignId = Request.QueryString["campaignId"];
            Guid campaignId = Guid.Empty;
            if (String.IsNullOrEmpty(strCampaignId) || Guid.TryParse(strCampaignId, out campaignId) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            //递增活动页面PV
            _campaignManager.PageVisit(campaignId);

            ShakingLotteryViewModel model = new ShakingLotteryViewModel();
            model.CampaignBundle = _campaignManager.ShakingLottery.GetBundle(campaignId);

            if (model.CampaignBundle == null || model.CampaignBundle.Empty)
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                            "~/Home/ErrorView/?message={0}", "td8"));
            }

            if (model.CampaignBundle.ShakingLottery.Mode == EnumCampaign_ShakingLotteryMode.Period)
            {
                if (model.CampaignBundle.ShakingLottery.Period.HasValue)
                {
                    model.CurrentPeriod = _campaignManager.ShakingLottery.GetPeriod(model.CampaignBundle.ShakingLottery.Period.Value);
                }

                if (model.CurrentPeriod != null)
                {
                    model.PlayedTimes = _campaignManager.ShakingLottery.GetMemberPlayedTimes(
                       campaignId, model.CurrentPeriod.Id, MemberContext.Member.Id);
                }
            }
            else
            {
                model.PlayedTimes = _campaignManager.ShakingLottery.GetMemberPlayedTimes(campaignId, null, MemberContext.Member.Id);
            }

            model.DataReport = _campaignManager.ShakingLottery.GetDataReport(campaignId);
            model.GiftList = _campaignManager.ShakingLottery.GetMemberObtainedGiftList(campaignId, MemberContext.Member.Id);

            WeixinJsApiConfig jsApiConfig = new WeixinJsApiConfig();
            jsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            jsApiConfig.JsApiList.Add("onMenuShareTimeline");
            jsApiConfig.JsApiList.Add("onMenuShareAppMessage");
            ViewBag.JsApiConfig = jsApiConfig;

            return View(model);
        }

        #endregion

        #region Donation

        public ActionResult Donation()
        {
            string strCampaignId = Request.QueryString["campaignId"];


            if (String.IsNullOrEmpty(strCampaignId))
            {
                return RespondResult(false, "参数无效。");
            }

            Guid campaignId = Guid.Parse(strCampaignId);

            //递增活动页面PV
            _campaignManager.PageVisit(campaignId);

            DonationViewModel model = new DonationViewModel();
            model.CampaignBundle = _campaignManager.Donation.GetDonationBundle(campaignId);

            if (model.CampaignBundle == null || model.CampaignBundle.Empty)
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                            "~/Home/ErrorView/?message={0}", "td8"));
            }


            model.JsApiConfig = DomainContext.GetJsApiConfig(HttpContext.Request.Url.ToString());
            model.JsApiConfig.JsApiList.Add("onMenuShareTimeline");
            model.JsApiConfig.JsApiList.Add("onMenuShareAppMessage");
            return View(model);
        }

        #endregion

    }
}
