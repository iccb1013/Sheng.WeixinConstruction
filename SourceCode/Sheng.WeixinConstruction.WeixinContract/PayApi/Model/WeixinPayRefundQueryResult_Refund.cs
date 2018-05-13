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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract.PayApi
{
    public class WeixinPayRefundQueryResult_Refund
    {
        /// <summary>
        /// 商户退款单号
        /// out_refund_no_$n
        /// </summary>
        public string OutRefundNo
        {
            get;
            set;
        }

        /// <summary>
        /// 微信退款单号
        /// refund_id_$n
        /// </summary>
        public string RefundId
        {
            get;
            set;
        }

        /// <summary>
        /// 退款渠道
        /// ORIGINAL—原路退款
        /// BALANCE—退回到余额
        /// refund_channel_$n
        /// </summary>
        public string RefundChannel
        {
            get;
            set;
        }

        /// <summary>
        /// 退款金额
        /// 退款总金额,单位为分,可以做部分退款
        /// refund_fee_$n
        /// </summary>
        public int RefundFee
        {
            get;
            set;
        }

        /// <summary>
        /// 代金券或立减优惠退款金额
        /// 代金券或立减优惠退款金额<=退款金额，退款金额-代金券或立减优惠退款金额为现金，说明详见代金券或立减优惠
        /// coupon_refund_fee_$n
        /// </summary>
        public int CouponRefundFee
        {
            get;
            set;
        }

        /// <summary>
        /// 代金券或立减优惠使用数量
        /// 代金券或立减优惠使用数量 ,$n为下标,从0开始编号
        /// coupon_refund_count_$n
        /// </summary>
        public int CouponRefundCount
        {
            get;
            set;
        }

        /*
         * 代金券或立减优惠批次ID	coupon_refund_batch_id_$n_$m	否	String(20)	100	批次ID ,$n为下标，$m为下标，从0开始编号
         * 代金券或立减优惠ID	coupon_refund_id_$n_$m	否	String(20)	10000 	代金券或立减优惠ID, $n为下标，$m为下标，从0开始编号
         * 单个代金券或立减优惠支付金额	coupon_refund_fee_$n_$m	否	Int	100	单个代金券或立减优惠支付金额, $n为下标，$m为下标，从0开始编号
         */

        public List<WeixinPayOrderQueryResult_Coupon> CouponList
        {
            get;
            set;
        }

        /// <summary>
        /// 退款状态
        /// 退款状态：
        /// SUCCESS—退款成功
        /// FAIL—退款失败
        /// PROCESSING—退款处理中
        /// NOTSURE—未确定，需要商户原退款单号重新发起
        /// CHANGE—转入代发，退款到银行发现用户的卡作废或者冻结了，导致原路退款银行卡失败，资金回流到商户的现金帐号，
        /// 需要商户人工干预，通过线下或者财付通转账的方式进行退款。
        /// refund_status_$n
        /// </summary>
        public string RefundStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 退款入账账户
        /// 取当前退款单的退款入账方
        /// 1）退回银行卡：
        /// {银行名称}{卡类型}{卡尾号}
        /// 2）退回支付用户零钱:
        /// 支付用户零钱
        /// refund_recv_accout_$n
        /// </summary>
        public string RefundRecvAccout
        {
            get;
            set;
        }
    }
}
