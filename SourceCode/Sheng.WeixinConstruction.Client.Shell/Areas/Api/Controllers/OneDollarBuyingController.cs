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
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using Sheng.WeixinConstruction.WeixinContract.PayApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Areas.Api.Controllers
{
    public class OneDollarBuyingController : ApiBasalController
    {
        private static readonly OneDollarBuyingManager _oneDollarBuyingManager = OneDollarBuyingManager.Instance;
        private static readonly LogService _log = LogService.Instance;

        /// <summary>
        /// 获取销售中的商品列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetForSaleCommodityList()
        {
            GetItemListArgs args = RequestArgs<GetItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _oneDollarBuyingManager.GetForSaleCommodityList(args);
            return RespondDataResult(result);
        }

        public ActionResult OrderOneDollarBuyingCommodity()
        {
            OrderOneDollarBuyingCommodityArgs args = RequestArgs<OrderOneDollarBuyingCommodityArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.MemberId = MemberContext.Member.Id;

            OrderOneDollarBuyingCommodityResult result = _oneDollarBuyingManager.OrderOneDollarBuyingCommodity(args);
            return RespondDataResult(result);
        }

        public ActionResult GetForSaleCommodityMemberPartNumber()
        {
            GetOneDollarBuyingCommodityMemberPartNumberArgs args = RequestArgs<GetOneDollarBuyingCommodityMemberPartNumberArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _oneDollarBuyingManager.GetForSaleCommodityMemberPartNumber(args);
            return RespondDataResult(result);
        }

        public ActionResult GetLuckyList()
        {
            GetItemListArgs args = RequestArgs<GetItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _oneDollarBuyingManager.GetLuckyList(args);
            return RespondDataResult(result);
        }

        public ActionResult GetParticipatedList()
        {
            GetOneDollarBuyingCommodityParticipatedListArgs args = RequestArgs<GetOneDollarBuyingCommodityParticipatedListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.Member = MemberContext.Member.Id;

            GetItemListResult result = _oneDollarBuyingManager.GetParticipatedList(args);
            return RespondDataResult(result);
        }

        public ActionResult GetAvailableSaleId()
        {
            string strCommodityId = Request.QueryString["commodityId"];
            Guid commodityId = Guid.Empty;
            if (String.IsNullOrEmpty(strCommodityId) || Guid.TryParse(strCommodityId, out commodityId) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            Guid? saleId = _oneDollarBuyingManager.GetAvailableSaleId(commodityId);
            if(saleId.HasValue)
            {
                return RespondDataResult(saleId);
            }
            else
            {
                return RespondResult(false, "此商品无在售信息。");
            }
        }

        public ActionResult GetDealInfo()
        {
            string strSaleId = Request.QueryString["saleId"];
            Guid saleId = Guid.Empty;
            if (String.IsNullOrEmpty(strSaleId) || Guid.TryParse(strSaleId, out saleId) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            OneDollarBuyingDealInfo dealInfo = _oneDollarBuyingManager.GetDealInfo(saleId);
            if (dealInfo != null)
            {
                return RespondDataResult(dealInfo);
            }
            else
            {
                return RespondResult(false, "无此条销售信息。");
            }
        }

        #region 管理功能

        public ActionResult GetDealInfoByPeriodNumber()
        {
            string periodNumber = Request.QueryString["periodNumber"];

            if (String.IsNullOrEmpty(periodNumber))
            {
                return RespondResult(false, "参数无效。");
            }

            OneDollarBuyingDealInfo dealInfo =
                _oneDollarBuyingManager.GetDealInfoByPeriodNumber(DomainContext.Domain.Id, DomainContext.AppId, periodNumber);

            if (dealInfo == null)
            {
                return RespondResult(false, "商品不存在。");
            }
            else
            {
                return RespondDataResult(dealInfo);
            }
        }

        public ActionResult Deal()
        {
            string strSaleId = Request.QueryString["saleId"];
            Guid saleId = Guid.Empty;
            if (String.IsNullOrEmpty(strSaleId) || Guid.TryParse(strSaleId, out saleId) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            int result = _oneDollarBuyingManager.Deal(saleId, MemberContext.User.Id);

            #region 操作日志

            if (result == 0)
            {
                _operatedLogManager.Create(new OperatedLogEntity()
                {
                    Domain = DomainContext.Domain.Id,
                    AppId = DomainContext.AppId,
                    User = MemberContext.User.Id,
                    IP = Request.UserHostAddress,
                    Module = EnumModule.OneDollarBuying,
                    Description = "领取1元抢购商品。"
                });
            }

            #endregion

            return RespondDataResult(new { Result = result });
        }

        #endregion
    }
}