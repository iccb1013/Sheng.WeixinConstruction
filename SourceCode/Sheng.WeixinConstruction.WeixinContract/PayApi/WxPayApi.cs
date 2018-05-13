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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sheng.WeixinConstruction.WeixinContract.PayApi
{
    public class WxPayApi
    {
        private static readonly HttpService _httpService = HttpService.Instance;

        private static readonly XmlSerializer _sendRedpackResultXmlSerializer =
            new XmlSerializer(typeof(WeixinPaySendRedpackResult));

        private static readonly XmlSerializer _unifiedOrderResultXmlSerializer =
            new XmlSerializer(typeof(WeixinPayUnifiedOrderResult));

        private static readonly XmlSerializer _orderQueryResultXmlSerializer =
            new XmlSerializer(typeof(WeixinPayOrderQueryResult));

        private static readonly XmlSerializer _closeOrderResultXmlSerializer =
            new XmlSerializer(typeof(WeixinPayCloseOrderResult));

        private static readonly XmlSerializer _refundResultXmlSerializer =
            new XmlSerializer(typeof(WeixinPayRefundResult));

        private static readonly XmlSerializer _refundQueryResultXmlSerializer =
            new XmlSerializer(typeof(WeixinPayRefundQueryResult));

        private static readonly XmlSerializer _notifyXmlSerializer =
           new XmlSerializer(typeof(WeixinPayNotify));


        /// <summary>
        /// 发红包
        /// 需要证书
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static RequestPayApiResult<WeixinPaySendRedpackResult> SendRedpack(
            WeixinPaySendRedpackArgs args, WxPayArgs wxPayArgs)
        {
            WxPayData wxPayData = new WxPayData(wxPayArgs.Key);
            wxPayData.SetValue("mch_billno", args.MchBillno);
            wxPayData.SetValue("mch_id", args.MchId);
            wxPayData.SetValue("wxappid", args.WxAppId);
            wxPayData.SetValue("send_name", args.SendName);
            wxPayData.SetValue("re_openid", args.ReOpenid);
            wxPayData.SetValue("total_amount", args.TotalAmount);
            wxPayData.SetValue("total_num", args.TotalNum);
            wxPayData.SetValue("wishing", args.Wishing);
            wxPayData.SetValue("client_ip", args.ClientIp);
            wxPayData.SetValue("act_name", args.ActName);
            wxPayData.SetValue("remark", args.Remark);
            wxPayData.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            wxPayData.SetValue("sign", wxPayData.MakeSign());//签名

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";
            requestArgs.Content = wxPayData.ToXml();

            if (wxPayArgs != null)
            {
                requestArgs.UseCertificate = wxPayArgs.UseCertificate;
                requestArgs.CertificatePath = wxPayArgs.CertificatePath;
                requestArgs.CertificatePassword = wxPayArgs.CertificatePassword;
            }

            RequestPayApiResult<WeixinPaySendRedpackResult> result =
               new RequestPayApiResult<WeixinPaySendRedpackResult>();

            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success == false)
            {
                result.Success = false;
                result.Message = "请求失败。";
                if (result.HttpRequestResult.Exception != null)
                {
                    result.Message += result.HttpRequestResult.Exception.Message;
                }

                return result;
            }

            WxPayData wxPayResultData = new WxPayData(wxPayArgs.Key);
            wxPayResultData.FromXml(result.HttpRequestResult.Content);
            if (wxPayResultData.CheckSign() == false)
            {
                result.Success = false;
                result.Message = "返回数据签名校验失败。";

                return result;
            }

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(result.HttpRequestResult.Content));
            result.ApiResult = _sendRedpackResultXmlSerializer.Deserialize(stream) as WeixinPaySendRedpackResult;
            result.Success = true;
            return result;
        }

        /// <summary>
        /// 统一下单
        /// 不需要证书
        /// 除被扫支付场景以外，商户系统先调用该接口在微信支付服务后台生成预支付交易单，
        /// 返回正确的预支付交易回话标识后再按扫码、JSAPI、APP等不同场景生成交易串调起支付。
        /// https://pay.weixin.qq.com/wiki/doc/api/native.php?chapter=9_1
        /// </summary>
        /// <param name="args"></param>
        /// <param name="wxPayArgs"></param>
        /// <returns></returns>
        public static RequestPayApiResult<WeixinPayUnifiedOrderResult> UnifiedOrder(
            WeixinPayUnifiedOrderArgs args, WxPayArgs wxPayArgs)
        {
            WxPayData wxPayData = new WxPayData(wxPayArgs.Key);
            wxPayData.SetValue("appid", args.AppId);
            wxPayData.SetValue("mch_id", args.MchId);
            wxPayData.SetValue("device_info", args.DeviceInfo);
            wxPayData.SetValue("body", args.Body);
            wxPayData.SetValue("detail", args.Detail);
            wxPayData.SetValue("attach", args.Attach);
            wxPayData.SetValue("out_trade_no", args.OutTradeNo);
            wxPayData.SetValue("fee_type", args.FeeType);
            wxPayData.SetValue("total_fee", args.TotalFee);
            wxPayData.SetValue("spbill_create_ip", args.SpbillCreateIp);
            wxPayData.SetValue("time_start", args.TimeStart);
            wxPayData.SetValue("time_expire", args.TimeExpire);
            wxPayData.SetValue("goods_tag", args.GoodsTag);
            wxPayData.SetValue("notify_url", args.NotifyUrl);
            wxPayData.SetValue("trade_type", args.TradeType);
            wxPayData.SetValue("product_id", args.ProductId);
            wxPayData.SetValue("limit_pay", args.LimitPay);
            wxPayData.SetValue("openid", args.OpenId);
            wxPayData.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            wxPayData.SetValue("sign", wxPayData.MakeSign());//签名

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            requestArgs.Content = wxPayData.ToXml();


            RequestPayApiResult<WeixinPayUnifiedOrderResult> result =
               new RequestPayApiResult<WeixinPayUnifiedOrderResult>();

            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success == false)
            {
                result.Success = false;
                result.Message = "请求失败。";
                if (result.HttpRequestResult.Exception != null)
                {
                    result.Message += result.HttpRequestResult.Exception.Message;
                }

                return result;
            }

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(result.HttpRequestResult.Content));
            WeixinPayUnifiedOrderResult orderResult = 
                _unifiedOrderResultXmlSerializer.Deserialize(stream) as WeixinPayUnifiedOrderResult;

            if (orderResult.ReturnCode == "FAIL")
            {
                result.Success = false;
                result.Message = orderResult.ReturnMsg;
                return result;
            }

            WxPayData wxPayResultData = new WxPayData(wxPayArgs.Key);
            wxPayResultData.FromXml(result.HttpRequestResult.Content);
            if (wxPayResultData.CheckSign() == false)
            {
                result.Success = false;
                result.Message = "返回数据签名校验失败。";

                return result;
            }

            if (orderResult.ResultCode == "FAIL")
            {
                result.Success = false;
                result.Message = orderResult.ErrCode + " " + orderResult.ErrCodeDes;
                return result;
            }

            result.ApiResult = orderResult;
            result.Success = true;
            return result;

        }

        /// <summary>
        /// 查询订单
        /// 该接口提供所有微信支付订单的查询，商户可以通过该接口主动查询订单状态
        /// 不需要证书
        /// </summary>
        /// <param name="args"></param>
        /// <param name="wxPayArgs"></param>
        /// <returns></returns>
        public static RequestPayApiResult<WeixinPayOrderQueryResult> OrderQuery(
            WeixinPayOrderQueryArgs args, WxPayArgs wxPayArgs)
        {
            WxPayData wxPayData = new WxPayData(wxPayArgs.Key);
            wxPayData.SetValue("appid", args.AppId);
            wxPayData.SetValue("mch_id", args.MchId);
            wxPayData.SetValue("transaction_id", args.TransactionId);
            wxPayData.SetValue("out_trade_no", args.OutTradeNo);
            wxPayData.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));
            wxPayData.SetValue("sign", wxPayData.MakeSign());

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = "https://api.mch.weixin.qq.com/pay/orderquery";
            requestArgs.Content = wxPayData.ToXml();

            RequestPayApiResult<WeixinPayOrderQueryResult> result =
               new RequestPayApiResult<WeixinPayOrderQueryResult>();

            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success == false)
            {
                result.Success = false;
                result.Message = "请求失败。";
                if (result.HttpRequestResult.Exception != null)
                {
                    result.Message += result.HttpRequestResult.Exception.Message;
                }

                return result;
            }

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(result.HttpRequestResult.Content));
            WeixinPayOrderQueryResult orderQueryResult =
                _orderQueryResultXmlSerializer.Deserialize(stream) as WeixinPayOrderQueryResult;

            if (orderQueryResult.ReturnCode == "FAIL")
            {
                result.Success = false;
                result.Message = orderQueryResult.ReturnMsg;
                return result;
            }

            WxPayData wxPayResultData = new WxPayData(wxPayArgs.Key);
            wxPayResultData.FromXml(result.HttpRequestResult.Content);
            if (wxPayResultData.CheckSign() == false)
            {
                result.Success = false;
                result.Message = "返回数据签名校验失败。";

                return result;
            }

            if (orderQueryResult.ResultCode == "FAIL")
            {
                result.Success = false;
                result.Message = orderQueryResult.ErrCode + " " + orderQueryResult.ErrCodeDes;
                return result;
            }

            if (orderQueryResult.CouponCount > 0)
            {
                orderQueryResult.CouponList = new List<WeixinPayOrderQueryResult_Coupon>();
                for (int i = 0; i < orderQueryResult.CouponCount; i++)
                {
                    WeixinPayOrderQueryResult_Coupon coupon = new WeixinPayOrderQueryResult_Coupon();

                    object couponBatchId = wxPayResultData.GetValue("coupon_batch_id_" + i.ToString());
                    if (couponBatchId != null)
                        coupon.CouponBatchId = couponBatchId.ToString();

                    object couponType = wxPayResultData.GetValue("coupon_type_" + i.ToString());
                    if (couponType != null)
                        coupon.CouponType = couponType.ToString();

                    object couponId = wxPayResultData.GetValue("coupon_id_" + i.ToString());
                    if (couponId != null)
                        coupon.CouponId = couponId.ToString();

                    object couponFee = wxPayResultData.GetValue("coupon_fee_" + i.ToString());
                    if (couponFee != null)
                        coupon.CouponFee = Int32.Parse(couponFee.ToString());

                    orderQueryResult.CouponList.Add(coupon);
                }
            }

            result.ApiResult = orderQueryResult;
            result.Success = true;
            return result;

        }

        /// <summary>
        /// 关闭订单
        /// 不需要 证书
        /// https://pay.weixin.qq.com/wiki/doc/api/native.php?chapter=9_3
        /// </summary>
        /// <param name="args"></param>
        /// <param name="wxPayArgs"></param>
        /// <returns></returns>
        public static RequestPayApiResult<WeixinPayCloseOrderResult> CloseOrder(
            WeixinPayCloseOrderArgs args, WxPayArgs wxPayArgs)
        {
            WxPayData wxPayData = new WxPayData(wxPayArgs.Key);
            wxPayData.SetValue("appid", args.AppId);
            wxPayData.SetValue("mch_id", args.MchId);
            wxPayData.SetValue("out_trade_no", args.OutTradeNo);
            wxPayData.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));
            wxPayData.SetValue("sign", wxPayData.MakeSign());

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = "https://api.mch.weixin.qq.com/pay/closeorder";
            requestArgs.Content = wxPayData.ToXml();

            RequestPayApiResult<WeixinPayCloseOrderResult> result =
               new RequestPayApiResult<WeixinPayCloseOrderResult>();

            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success == false)
            {
                result.Success = false;
                result.Message = "请求失败。";
                if (result.HttpRequestResult.Exception != null)
                {
                    result.Message += result.HttpRequestResult.Exception.Message;
                }

                return result;
            }

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(result.HttpRequestResult.Content));
            WeixinPayCloseOrderResult closeOrderResult = 
                _closeOrderResultXmlSerializer.Deserialize(stream) as WeixinPayCloseOrderResult;

            if (closeOrderResult.ReturnCode == "FAIL")
            {
                result.Success = false;
                result.Message = closeOrderResult.ReturnMsg;
                return result;
            }

            WxPayData wxPayResultData = new WxPayData(wxPayArgs.Key);
            wxPayResultData.FromXml(result.HttpRequestResult.Content);
            if (wxPayResultData.CheckSign() == false)
            {
                result.Success = false;
                result.Message = "返回数据签名校验失败。";

                return result;
            }

            if (closeOrderResult.ResultCode == "FAIL")
            {
                result.Success = false;
                result.Message = closeOrderResult.ErrCode + " " + closeOrderResult.ErrCodeDes;
                return result;
            }

            result.ApiResult = closeOrderResult;
            result.Success = true;
            return result;
        }

        /// <summary>
        /// 申请退款
        /// 请求需要双向证书。 详见证书使用
        /// https://api.mch.weixin.qq.com/secapi/pay/refund
        /// </summary>
        public static RequestPayApiResult<WeixinPayRefundResult> Refund(
            WeixinPayRefundArgs args, WxPayArgs wxPayArgs)
        {
            WxPayData wxPayData = new WxPayData(wxPayArgs.Key);
            wxPayData.SetValue("appid", args.AppId);
            wxPayData.SetValue("mch_id", args.MchId);
            wxPayData.SetValue("device_info", args.DeviceInfo);
            wxPayData.SetValue("transaction_id", args.TransactionId);
            wxPayData.SetValue("out_trade_no", args.OutTradeNo);
            wxPayData.SetValue("out_refund_no", args.OutRefundNo);
            wxPayData.SetValue("total_fee", args.TotalFee);
            wxPayData.SetValue("refund_fee", args.RefundFee);
            wxPayData.SetValue("refund_fee_type", args.RefundFeeType);
            wxPayData.SetValue("op_user_id", args.OpUserId);
            wxPayData.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));
            wxPayData.SetValue("sign", wxPayData.MakeSign());

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
            requestArgs.Content = wxPayData.ToXml();

            if (wxPayArgs != null)
            {
                requestArgs.UseCertificate = wxPayArgs.UseCertificate;
                requestArgs.CertificatePath = wxPayArgs.CertificatePath;
                requestArgs.CertificatePassword = wxPayArgs.CertificatePassword;
            }

            RequestPayApiResult<WeixinPayRefundResult> result =
               new RequestPayApiResult<WeixinPayRefundResult>();

            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success == false)
            {
                result.Success = false;
                result.Message = "请求失败。";
                if (result.HttpRequestResult.Exception != null)
                {
                    result.Message += result.HttpRequestResult.Exception.Message;
                }

                return result;
            }
          
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(result.HttpRequestResult.Content));
            WeixinPayRefundResult refundResult = _refundResultXmlSerializer.Deserialize(stream) as WeixinPayRefundResult;

            if (refundResult.ReturnCode == "FAIL")
            {
                result.Success = false;
                result.Message = refundResult.ReturnMsg;
                return result;
            }

            WxPayData wxPayResultData = new WxPayData(wxPayArgs.Key);
            wxPayResultData.FromXml(result.HttpRequestResult.Content);
            if (wxPayResultData.CheckSign() == false)
            {
                result.Success = false;
                result.Message = "返回数据签名校验失败。";

                return result;
            }

            if (refundResult.ResultCode == "FAIL")
            {
                result.Success = false;
                result.Message = refundResult.ErrCode + " " + refundResult.ErrCodeDes;
                return result;
            }

            result.ApiResult = refundResult;
            result.Success = true;
            return result;
        }

        /// <summary>
        /// 查询退款
        /// 不需要 证书
        /// 提交退款申请后，通过调用该接口查询退款状态。
        /// 退款有一定延时，用零钱支付的退款20分钟内到账，银行卡支付的退款3个工作日后重新查询退款状态。
        /// https://pay.weixin.qq.com/wiki/doc/api/native.php?chapter=9_5
        /// </summary>
        /// <param name="args"></param>
        /// <param name="wxPayArgs"></param>
        /// <returns></returns>
        public static RequestPayApiResult<WeixinPayRefundQueryResult> RefundQuery(
            WeixinPayRefundQueryArgs args, WxPayArgs wxPayArgs)
        {
            WxPayData wxPayData = new WxPayData(wxPayArgs.Key);
            wxPayData.SetValue("appid", args.AppId);
            wxPayData.SetValue("mch_id", args.MchId);
            wxPayData.SetValue("device_info", args.DeviceInfo);
            wxPayData.SetValue("transaction_id", args.TransactionId);
            wxPayData.SetValue("out_trade_no", args.OutTradeNo);
            wxPayData.SetValue("out_refund_no", args.OutRefundNo);
            wxPayData.SetValue("refund_id", args.RefundId);
            wxPayData.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));
            wxPayData.SetValue("sign", wxPayData.MakeSign());

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = "https://api.mch.weixin.qq.com/pay/refundquery";
            requestArgs.Content = wxPayData.ToXml();

            RequestPayApiResult<WeixinPayRefundQueryResult> result =
               new RequestPayApiResult<WeixinPayRefundQueryResult>();

            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Success == false)
            {
                result.Success = false;
                result.Message = "请求失败。";
                if (result.HttpRequestResult.Exception != null)
                {
                    result.Message += result.HttpRequestResult.Exception.Message;
                }

                return result;
            }

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(result.HttpRequestResult.Content));
            WeixinPayRefundQueryResult refundQueryResult =
                _refundQueryResultXmlSerializer.Deserialize(stream) as WeixinPayRefundQueryResult;

            if (refundQueryResult.ReturnCode == "FAIL")
            {
                result.Success = false;
                result.Message = refundQueryResult.ReturnMsg;
                return result;
            }

            WxPayData wxPayResultData = new WxPayData(wxPayArgs.Key);
            wxPayResultData.FromXml(result.HttpRequestResult.Content);
            if (wxPayResultData.CheckSign() == false)
            {
                result.Success = false;
                result.Message = "返回数据签名校验失败。";

                return result;
            }

            if (refundQueryResult.ResultCode == "FAIL")
            {
                result.Success = false;
                result.Message = refundQueryResult.ErrCode + " " + refundQueryResult.ErrCodeDes;
                return result;
            }

            //退款笔数
            if (refundQueryResult.RefundCount > 0)
            {
                refundQueryResult.RefundList = new List<WeixinPayRefundQueryResult_Refund>();
                for (int i = 0; i < refundQueryResult.RefundCount; i++)
                {
                    WeixinPayRefundQueryResult_Refund refund = new WeixinPayRefundQueryResult_Refund();

                    object outRefundNo = wxPayResultData.GetValue("out_refund_no_" + i.ToString());
                    if (outRefundNo != null)
                        refund.OutRefundNo = outRefundNo.ToString();

                    object refundId = wxPayResultData.GetValue("refund_id_" + i.ToString());
                    if (refundId != null)
                        refund.RefundId = refundId.ToString();

                    object refundChannel = wxPayResultData.GetValue("refund_channel_" + i.ToString());
                    if (refundChannel != null)
                        refund.RefundChannel = refundChannel.ToString();

                    object refundFee = wxPayResultData.GetValue("refund_fee_" + i.ToString());
                    if (refundFee != null)
                        refund.RefundFee = Int32.Parse(refundFee.ToString());

                    object couponRefundFee = wxPayResultData.GetValue("coupon_refund_fee_" + i.ToString());
                    if (couponRefundFee != null)
                        refund.CouponRefundFee = Int32.Parse(couponRefundFee.ToString());

                    object refundStatus = wxPayResultData.GetValue("refund_status_" + i.ToString());
                    if (refundStatus != null)
                        refund.RefundStatus = refundStatus.ToString();

                    object refundRecvAccout = wxPayResultData.GetValue("refund_recv_accout_" + i.ToString());
                    if (refundRecvAccout != null)
                        refund.RefundRecvAccout = refundRecvAccout.ToString();

                    //代金券或立减优惠使用数量
                    object couponRefundCount = wxPayResultData.GetValue("coupon_refund_count_" + i.ToString());
                    if (couponRefundCount != null)
                        refund.CouponRefundCount = Int32.Parse(couponRefundCount.ToString());

                    if (refund.CouponRefundCount > 0)
                    {
                        refund.CouponList = new List<WeixinPayOrderQueryResult_Coupon>();
                        for (int j = 0; j < refund.CouponRefundCount; j++)
                        {
                            WeixinPayOrderQueryResult_Coupon coupon = new WeixinPayOrderQueryResult_Coupon();

                            object couponBatchId = wxPayResultData.GetValue("coupon_refund_batch_id_" + i.ToString() + "_" + j.ToString());
                            if (couponBatchId != null)
                                coupon.CouponBatchId = couponBatchId.ToString();

                            object couponId = wxPayResultData.GetValue("coupon_refund_id_" + i.ToString() + "_" + j.ToString());
                            if (couponId != null)
                                coupon.CouponId = couponId.ToString();

                            object couponFee = wxPayResultData.GetValue("coupon_refund_fee_" + i.ToString() + "_" + j.ToString());
                            if (couponFee != null)
                                coupon.CouponFee = Int32.Parse(couponFee.ToString());

                            refund.CouponList.Add(coupon);
                        }
                    }

                    refundQueryResult.RefundList.Add(refund);
                }
            }

            result.ApiResult = refundQueryResult;
            result.Success = true;
            return result;
        }

        public static RequestPayApiResult<WeixinPayNotify> Notify(string xml, WxPayArgs wxPayArgs)
        {
            RequestPayApiResult<WeixinPayNotify> result =
               new RequestPayApiResult<WeixinPayNotify>();

            WxPayData wxPayResultData = new WxPayData(wxPayArgs.Key);
            wxPayResultData.FromXml(xml);
            if (wxPayResultData.CheckSign() == false)
            {
                result.Success = false;
                result.Message = "返回数据签名校验失败。";

                return result;
            }

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            WeixinPayNotify orderQueryResult =
                _notifyXmlSerializer.Deserialize(stream) as WeixinPayNotify;

            if (orderQueryResult.ReturnCode == "FAIL")
            {
                result.Success = false;
                result.Message = orderQueryResult.ReturnMsg;
                return result;
            }

            if (orderQueryResult.ResultCode == "FAIL")
            {
                result.Success = false;
                result.Message = orderQueryResult.ErrCode + " " + orderQueryResult.ErrCodeDes;
                return result;
            }

            if (orderQueryResult.CouponCount > 0)
            {
                orderQueryResult.CouponList = new List<WeixinPayNotify_Coupon>();
                for (int i = 0; i < orderQueryResult.CouponCount; i++)
                {
                    WeixinPayNotify_Coupon coupon = new WeixinPayNotify_Coupon();

                    object couponId = wxPayResultData.GetValue("coupon_id_" + i.ToString());
                    if (couponId != null)
                        coupon.CouponId = couponId.ToString();

                    object couponFee = wxPayResultData.GetValue("coupon_fee_" + i.ToString());
                    if (couponFee != null)
                        coupon.CouponFee = Int32.Parse(couponFee.ToString());

                    orderQueryResult.CouponList.Add(coupon);
                }
            }

            result.ApiResult = orderQueryResult;
            result.Success = true;
            return result;
        }
    }
}
