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
    public class PayController : BasalController
    {
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly OneDollarBuyingManager _oneDollarBuyingManager = OneDollarBuyingManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;
        private static readonly PayManager _payManager = PayManager.Instance;

        #region 账户操作

        public ActionResult CashDeposit()
        {
            CashAccountCashDepositArgs args = RequestArgs<CashAccountCashDepositArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            //JS浮点乘法有BUG，不可在前端运算，如1.11*100得到错误结果
            args.Fee = args.Fee * 100;

            args.Domain = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.IP = Request.UserHostAddress;
            args.OperatorUser = UserContext.User.Id;

            CashAccountTrackResult trackResult = _payManager.CashDeposit(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Pay,
                Description = "现金充值"
            });

            #endregion

            return RespondDataResult(trackResult);
        }

        public ActionResult Charge()
        {
            CashAccountChargeArgs args = RequestArgs<CashAccountChargeArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            //JS浮点乘法有BUG，不可在前端运算，如1.11*100得到错误结果
            args.Fee = args.Fee * 100;

            args.Domain = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.IP = Request.UserHostAddress;
            args.OperatorUser = UserContext.User.Id;

            CashAccountTrackResult trackResult = _payManager.Charge(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Pay,
                Description = "现金消费"
            });

            #endregion

            return RespondDataResult(trackResult);
        }

        #endregion
    }
}