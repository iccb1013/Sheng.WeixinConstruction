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

namespace Sheng.WeixinConstruction.Management.Shell.Areas.Api.Controllers
{
    public class CouponController : BasalController
    {
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly CouponManager _couponManager = CouponManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        public ActionResult GetCouponList()
        {
            GetCouponListArgs args = RequestArgs<GetCouponListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _couponManager.GetCouponList(args);
            return RespondDataResult(result);
        }

        public ActionResult GetCoupon()
        {

            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            CouponEntity coupon = _couponManager.GetCoupon(id);

            return RespondDataResult(coupon);
        }

        public ActionResult CreateCoupon()
        {
            CouponEntity coupon = RequestArgs<CouponEntity>();
            if (coupon == null)
            {
                return RespondResult(false, "参数无效。");
            }

            coupon.Domain = UserContext.User.Domain;
            coupon.AppId = DomainContext.AppId;
            coupon.CreateUser = UserContext.User.Id;
            _couponManager.CreateCoupon(coupon);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Coupon,
                Description = "添加卡券"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateCoupon()
        {
            CouponEntity coupon = RequestArgs<CouponEntity>();
            if (coupon == null)
            {
                return RespondResult(false, "参数无效。");
            }

            _couponManager.UpdateCoupon(coupon);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Coupon,
                Description = "更新卡券"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveCoupon()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            _couponManager.RemoveCoupon(id);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Coupon,
                Description = "删除卡券"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult StockIncrement()
        {
            CouponStockAdjustmentArgs args = RequestArgs<CouponStockAdjustmentArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            int stock = _couponManager.StockIncrement(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Coupon,
                Description = "增加库存"
            });

            #endregion

            return RespondDataResult(new
            {
                Stock = stock
            });
        }

        public ActionResult StockDecrement()
        {
            CouponStockAdjustmentArgs args = RequestArgs<CouponStockAdjustmentArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            int stock = _couponManager.StockDecrement(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Coupon,
                Description = "消减库存"
            });

            #endregion

            return RespondDataResult(new
            {
                Stock = stock
            });
        }

        public ActionResult GetDistributedCouponList()
        {
            GetDistributedCouponListArgs args = RequestArgs<GetDistributedCouponListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _couponManager.GetDistributedCouponList(args);
            return RespondDataResult(result);
        }

        public ActionResult Distribute()
        {
            CouponDistributeArgs args = RequestArgs<CouponDistributeArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;
            args.DistributeUser = UserContext.User.Id;
            args.DistributeIP = Request.UserHostAddress;
            if (args.LimitedTime.HasValue)
            {
                args.LimitedTime = args.LimitedTime.Value.Add(new TimeSpan(23, 59, 59));
            }

            NormalResult result = _couponManager.Distribute(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Coupon,
                Description = "派发卡券"
            });

            #endregion

            return RespondDataResult(result);
        }
    }
}