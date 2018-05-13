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
    public class PayController : ApiBasalController
    {
        private static readonly CampaignManager _campaignManager = CampaignManager.Instance;
        private static readonly PayManager _payManager = PayManager.Instance;
        private static readonly LogService _log = LogService.Instance;

        static Random _random = new Random(100);
        // GET: Api/Pay
        public ActionResult Redpack()
        {
            //判断有没有参与投票活动
            int campaignCount = _campaignManager.PictureVote.GetMemberPictureVoteItemCount(
                Guid.Parse("16bc2e8e-8dbd-405a-85cc-34fb1879fd8a"), MemberContext.Member.Id, EnumCampaignPictureVoteItemApproveStatus.Approved);
            if (campaignCount == 0)
            {
                return RespondResult(false, "请参与晒宝宝赢红包活动，上传宝宝照片并通过审核后，即可拆红包！~");
            }

            if (_memberManager.Redpack(DomainContext.Domain.Id, MemberContext.Member.Id) == false)
            {
                return RespondResult(false, "您已经领取过本轮红包啦！~");
            }

            string billno = _random.Next(0, 99999999).ToString();
            billno = "0000000000" + billno;

            WeixinPaySendRedpackArgs args = new WeixinPaySendRedpackArgs();
            args.MchBillno = "10000" + DateTime.Now.ToString("yyyymmdd") + billno.Substring(billno.Length - 10, 10);
            args.MchId = "1277619601";
            args.WxAppId = "wx8c36b3c0000a0a49";
            args.SendName = "升讯威";
            args.ReOpenid = MemberContext.Member.OpenId;
            args.TotalAmount = _random.Next(100, 120);
            args.Wishing = "新的快乐！";
            args.ClientIp = "121.40.198.87";
            args.ActName = "test";
            args.Remark = "红包";

            Log.Write("Redpack 发送：", JsonConvert.SerializeObject(args), TraceEventType.Verbose);

            ApiResult apiResult = new ApiResult();

            WxPayArgs wxPayArgs = new WxPayArgs();
            wxPayArgs.UseCertificate = true;
            wxPayArgs.CertificatePath = @"D:\wwwroot\WeixinConstruction\cert\apiclient_cert.p12";
            wxPayArgs.CertificatePassword = "1277619601";
            wxPayArgs.Key = "192006250b4c09247ec02edce69f6a99";

            RequestPayApiResult<WeixinPaySendRedpackResult> result = WxPayApi.SendRedpack(args, wxPayArgs);

            Log.Write("Redpack 返回：", JsonConvert.SerializeObject(result), TraceEventType.Verbose);


            if (result.Success == false)
            {
                //apiResult.Message = result.Message;
                Log.Write("Redpack 失败：", result.Message,
                    TraceEventType.Warning);
                apiResult.Message = "您来晚了哦，本轮红包已派发完毕，新年期间我们还将继续发起红包大派发活动，敬请留意~";
                return RespondResult(apiResult);
            }

            if (result.ApiResult.ReturnCode == "SUCCESS" && result.ApiResult.ResultCode == "SUCCESS")
            {
                apiResult.Success = true;
                return RespondResult();
            }
            else
            {
                //apiResult.Message = result.ApiResult.ErrCode + result.ApiResult.ErrCodeDes;
                Log.Write("Redpack 失败：", result.ApiResult.ErrCode + result.ApiResult.ErrCodeDes,
                    TraceEventType.Warning);
                apiResult.Message = "您来晚了哦，本轮红包已派发完毕，新年期间我们还将继续发起红包大派发活动，敬请留意~";
                return RespondResult(apiResult);
            }
        }

        /// <summary>
        /// 获取前端 WeixinJSBridge.invoke 使用的支付参数的 json 字符串
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBrandWCPayRequestArgs()
        {
            string strPayOrderId = Request.QueryString["payOrderId"];
            Guid payOrderId = Guid.Empty;
            if (String.IsNullOrEmpty(strPayOrderId) || Guid.TryParse(strPayOrderId, out payOrderId) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            PayOrderEntity payOrder = _payManager.GetPayOrder(payOrderId);
            if (payOrder == null)
            {
                return RespondResult(false, "指定的支付订单不存在。");
            }

            if (payOrder.Member != MemberContext.Member.Id)
            {
                return RespondResult(false, "订单不属于您。");
            }

            if (payOrder.TradeState != EnumPayTradeState.NOTPAY)
            {
                return RespondResult(false, "订单状态不是待支付状态。");
            }

            WxPayData jsApiParam = new WxPayData(DomainContext.AuthorizerPay.Key);
            jsApiParam.SetValue("appId", DomainContext.AppId);
            jsApiParam.SetValue("timeStamp", WeixinApiHelper.GetTimesTamp().ToString());
            jsApiParam.SetValue("nonceStr", WeixinApiHelper.GetNonceStr());
            jsApiParam.SetValue("package", "prepay_id=" + payOrder.PrepayId);
            jsApiParam.SetValue("signType", "MD5");
            jsApiParam.SetValue("paySign", jsApiParam.MakeSign());

            string parameters = jsApiParam.ToJson();

            return RespondDataResult(parameters);

        }

        public ActionResult GetCashAccountTrackList()
        {
            GetCashAccountTrackListArgs args = RequestArgs<GetCashAccountTrackListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.Member = MemberContext.Member.Id;

            GetItemListResult result = _payManager.GetCashAccountTrackList(args);
            return RespondDataResult(result);
        }

        public ActionResult RefreshPayOrder()
        {
            string strOutTradeNo = Request.QueryString["outTradeNo"];
            if (String.IsNullOrEmpty(strOutTradeNo))
            {
                return RespondResult(false, "参数无效。");
            }

            NormalResult result = _payManager.RefreshPayOrder(strOutTradeNo, DomainContext.AuthorizerPay);

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult ClosePayOrder()
        {
            string strOutTradeNo = Request.QueryString["outTradeNo"];
            if (String.IsNullOrEmpty(strOutTradeNo))
            {
                return RespondResult(false, "参数无效。");
            }

            NormalResult result = _payManager.ClosePayOrder(strOutTradeNo, DomainContext.AuthorizerPay);

            if (result.Success)
            {
                _payManager.RefreshPayOrder(strOutTradeNo, DomainContext.AuthorizerPay);
            }

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult Deposit()
        {
            CashAccountDepositArgs args = RequestArgs<CashAccountDepositArgs>();
            if (args == null || args.Fee <= 0)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Fee = args.Fee * 100;

            args.MemberId = MemberContext.Member.Id;
            args.OpenId = MemberContext.Member.OpenId;
            args.SpbillCreateIp = Request.UserHostAddress;

            NormalResult<CreatePayOrderResult> depositResult =
                _payManager.Deposit(args, DomainContext.AuthorizerPay);
            if (depositResult.Success)
            {
                WxPayData jsApiParam = new WxPayData(DomainContext.AuthorizerPay.Key);
                jsApiParam.SetValue("appId", DomainContext.AppId);
                jsApiParam.SetValue("timeStamp", WeixinApiHelper.GetTimesTamp().ToString());
                jsApiParam.SetValue("nonceStr", WeixinApiHelper.GetNonceStr());
                jsApiParam.SetValue("package", "prepay_id=" + depositResult.Data.PrepayId);
                jsApiParam.SetValue("signType", "MD5");
                jsApiParam.SetValue("paySign", jsApiParam.MakeSign());

                string parameters = jsApiParam.ToJson();

                //WeixinPayGetBrandWCPayRequestArgs result = new WeixinPayGetBrandWCPayRequestArgs();
                //result.AppId = this.DomainContext.AppId;
                //result.TimeStamp = WeixinApiHelper.GetTimesTamp().ToString();
                //result.NonceStr = WeixinApiHelper.GetNonceStr();
                //result.Package = "prepay_id=" + depositResult.Data.PrepayId;
                //result.SignType = "MD5";

                return RespondDataResult(parameters);
            }
            else
            {
                return RespondResult(false, depositResult.Message);
            }
        }

        public ActionResult GetPayOrderList()
        {
            GetPayOrderListArgs args = RequestArgs<GetPayOrderListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.Member = MemberContext.Member.Id;

            GetItemListResult result = _payManager.GetPayOrderList(args);
            return RespondDataResult(result);
        }

        /// <summary>
        /// 接收微信服务器返回的 支付结果通用通知
        /// https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=9_7
        /// </summary>
        /// <returns></returns>
        [AllowedAnonymous]
        public ActionResult PayNotify()
        {
            string postString;
            using (Stream stream = HttpContext.Request.InputStream)
            {
                Byte[] postBytes = new Byte[stream.Length];
                stream.Read(postBytes, 0, (int)stream.Length);
                postString = Encoding.UTF8.GetString(postBytes);
            }

            _log.Write("收到支付结果通知", postString, TraceEventType.Verbose);

            NormalResult normalResult = _payManager.Notify(postString, DomainContext.AuthorizerPay);

            string isSuccess;
            if (normalResult.Success)
            {
                isSuccess = "SUCCESS";
            }
            else
            {
                isSuccess = "FAIL";
            }

            string resultContent =
                String.Format("<xml><return_code><![CDATA[{0}]]></return_code><return_msg><![CDATA[{1}]]></return_msg></xml>",
                isSuccess, normalResult.Message);

            _log.Write("支付结果通知返回", resultContent, TraceEventType.Verbose);

            return new ContentResult() { Content = resultContent };
        }

        #region 管理功能

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
            args.OperatorUser = MemberContext.User.Id;

            CashAccountTrackResult trackResult = _payManager.CashDeposit(args);

            #region 操作日志

            if (trackResult.Success)
            {
                _operatedLogManager.Create(new OperatedLogEntity()
                {
                    Domain = DomainContext.Domain.Id,
                    AppId = DomainContext.AppId,
                    User = MemberContext.User.Id,
                    IP = Request.UserHostAddress,
                    Module = EnumModule.Pay,
                    Description = "现金充值。"
                });
            }

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

            args.Fee = args.Fee * 100;

            args.Domain = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            args.IP = Request.UserHostAddress;
            args.OperatorUser = MemberContext.User.Id;

            CashAccountTrackResult trackResult = _payManager.Charge(args);

            #region 操作日志

            if (trackResult.Success)
            {
                _operatedLogManager.Create(new OperatedLogEntity()
                {
                    Domain = DomainContext.Domain.Id,
                    AppId = DomainContext.AppId,
                    User = MemberContext.User.Id,
                    IP = Request.UserHostAddress,
                    Module = EnumModule.Pay,
                    Description = "现金消费。"
                });
            }

            #endregion

            return RespondDataResult(trackResult);
        }

        #endregion
    }
}