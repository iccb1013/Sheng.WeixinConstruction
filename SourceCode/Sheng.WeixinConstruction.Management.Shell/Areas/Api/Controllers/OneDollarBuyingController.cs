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
    public class OneDollarBuyingController : BasalController
    {
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly OneDollarBuyingManager _oneDollarBuyingManager = OneDollarBuyingManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        public ActionResult GetCommodityList()
        {
            GetOneDollarBuyingCommodityListArgs args = RequestArgs<GetOneDollarBuyingCommodityListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _oneDollarBuyingManager.GetCommodityList(args);
            return RespondDataResult(result);
        }

        public ActionResult GetCommodityForSaleList()
        {
            GetOneDollarBuyingCommodityForSaleListArgs args = RequestArgs<GetOneDollarBuyingCommodityForSaleListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _oneDollarBuyingManager.GetCommodityForSaleList(args);
            return RespondDataResult(result);
        }

        public ActionResult CreateCommodity()
        {
            OneDollarBuyingCommodityEntity commodity = RequestArgs<OneDollarBuyingCommodityEntity>();
            if (commodity == null)
            {
                return RespondResult(false, "参数无效。");
            }

            commodity.Domain = UserContext.User.Domain;
            commodity.AppId = DomainContext.AppId;
            commodity.CreateUser = UserContext.User.Id;
            NormalResult result = _oneDollarBuyingManager.CreateCommodity(commodity);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "添加1元抢购商品"
            });

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult UpdateCommodity()
        {
            OneDollarBuyingCommodityEntity commodity = RequestArgs<OneDollarBuyingCommodityEntity>();
            if (commodity == null)
            {
                return RespondResult(false, "参数无效。");
            }

            NormalResult result = _oneDollarBuyingManager.UpdateCommodity(commodity);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "更新1元抢购商品"
            });

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult RemoveCommodity()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            _oneDollarBuyingManager.RemoveCommodity(Guid.Parse(id));

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "删除1元抢购商品"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult BatchRemoveCommodity()
        {

            IdListArgs args = RequestArgs<IdListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            _oneDollarBuyingManager.BatchRemoveCommodity(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "删除1元抢购商品（批量删除）"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult GetCommodity()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            OneDollarBuyingCommodityEntity commodity = _oneDollarBuyingManager.GetCommodity(id);

            return RespondDataResult(commodity);
        }

        public ActionResult StockIncrement()
        {
            OneDollarBuyingCommodityStockAdjustmentArgs args = RequestArgs<OneDollarBuyingCommodityStockAdjustmentArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            int stock = _oneDollarBuyingManager.StockIncrement(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "增加1元抢购商品库存"
            });

            #endregion

            return RespondDataResult(new
            {
                Stock = stock
            });
        }

        public ActionResult StockDecrement()
        {
            OneDollarBuyingCommodityStockAdjustmentArgs args = RequestArgs<OneDollarBuyingCommodityStockAdjustmentArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            int stock = _oneDollarBuyingManager.StockDecrement(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Campaign,
                Description = "消减1元抢购商品库存"
            });

            #endregion

            return RespondDataResult(new
            {
                Stock = stock
            });
        }

        public ActionResult Deal()
        {
            string strSaleId = Request.QueryString["saleId"];
            Guid saleId = Guid.Empty;
            if (String.IsNullOrEmpty(strSaleId) || Guid.TryParse(strSaleId, out saleId) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            int result = _oneDollarBuyingManager.Deal(saleId, UserContext.User.Id);

            return RespondDataResult(new { Result = result });
        }
    }
}