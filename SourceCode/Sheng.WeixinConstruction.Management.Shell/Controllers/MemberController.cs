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
    public class MemberController : BasalController
    {
        private static readonly MemberManager _memberManager = MemberManager.Instance;
        private static readonly MemberGroupManager _memberGroupManager = MemberGroupManager.Instance;
        private static readonly GroupMessageManager _groupMessageManager = GroupMessageManager.Instance;
        private static readonly CouponManager _couponManager = CouponManager.Instance;
        private static readonly PointCommodityManager _pointCommodityManager = PointCommodityManager.Instance;
        private static readonly RecommendUrlManager _recommendUrlManager = RecommendUrlManager.Instance;


        #region 会员管理

        public ActionResult Member()
        {
            //GetMaterialListArgs args = new GetMaterialListArgs();
            //args.Type = MaterialType.Image;
            //args.Count = 20;
            //args.Offset = 0;
            //RequestApiResult<GetNormalMaterialListResult> r = MaterialApiWrapper.GetNormalMaterialList(this.DomainContext, args);

            MemberViewModel model = new MemberViewModel();
            model.DomainContext = this.DomainContext;
            return View(model);
        }

        public ActionResult MemberInfo()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                //TODO:ID无效
                //return RedirectToAction("PointCommodity", new { domain = domain });
            }

            MemberInfoViewModel model = new MemberInfoViewModel();
            model.MemberCardLevelList = _memberManager.GetMemberCardList(DomainContext.Domain.Id, DomainContext.AppId);
            model.Member = _memberManager.GetMember(id);
            if (model.Member.CardLevel.HasValue)
            {
                model.MemberCard = _memberManager.GetMemberCard(model.Member.CardLevel.Value);
            }
            else
            {
                SettingsEntity settings = DomainContext.Settings;
                if (settings != null && settings.DefaultMemberCardLevel.HasValue)
                {
                    model.MemberCard = _memberManager.GetMemberCard(settings.DefaultMemberCardLevel.Value);
                }
            }
            return View(model);
        }

        public ActionResult MemberGroupEdit()
        {
            return View();
        }

        public ActionResult MemberSelect()
        {
            return View();
        }

        #endregion

        #region 消息群发

        /// <summary>
        /// 群发功能
        /// </summary>
        /// <returns></returns>
        public ActionResult GroupMessage()
        {
            MemberGroupMessageViewModel model = new MemberGroupMessageViewModel();
            model.MemberGroupList = _memberGroupManager.GetMemberGroupList(DomainContext.Domain.Id,DomainContext.AppId);
            return View(model);
        }

        public ActionResult SentGroupMessage()
        {
            return View();
        }

        public ActionResult GroupMessageDetail()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                //TODO:ID无效
                //return RedirectToAction("PointCommodity", new { domain = domain });
            }

            GroupMessageDetailViewModel model = new GroupMessageDetailViewModel();
            model.GroupMessage = _groupMessageManager.GetGroupMessage(id);
            return View(model);
        }

        #endregion

        #region 积分商品

        public ActionResult PointCommodity()
        {
            //PointCommodityViewModel model = new PointCommodityViewModel();
            return View();
        }

        public ActionResult PointCommodityEdit()
        {
            return View();
        }

        /// <summary>
        /// 订单列表
        /// </summary>
        /// <returns></returns>
        public ActionResult PointCommodityOrder()
        {
            return View();
        }

        public ActionResult PointCommodityOrderDetail()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                //TODO:ID无效
                //return RedirectToAction("PointCommodity", new { domain = domain });
            }

            PointCommodityOrderDetailViewModel model = new PointCommodityOrderDetailViewModel();
            model.Order = _pointCommodityManager.GetOrder(id);
            model.Member = _memberManager.GetMember(model.Order.Member);
            model.ItemList = _pointCommodityManager.GetOrderItemList(id);
            model.LogList = _pointCommodityManager.GetOrderLogList(id);
            return View(model);
        }

        #endregion

        #region 卡券

        public ActionResult Coupon()
        {
            return View();
        }

        public ActionResult CouponEdit()
        {
            return View();
        }

        /// <summary>
        /// 已派发卡券列表
        /// </summary>
        /// <returns></returns>
        public ActionResult DistributedCoupon()
        {
            string strCouponId = Request.QueryString["couponId"];
            Guid couponId = Guid.Empty;
            if (String.IsNullOrEmpty(strCouponId) || Guid.TryParse(strCouponId, out couponId) == false)
            {
                //TODO:ID无效
                //return RedirectToAction("PointCommodity", new { domain = domain });
            }

            DistributedCouponViewModel model = new DistributedCouponViewModel();
            model.Coupon = _couponManager.GetCoupon(couponId);
            return View(model);
        }

        /// <summary>
        /// 派发卡券
        /// </summary>
        /// <returns></returns>
        public ActionResult CouponDistribute()
        {
            string strCouponId = Request.QueryString["couponId"];
            Guid couponId = Guid.Empty;
            if (String.IsNullOrEmpty(strCouponId) || Guid.TryParse(strCouponId, out couponId) == false)
            {
                //TODO:ID无效
                //return RedirectToAction("PointCommodity", new { domain = domain });
            }

            CouponEntity coupon = _couponManager.GetCoupon(couponId);
            return View(coupon);
        }

        #endregion

        #region 多级推广

        public ActionResult RecommendUrl()
        {
            //没有绑定公众号的，初始化一个临时的设置对象用于UI显示
            //此时UI是不允许编辑的
            RecommendUrlSettingsEntity settings;
            if (DomainContext.Online)
            {
                settings = _recommendUrlManager.GetSettings(DomainContext.Domain.Id, DomainContext.AppId);
            }
            else
            {
                settings = new RecommendUrlSettingsEntity();
            }
            
            return View(settings);
        }

        #endregion
    }
}