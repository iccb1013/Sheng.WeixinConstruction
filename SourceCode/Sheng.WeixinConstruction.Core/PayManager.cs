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
using Linkup.Data;
using Linkup.DataRelationalMapping;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using Sheng.WeixinConstruction.WeixinContract.PayApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class PayManager
    {
        private static readonly PayManager _instance = new PayManager();
        public static PayManager Instance
        {
            get { return _instance; }
        }

        private LogService _log = LogService.Instance;
        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        public NormalResult Notify(string xml, AuthorizerPayConfig config)
        {
            _log.Write("收到支付结果通知", xml, TraceEventType.Verbose);

            NormalResult result = new NormalResult();

            WxPayArgs wxPayArgs = config.GetWxPayArgs(false);

            RequestPayApiResult<WeixinPayNotify> notifyResult = WxPayApi.Notify(xml, wxPayArgs);
            if (notifyResult.Success == false)
            {
                _log.Write("支付结果通知显示失败", notifyResult.Message, TraceEventType.Verbose);

                result.Success = false;
                result.Message = notifyResult.Message;
                return result;
            }

            WeixinPayNotify notify = notifyResult.ApiResult;

            Guid payOrderId;
            if (Guid.TryParse(notify.Attach, out payOrderId) == false)
            {
                string attacth = String.Empty;
                if (notify.Attach != null)
                {
                    attacth = notify.Attach;
                }
                _log.Write("Attach 无法转为本地订单Id" + attacth, notify.OutTradeNo, TraceEventType.Verbose);
                result.Success = false;
                result.Message = "Attach 无法转为本地订单Id。" + attacth;
                return result;
            }

            bool updateNotifyStatus = UpdateNotifyStatus(payOrderId);

            if (updateNotifyStatus == false)
            {
                //已经处理过了
                //处理过的还是返回true，防止微信服务器反复推送通知
                //微信文档上说通知频率为15/15/30/180/1800/1800/1800/1800/3600，单位：秒
                //但是实测支付成功后连续推送通知过来
                result.Success = true;
                result.Message = "已经接收到过此订单的支付通知。";
                return result;
            }

            SqlStructureBuild sqlBuild = new SqlStructureBuild();
            sqlBuild.Table = "PayOrder";
            sqlBuild.Type = SqlExpressionType.Update;
            sqlBuild.AddParameter("Id", payOrderId, true);
            sqlBuild.AddParameter("Notify", true);
            if (notify.ReturnCode == "SUCCESS" && notify.ResultCode == "SUCCESS")
            {
                sqlBuild.AddParameter("TradeState", EnumPayTradeState.SUCCESS);
            }
            else
            {
                sqlBuild.AddParameter("TradeState", EnumPayTradeState.PAYERROR);
            }
            sqlBuild.AddParameter("BankType", notify.BankType);
            sqlBuild.AddParameter("FeeType", notify.FeeType);
            sqlBuild.AddParameter("CouponFee", notify.CouponFee);
            sqlBuild.AddParameter("CouponCount", notify.CouponCount);
            sqlBuild.AddParameter("TransactionId", notify.TransactionId);
            sqlBuild.AddParameter("TimeEnd", WeixinApiHelper.ConvertStringToDateTime(notify.TimeEnd));
            sqlBuild.AddParameter("Notify_ReturnCode", notify.ReturnCode);
            sqlBuild.AddParameter("Notify_ReturnMsg", notify.ReturnMsg);
            sqlBuild.AddParameter("Notify_ResultCode", notify.ResultCode);
            sqlBuild.AddParameter("Notify_ErrCode", notify.ErrCode);
            sqlBuild.AddParameter("Notify_ErrCodeDes", notify.ErrCodeDes);
            int affectedRowCount = _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());

            if (affectedRowCount == 0)
            {
                _log.Write("本地订单不存在", notify.OutTradeNo, TraceEventType.Verbose);
                result.Success = false;
                result.Message = "本地订单不存在。";
                return result;
            }

            PayOrderEntity payOrder = GetPayOrder(payOrderId);

            if (notify.CouponCount > 0)
            {
                foreach (WeixinPayNotify_Coupon coupon in notify.CouponList)
                {
                    coupon.PayOrderId = payOrder.Id;
                    _dataBase.Insert(coupon);
                }
            }

            if (notify.ReturnCode == "SUCCESS" && notify.ResultCode == "SUCCESS")
            {
                switch (payOrder.Type)
                {
                    case EnumPayOrderType.Unknow:
                        _log.Write("收到支付结果通知", "未知订单类型", TraceEventType.Warning);
                        break;
                    case EnumPayOrderType.Deposit:
                        ProcessDepositPayNotify(payOrder);
                        break;
                    case EnumPayOrderType.PointCommodity:
                        ProcessPointCommodityPayNotify(payOrder);
                        break;
                    case EnumPayOrderType.Donation:
                        ProcessDonationPayNotify(payOrder);
                        break;
                    default:
                        _log.Write("收到支付结果通知", "订单类型未处理：" + payOrder.Type.ToString(), TraceEventType.Warning);
                        break;
                }
            }

            //更新一下订单状态
            RefreshPayOrder(payOrder.OutTradeNo, config);

            result.Success = true;
            return result;
        }

        /// <summary>
        /// 创建一个微信支付订单
        /// 默认在2小时后过期
        /// </summary>
        /// <param name="args"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public NormalResult<CreatePayOrderResult> CreatePayOrder(CreatePayOrderArgs args, AuthorizerPayConfig config)
        {
            NormalResult<CreatePayOrderResult> result = new NormalResult<CreatePayOrderResult>(false);

            if (config == null)
            {
                result.Message = "当前公众号没有微信支付所需配置信息。";
                return result;
            }

            //TODO：这里只判断了 Deposit
            if (ExistUnfinishedOrder(config.Domain, config.AppId, args.MemberId, EnumPayOrderType.Deposit))
            {
                result.Message = "存在尚未结束的订单，请先完成当前订单的支付或将其关闭。";
                return result;
            }

            //string outTradeNo =Guid.NewGuid().ToString().Replace("-", "");
            DateTime timeStart = DateTime.Now;
            DateTime timeExpire = DateTime.Now.AddHours(2);

            Guid payOrderId = Guid.NewGuid();

            WeixinPayUnifiedOrderArgs unifiedOrderArgs = new WeixinPayUnifiedOrderArgs();
            unifiedOrderArgs.AppId = config.AppId;
            unifiedOrderArgs.MchId = config.MchId;
            unifiedOrderArgs.DeviceInfo = "WEB";
            unifiedOrderArgs.Body = args.Body; //"会员充值";
            unifiedOrderArgs.OutTradeNo =  args.OutTradeNo; // outTradeNo;
            unifiedOrderArgs.TotalFee = (int)args.Fee;

            unifiedOrderArgs.SpbillCreateIp = args.SpbillCreateIp;
            if (unifiedOrderArgs.SpbillCreateIp == "::1")
            {
                unifiedOrderArgs.SpbillCreateIp = "127.0.0.1";
            }

            unifiedOrderArgs.TimeStart = timeStart.ToString("yyyyMMddHHmmss");
            unifiedOrderArgs.TimeExpire = timeExpire.ToString("yyyyMMddHHmmss");
            unifiedOrderArgs.NotifyUrl = "";
            unifiedOrderArgs.TradeType = "JSAPI";
            unifiedOrderArgs.OpenId = args.OpenId;
            unifiedOrderArgs.NotifyUrl =
                SettingsManager.Instance.GetClientAddress(config.AppId) + "Api/Pay/PayNotify/" + config.Domain;
            unifiedOrderArgs.Attach = payOrderId.ToString();

            WxPayArgs wxPayArgs = config.GetWxPayArgs(false);

            RequestPayApiResult<WeixinPayUnifiedOrderResult> unifiedOrderResult =
                WxPayApi.UnifiedOrder(unifiedOrderArgs, wxPayArgs);

            if (unifiedOrderResult.Success == false)
            {
                _log.Write("UnifiedOrder 失败",
                    unifiedOrderResult.Message + "\r\n"
                    + JsonHelper.Serializer(unifiedOrderArgs) + " "
                    + JsonHelper.Serializer(unifiedOrderResult),
                    TraceEventType.Warning);

                result.Success = false;
                result.Message = unifiedOrderResult.Message;
                return result;
            }

            PayOrderEntity orderEntity = new PayOrderEntity();
            orderEntity.Id = payOrderId;
            orderEntity.Domain = config.Domain;
            orderEntity.Member = args.MemberId;
            orderEntity.Type = args.OrderType;
            orderEntity.AppId = config.AppId;
            orderEntity.MchId = config.MchId;
            orderEntity.DeviceInfo = unifiedOrderArgs.DeviceInfo;
            orderEntity.Body = unifiedOrderArgs.Body;
            orderEntity.OutTradeNo = args.OutTradeNo;//outTradeNo;
            orderEntity.TotalFee = unifiedOrderArgs.TotalFee;
            orderEntity.SpbillCreateIp = unifiedOrderArgs.SpbillCreateIp;
            orderEntity.TimeStart = timeStart;
            orderEntity.TimeExpire = timeExpire;
            orderEntity.TradeType = unifiedOrderArgs.TradeType;
            orderEntity.OpenId = unifiedOrderArgs.OpenId;

            orderEntity.PrepayId = unifiedOrderResult.ApiResult.PrepayId;

            orderEntity.TradeState = EnumPayTradeState.NOTPAY;

            _dataBase.Insert(orderEntity);

            CreatePayOrderResult depositPayResult = new CreatePayOrderResult();
            depositPayResult.PayOrderId = orderEntity.Id;
            depositPayResult.PrepayId = unifiedOrderResult.ApiResult.PrepayId;
            result.Data = depositPayResult;
            result.Success = true;
            return result;
        }

        /// <summary>
        /// 获取现金帐户记录列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetItemListResult GetCashAccountTrackList(GetCashAccountTrackListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@memberId", args.Member));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCashAccountTrackList", 
                parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetCashAccountTrackList(args);
            }

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            int totalCount = int.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            result.TotalCount = totalCount;
            result.TotalPage = totalCount / args.PageSize;
            if (totalCount % args.PageSize > 0)
            {
                result.TotalPage++;
            }
            result.Page = args.Page;

            return result;
        }

        /// <summary>
        /// 通过微信支付充值
        /// </summary>
        /// <param name="args"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public NormalResult<CreatePayOrderResult> Deposit(CashAccountDepositArgs args, AuthorizerPayConfig config)
        {
            CreatePayOrderArgs createPayOrderArgs = new CreatePayOrderArgs();

            createPayOrderArgs.MemberId = args.MemberId;
            createPayOrderArgs.OpenId = args.OpenId;
            createPayOrderArgs.Fee = args.Fee;
            createPayOrderArgs.SpbillCreateIp = args.SpbillCreateIp;

            createPayOrderArgs.OrderType = EnumPayOrderType.Deposit;
            createPayOrderArgs.Body = "会员充值";
            createPayOrderArgs.OutTradeNo = Guid.NewGuid().ToString().Replace("-", "");
            NormalResult<CreatePayOrderResult> result = CreatePayOrder(createPayOrderArgs, config);
            return result;
        }

        /// <summary>
        /// 线下现金充值
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public CashAccountTrackResult CashDeposit(CashAccountCashDepositArgs args)
        {
            CashAccountTrackResult result = new CashAccountTrackResult();

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.Domain));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@memberId", args.MemberId));
            parameterList.Add(new CommandParameter("@fee", args.Fee));
            parameterList.Add(new CommandParameter("@remark", args.Remark));
            parameterList.Add(new CommandParameter("@type", EnumCashAccountTrackType.CashDeposit));
            parameterList.Add(new CommandParameter("@operatorUser", args.OperatorUser));
            parameterList.Add(new CommandParameter("@ip", args.IP));

            DataSet dsResult =
               _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "TrackMemberCash", parameterList,
               new string[] { "result" });

            result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());
            if (result.Reason == 0)
            {
                result.LeftCashAccount = int.Parse(dsResult.Tables[0].Rows[0]["CashAccount"].ToString());
            }
            result.Success = result.Reason == 0;

            return result;
        }

        /// <summary>
        /// 是否存在尚未结束的订单
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="appId"></param>
        /// <param name="memberId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool ExistUnfinishedOrder(Guid domainId, string appId, Guid memberId, EnumPayOrderType type)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@memberId", memberId));
            parameterList.Add(new CommandParameter("@type", type));

            int intStatus = 0;

            _dataBase.ExecuteScalar<int>(
                "SELECT Count(1) FROM [PayOrder] WHERE [Domain] = @domainId AND [AppId] = @appId AND [Member] = @memberId AND [Type] = @type AND ( [TradeState] = 2 OR [TradeState] = 5 )",
                parameterList, (scalarValue) => { intStatus = scalarValue; });

            return intStatus != 0;
        }

        /// <summary>
        /// 微信支付允许创建针对同一个 outTradeNo 的订单
        /// 只是不允许重复 支付 针对同一个 outTradeNo 的订单
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        public List<PayOrderEntity> GetPayOrderByOutTradeNo(string outTradeNo)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@outTradeNo", outTradeNo));

            List<PayOrderEntity> payOrderList = _dataBase.Select<PayOrderEntity>(
                "SELECT * FROM [PayOrder] WHERE [OutTradeNo] = @outTradeNo", parameterList);

            return payOrderList;
        }

        public PayOrderEntity GetPayOrder(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            List<PayOrderEntity> payOrderList = _dataBase.Select<PayOrderEntity>(
                "SELECT * FROM [PayOrder] WHERE [Id] = @id", parameterList);

            if (payOrderList.Count == 0)
                return null;
            else
                return payOrderList[0];
        }

        public GetItemListResult GetPayOrderList(GetPayOrderListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@memberId", args.Member));
            parameterList.Add(new CommandParameter("@tradeState", args.TradeState));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetPayOrderList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetPayOrderList(args);
            }

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            int totalCount = int.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            result.TotalCount = totalCount;
            result.TotalPage = totalCount / args.PageSize;
            if (totalCount % args.PageSize > 0)
            {
                result.TotalPage++;
            }
            result.Page = args.Page;

            return result;
        }

        public NormalResult RefreshPayOrder(string outTradeNo, AuthorizerPayConfig config)
        {
            NormalResult result = new NormalResult();

            if (config == null)
            {
                result.Success = false;
                result.Message = "当前公众号没有微信支付所需配置信息。";
                return result;
            }

            WeixinPayOrderQueryArgs orderQueryArgs = new WeixinPayOrderQueryArgs();
            orderQueryArgs.AppId = config.AppId;
            orderQueryArgs.MchId = config.MchId;
            orderQueryArgs.OutTradeNo = outTradeNo;

            WxPayArgs wxPayArgs = config.GetWxPayArgs(false);

            RequestPayApiResult<WeixinPayOrderQueryResult> orderQueryResult =
               WxPayApi.OrderQuery(orderQueryArgs, wxPayArgs);

            if (orderQueryResult.Success == false)
            {
                _log.Write("OrderQuery 失败",
                   orderQueryResult.Message + "\r\n"
                   + JsonHelper.Serializer(orderQueryArgs) + " "
                   + JsonHelper.Serializer(orderQueryResult),
                   TraceEventType.Warning);

                result.Success = false;
                result.Message = orderQueryResult.Message;
                return result;
            }

            WeixinPayOrderQueryResult orderQuery = orderQueryResult.ApiResult;

            Guid payOrderId;
            if (Guid.TryParse(orderQuery.Attach, out payOrderId) == false)
            {
                string attacth = String.Empty;
                if (orderQuery.Attach != null)
                {
                    attacth = orderQuery.Attach;
                }
                _log.Write("Attach 无法转为本地订单Id" + attacth, orderQuery.OutTradeNo, TraceEventType.Verbose);
                result.Success = false;
                result.Message = "Attach 无法转为本地订单Id。" + attacth;
                return result;
            }

            PayOrderEntity payOrder = GetPayOrder(payOrderId);

            SqlStructureBuild sqlBuild = new SqlStructureBuild();
            sqlBuild.Table = "PayOrder";
            sqlBuild.Type = SqlExpressionType.Update;
            sqlBuild.AddParameter("Id", payOrderId, true);
            sqlBuild.AddParameter("TradeState", EnumHelper.GetEnumFieldByValue<EnumPayTradeState>(orderQuery.TradeState));
            sqlBuild.AddParameter("TradeStateDesc", orderQuery.TradeStateDesc);
            sqlBuild.AddParameter("BankType", orderQuery.BankType);
            sqlBuild.AddParameter("FeeType", orderQuery.FeeType);
            sqlBuild.AddParameter("CouponFee", orderQuery.CouponFee);
            sqlBuild.AddParameter("CouponCount", orderQuery.CouponCount);
            sqlBuild.AddParameter("TransactionId", orderQuery.TransactionId);
            if (orderQuery.TimeEnd != null)
            {
                sqlBuild.AddParameter("TimeEnd", WeixinApiHelper.ConvertStringToDateTime(orderQuery.TimeEnd));
            }
            sqlBuild.AddParameter("Notify_ReturnCode", orderQuery.ReturnCode);
            sqlBuild.AddParameter("Notify_ReturnMsg", orderQuery.ReturnMsg);
            sqlBuild.AddParameter("Notify_ResultCode", orderQuery.ResultCode);
            sqlBuild.AddParameter("Notify_ErrCode", orderQuery.ErrCode);
            sqlBuild.AddParameter("Notify_ErrCodeDes", orderQuery.ErrCodeDes);
            int affectedRowCount = _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());

            if (affectedRowCount == 0)
            {
                _log.Write("本地订单不存在", orderQuery.OutTradeNo, TraceEventType.Warning);
                result.Success = false;
                result.Message = "本地订单不存在。";
                return result;
            }         

            if (orderQuery.CouponCount > 0)
            {
                List<CommandParameter> parameterList = new List<CommandParameter>();
                parameterList.Add(new CommandParameter("@payOrderId", payOrder.Id));

                _dataBase.ExecuteNonQuery(
                    "DELETE FROM [PayOrderCoupon] WHERE [PayOrderId] = @payOrderId", parameterList);

                foreach (WeixinPayOrderQueryResult_Coupon coupon in orderQuery.CouponList)
                {
                    coupon.PayOrderId = payOrder.Id;
                    _dataBase.Insert(coupon);
                }
            }

            result.Success = true;
            return result;
        }

        public NormalResult ClosePayOrder(string outTradeNo, AuthorizerPayConfig config)
        {
            NormalResult result = new NormalResult();

            if (config == null)
            {
                result.Success = false;
                result.Message = "当前公众号没有微信支付所需配置信息。";
                return result;
            }

            WeixinPayCloseOrderArgs closeOrderArgs = new WeixinPayCloseOrderArgs();
            closeOrderArgs.AppId = config.AppId;
            closeOrderArgs.MchId = config.MchId;
            closeOrderArgs.OutTradeNo = outTradeNo;

            WxPayArgs wxPayArgs = config.GetWxPayArgs(false);

            RequestPayApiResult<WeixinPayCloseOrderResult> closeOrderResult =
               WxPayApi.CloseOrder(closeOrderArgs, wxPayArgs);

            if (closeOrderResult.Success == false)
            {
                _log.Write("CloseOrder 失败",
                   closeOrderResult.Message + "\r\n"
                   + JsonHelper.Serializer(closeOrderArgs) + " "
                   + JsonHelper.Serializer(closeOrderResult),
                   TraceEventType.Warning);

                result.Success = false;
                result.Message = closeOrderResult.Message;
                return result;
            }

            result.Success = true;
            return result;
        }

        /// <summary>
        /// 消费
        /// 0:消费成功 1 余额不足 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public CashAccountTrackResult Charge(CashAccountChargeArgs args)
        {
            CashAccountTrackResult result = new CashAccountTrackResult();

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.Domain));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@memberId", args.MemberId));
            parameterList.Add(new CommandParameter("@fee", args.Fee));
            parameterList.Add(new CommandParameter("@remark", args.Remark));
            parameterList.Add(new CommandParameter("@type", EnumCashAccountTrackType.Charge));
            parameterList.Add(new CommandParameter("@operatorUser", args.OperatorUser));
            parameterList.Add(new CommandParameter("@ip", args.IP));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "TrackMemberCash", parameterList,
                new string[] { "result" });

            result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());
            if (result.Reason == 0)
            {
                result.LeftCashAccount = int.Parse(dsResult.Tables[0].Rows[0]["CashAccount"].ToString());
            }
            result.Success = result.Reason == 0;

            return result;
        }

        /// <summary>
        /// 0:成功 1 余额不足 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public NormalResult CashRefund(CashAccountCashRefundArgs args)
        {
            NormalResult normalResult = new NormalResult();

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.Domain));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@memberId", args.Member));
            parameterList.Add(new CommandParameter("@fee", args.Fee));
            parameterList.Add(new CommandParameter("@remark", args.Remark));
            parameterList.Add(new CommandParameter("@operatorUser", args.OperatorUser));
            parameterList.Add(new CommandParameter("@ip", args.IP));
            parameterList.Add(new CommandParameter("@dateTime", args.DateTime));

            DataSet dsResult =
               _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "CashAccountCashRefund", parameterList,
               new string[] { "result" });

            normalResult.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());
            normalResult.Success = normalResult.Reason == 0;

            return normalResult;
        }

        #region 处理支付结果

        private NormalResult ProcessPayNotify(PayOrderEntity payOrder)
        {
            NormalResult result = new NormalResult();

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", payOrder.Domain));
            parameterList.Add(new CommandParameter("@appId", payOrder.AppId));
            parameterList.Add(new CommandParameter("@memberId", payOrder.Member));
            parameterList.Add(new CommandParameter("@fee", payOrder.TotalFee));
            parameterList.Add(new CommandParameter("@remark", DBNull.Value));
            parameterList.Add(new CommandParameter("@type", EnumCashAccountTrackType.Deposit));
            parameterList.Add(new CommandParameter("@operatorUser", DBNull.Value));
            parameterList.Add(new CommandParameter("@ip", DBNull.Value));

            DataSet dsResult = _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "TrackMemberCash",
                parameterList, new string[] { "result" });

            result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());

            if (result.Reason == 0)
            {
                result.Success = true;
            }
            else
            {
                switch (result.Reason)
                {
                    case 1:
                        result.Message = "账户余额不足。";
                        break;
                    case 2:
                        result.Message = "指定的会员不存在。";
                        break;
                    case 3:
                    case 4:
                        result.Message = "无权限。";
                        break;
                    default:
                        result.Message = "未知错误：" + result.Reason;
                        break;
                }
                result.Success = false;
            }

            return result;
        }

        /// <summary>
        /// 账户充值支付结果
        /// </summary>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        private NormalResult ProcessDepositPayNotify(PayOrderEntity payOrder)
        {
            NormalResult result = ProcessPayNotify(payOrder);

            if (result.Success)
            {
                FinishPayOrder(payOrder.Id);
            }

            return result;
        }

        /// <summary>
        /// 处理积分商城支付结果
        /// 先把钱存到账户余额，然后再从余额中下帐成单
        /// 防止订单过期，状态不对等各种原因导致钱丢失
        /// </summary>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        private NormalResult ProcessPointCommodityPayNotify(PayOrderEntity payOrder)
        {
            NormalResult result = ProcessPayNotify(payOrder);

            if (result.Success == false)
            {
                return result;
            }

            result = PointCommodityManager.Instance.ProcessWeixinPayNotify(payOrder);

            if (result.Success)
            {
                FinishPayOrder(payOrder.Id);
            }
           
            return result;
        }

        private NormalResult ProcessDonationPayNotify(PayOrderEntity payOrder)
        {
            FinishPayOrder(payOrder.Id);

            return new NormalResult();
        }

        /// <summary>
        /// 设置订单状态为已处理完毕
        /// </summary>
        /// <param name="payOrderId"></param>
        private void FinishPayOrder(Guid payOrderId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@payOrderId", payOrderId));

            _dataBase.ExecuteNonQuery("UPDATE [PayOrder] SET [Finish] = 1 WHERE [Id] = @payOrderId",
                parameterList);

        }

        /// <summary>
        /// 更新订单中的支付通知接收状态，用行锁定防止并发
        /// true 更新通知状态成功 false 之前已经处理过了
        /// </summary>
        /// <returns></returns>
        private bool UpdateNotifyStatus(Guid payOrderId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", payOrderId));

            DataSet dsResult = _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "UpdatePayNotifyStatus",
                parameterList, new string[] { "result" });

            int result = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());

            return result == 0;
        }

        #endregion
    }
}
