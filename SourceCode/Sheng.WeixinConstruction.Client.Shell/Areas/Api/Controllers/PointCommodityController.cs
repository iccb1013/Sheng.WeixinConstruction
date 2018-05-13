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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Areas.Api.Controllers
{
    public class PointCommodityController : ApiBasalController
    {
        private static readonly PointCommodityManager _pointCommodityManager = PointCommodityManager.Instance;

        public ActionResult GetPointCommodityList()
        {
            GetPointCommodityListArgs args = RequestArgs<GetPointCommodityListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }
            //只取上架的商品
            args.ForSale = true;
            GetItemListResult<PointCommodityEntity> result =
                _pointCommodityManager.GetPointCommodityList(DomainContext.Domain.Id,DomainContext.AppId, args);
            return RespondDataResult(result);
        }

        //public ActionResult GetPointCommodity()
        //{
        //    string id = Request.QueryString["id"];

        //    if (String.IsNullOrEmpty(id))
        //    {
        //        return RespondResult(false, "参数无效。");
        //    }

        //    PointCommodity pointCommodity = _pointCommodityManager.GetPointCommodity(Guid.Parse(id));

        //    return RespondDataResult(pointCommodity);
        //}

        //public ActionResult OrderPointCommodity()
        //{
        //    OrderPointCommodityArgs args = RequestArgs<OrderPointCommodityArgs>();
        //    if (args == null)
        //    {
        //        return RespondResult(false, "参数无效。");
        //    }

        //    args.MemberId = MemberContext.Member.Id;

        //    OrderPointCommodityResult result =
        //        _pointCommodityManager.OrderPointCommodity(DomainContext.Domain.Id,DomainContext.AppId, args);
        //    return RespondDataResult(result);
        //}

        public ActionResult GetMemberPointCommodityOrderList()
        {
            GetMemberPointCommodityOrderListArgs args = RequestArgs<GetMemberPointCommodityOrderListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.Member = MemberContext.Member.Id;

            GetItemListResult result = _pointCommodityManager.GetMemberOrderList(args);
            return RespondDataResult(result);
        }

        public ActionResult AddToCart()
        {
            PointCommodityAddToCartArgs args = RequestArgs<PointCommodityAddToCartArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.Member = MemberContext.Member.Id;

            NormalResult<PointCommodityShoppingCartOperateResult> result = _pointCommodityManager.AddToCart(args);
            return RespondDataResult(result);
        }

        public ActionResult RemoveFormCart()
        {
            PointCommodityAddToCartArgs args = RequestArgs<PointCommodityAddToCartArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.Member = MemberContext.Member.Id;

            NormalResult result = _pointCommodityManager.RemoveFormCart(args);
            return RespondDataResult(result);
        }

        public ActionResult SetCartItemQuantity()
        {
            PointCommodityAddToCartArgs args = RequestArgs<PointCommodityAddToCartArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.Member = MemberContext.Member.Id;

            NormalResult result = _pointCommodityManager.SetCartItemQuantity(args);
            return RespondDataResult(result);
        }

        public ActionResult GetShoppingCartItemList()
        {
            GetItemListResult result =
               _pointCommodityManager.GetShoppingCartItemList(MemberContext.Member.Id);
            return RespondDataResult(result);
        }

        /*
         * 关于订单支付
         * 如果订单只需要消费积分，则在提交订单时，直接扣减积分成单
         * 如果需要现金消费，则先不扣积分，在生成订单后，转到订单详情画面，用户确认支付成功时再作扣减
         * 如果账户余额足够且用户完全使用了账户余额，则扣减余额和积分，成单
         * 如果需要微信支付，则生成微信支付订单，在完成微信支付之后的回调中，再扣减积分、余额，成单
         * 如果微信支付回调时，积分或余额不足，则微信支付的钱转到账户余额
         */

        /// <summary>
        /// 购物车结算，提交订单
        /// CheckoutOrder
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateOrder()
        {
            PointCommodityCheckoutOrderArgs args = RequestArgs<PointCommodityCheckoutOrderArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.MemberId = MemberContext.Member.Id;

            NormalResult<PointCommodityCheckoutOrderResult> result = _pointCommodityManager.CheckoutOrder(args);
            return RespondDataResult(result);
        }

        /// <summary>
        /// 订单支付
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderPay()
        {
            PointCommodityOrderPayArgs args = RequestArgs<PointCommodityOrderPayArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.MemberId = MemberContext.Member.Id;
            args.OpenId = MemberContext.Member.OpenId;
            args.SpbillCreateIp = Request.UserHostAddress;

            NormalResult<PointCommodityOrderPayResult> result = _pointCommodityManager.OrderPay(args, DomainContext.AuthorizerPay);
            return RespondDataResult(result);
        }

        #region 管理功能

        public ActionResult GetPointCommodityOrderByOrderNumber()
        {
            string orderNumber = Request.QueryString["orderNumber"];

            if (String.IsNullOrEmpty(orderNumber))
            {
                return RespondResult(false, "参数无效。");
            }

            PointCommodityOrderEntity pointCommodityOrder =
                _pointCommodityManager.GetOrderByOrderNumber(DomainContext.Domain.Id, DomainContext.AppId, orderNumber);

            if (pointCommodityOrder == null)
            {
                return RespondResult(false, "订单不存在。");
            }
            else
            {
                MemberEntity member = _memberManager.GetMember(pointCommodityOrder.Member);
                if (member == null)
                {
                    return RespondResult(false, "会员数据异常。");
                }

                return RespondDataResult(new
                {
                    Order = pointCommodityOrder,
                    Member = member,
                    ItemList = _pointCommodityManager.GetOrderItemList(pointCommodityOrder.Id)
                });
            }
        }

        public ActionResult DealPointCommodityOrder()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            int result = _pointCommodityManager.DealOrder(Guid.Parse(id),MemberContext.User.Id);

            //#region 操作日志

            if (result == 0)
            {
                _operatedLogManager.Create(new OperatedLogEntity()
                {
                    Domain = DomainContext.Domain.Id,
                    AppId = DomainContext.AppId,
                    User = MemberContext.User.Id,
                    IP = Request.UserHostAddress,
                    Module = EnumModule.PointCommodity,
                    Description = "兑换积分商品"
                });
            }

            //#endregion

            return RespondDataResult(new { Result = result });
        }


        #endregion
    }
}