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
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Areas.Api.Controllers
{
    public class CouponController : ApiBasalController
    {
        private static readonly FileService _fileService = FileService.Instance;
        private static readonly CouponManager _couponManager = CouponManager.Instance;

        public ActionResult GetMemberCouponList()
        {
            GetMemberCouponListArgs args = RequestArgs<GetMemberCouponListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.MemberId = MemberContext.Member.Id;

            GetItemListResult result = _couponManager.GetMemberCouponList(args);
            return RespondDataResult(result);
        }

        public ActionResult GetCouponRecordQRCode()
        {
            string strRecordId = Request.QueryString["recordId"];
            Guid recordId = Guid.Empty;
            if (String.IsNullOrEmpty(strRecordId) || Guid.TryParse(strRecordId, out recordId) == false)
            {
                return RespondResult(false, "参数无效。");
            }
            

            string url = _couponManager.GetRecordQRCodeImageUrl(recordId);

            if (String.IsNullOrEmpty(url))
            {
                string serialNumber = Request.QueryString["serialNumber"];

                string content = String.Format(_settingsManager.GetClientAddress(DomainContext) +
                    "Home/QRCode/{0}?type=coupon&serialNumber={1}",
                    DomainContext.Domain.Id, serialNumber);

                GetQRCodeImageArgs getQRCodeImageArgs = new GetQRCodeImageArgs();
                getQRCodeImageArgs.Content = content;
                getQRCodeImageArgs.Domain = DomainContext.Domain.Id;
                GetQRCodeImageResult getQRCodeImageResult = _fileService.GetQRCodeImage(getQRCodeImageArgs);

                if (getQRCodeImageResult.Success == false)
                {
                    return RespondResult(false, "生成二维码失败：" + getQRCodeImageResult.Message);
                }

                url = _fileService.FileServiceUri + getQRCodeImageResult.FileName;

                _couponManager.UpdateRecordQRCodeImageUrl(recordId, url);
            }

            return RespondDataResult(url);
        }

        #region 管理功能

        public ActionResult GetCouponRecordBySerialNumber()
        {
            string serialNumber = Request.QueryString["serialNumber"];

            if (String.IsNullOrEmpty(serialNumber))
            {
                return RespondResult(false, "参数无效。");
            }

            CouponRecordEntity couponRecord = _couponManager.GetCouponRecordBySerialNumber(
                DomainContext.Domain.Id, DomainContext.AppId, serialNumber);

            if (couponRecord == null)
            {
                return RespondResult(false, "卡券不存在。");
            }
            else
            {
                CouponEntity coupon = _couponManager.GetCoupon(couponRecord.Coupon);
                return RespondDataResult(new
                {
                    Coupon = coupon,
                    CouponRecord = couponRecord
                });
            }
        }

        public ActionResult Charge()
        {
            string strRecordIdId = Request.QueryString["recordId"];
            Guid recordId = Guid.Empty;
            if (String.IsNullOrEmpty(strRecordIdId) || Guid.TryParse(strRecordIdId, out recordId) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            CouponChargeArgs args = new CouponChargeArgs();
            args.Domain = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.CouponRecordId = recordId;
            args.ChargeUser = MemberContext.User.Id;
            args.ChargeIP = Request.UserHostAddress;

            NormalResult result = _couponManager.Charge(args);

            #region 操作日志

            if (result.Success)
            {
                _operatedLogManager.Create(new OperatedLogEntity()
                {
                    Domain = DomainContext.Domain.Id,
                    AppId = DomainContext.AppId,
                    User = MemberContext.User.Id,
                    IP = Request.UserHostAddress,
                    Module = EnumModule.Coupon,
                    Description = "核销卡券"
                });
            }

            #endregion

            return RespondDataResult(result);
        }

        #endregion

    }
}