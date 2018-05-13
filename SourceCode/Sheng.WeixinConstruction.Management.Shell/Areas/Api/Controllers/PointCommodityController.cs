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
    public class PointCommodityController : BasalController
    {
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly PointCommodityManager _pointCommodityManager = PointCommodityManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        public ActionResult GetPointCommodityList()
        {
            //domainId，如果是微信端，则微信端的WEB页面要自动完成一个login操作
            //在SESSION中记录用户信息
            GetPointCommodityListArgs args = RequestArgs<GetPointCommodityListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult<PointCommodityEntity> result =
                _pointCommodityManager.GetPointCommodityList(UserContext.User.Domain, DomainContext.AppId, args);
            return RespondDataResult(result);
        }

        public ActionResult CreatePointCommodity()
        {
            PointCommodityEntity pointCommodity = RequestArgs<PointCommodityEntity>();
            if (pointCommodity == null)
            {
                return RespondResult(false, "参数无效。");
            }

            pointCommodity.Id = Guid.NewGuid();
            pointCommodity.Domain = UserContext.User.Domain;
            pointCommodity.AppId = DomainContext.AppId;

            NormalResult result = _pointCommodityManager.CreatePointCommodity(this.DomainContext, pointCommodity);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.PointCommodity,
                Description = "添加积分商品"
            });

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult UpdatePointCommodity()
        {
            PointCommodityEntity pointCommodity = RequestArgs<PointCommodityEntity>();
            if (pointCommodity == null)
            {
                return RespondResult(false, "参数无效。");
            }

            pointCommodity.Domain = UserContext.User.Domain;
            pointCommodity.AppId = DomainContext.AppId;

            NormalResult result = _pointCommodityManager.UpdatePointCommodity(pointCommodity);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.PointCommodity,
                Description = "更新积分商品"
            });

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult RemovePointCommodity()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _pointCommodityManager.RemovePointCommodity(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.PointCommodity,
                Description = "删除积分商品"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult BatchRemovePointCommodity()
        {

            IdListArgs args = RequestArgs<IdListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            _pointCommodityManager.BatchRemovePointCommodity(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.PointCommodity,
                Description = "删除积分商品（批量删除）"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetPointCommodity()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            PointCommodityEntity pointCommodity = _pointCommodityManager.GetPointCommodity(Guid.Parse(id));

            return RespondDataResult(pointCommodity);
        }

        public ActionResult StockIncrement()
        {
            PointCommodityStockAdjustmentArgs args = RequestArgs<PointCommodityStockAdjustmentArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            int stock = _pointCommodityManager.StockIncrement(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.PointCommodity,
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
            PointCommodityStockAdjustmentArgs args = RequestArgs<PointCommodityStockAdjustmentArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            int stock = _pointCommodityManager.StockDecrement(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.PointCommodity,
                Description = "消减库存"
            });

            #endregion

            return RespondDataResult(new
            {
                Stock = stock
            });
        }

        public ActionResult GetPointCommodityOrderList()
        {
            GetPointCommodityOrderListArgs args = RequestArgs<GetPointCommodityOrderListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _pointCommodityManager.GetOrderList(args);
            return RespondDataResult(result);
        }

        public ActionResult DealPointCommodityOrder()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            int result = _pointCommodityManager.DealOrder(Guid.Parse(id), UserContext.User.Id);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.PointCommodity,
                Description = "兑换积分商品"
            });

            #endregion

            return RespondDataResult(new { Result = result });
        }
    }
}