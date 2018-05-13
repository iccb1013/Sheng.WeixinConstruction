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
    /*
     *https://pay.weixin.qq.com/wiki/doc/api/cash_coupon.php?chapter=13_5 
     * 
     */
    public class WeixinPaySendRedpackArgs
    {
        /// <summary>
        /// 商户订单号（每个订单号必须唯一）
        /// 组成：mch_id+yyyymmdd+10位一天内不能重复的数字。
        /// 接口根据商户订单号支持重入，如出现超时可再调用。
        /// mch_billno
        /// </summary>
        public string MchBillno
        {
            get;
            set;
        }

        /// <summary>
        /// 微信支付分配的商户号
        /// mch_id
        /// </summary>
        public string MchId
        {
            get;
            set;
        }

        /// <summary>
        /// 微信分配的公众账号ID（企业号corpid即为此appId）。
        /// 接口传入的所有appid应该为公众号的appid（在mp.weixin.qq.com申请的），
        /// 不能为APP的appid（在open.weixin.qq.com申请的）。
        /// wxappid
        /// </summary>
        public string WxAppId
        {
            get;
            set;
        }

        /// <summary>
        /// 商户名称
        /// 红包发送者名称
        /// send_name
        /// </summary>
        public string SendName
        {
            get;
            set;
        }

        /// <summary>
        /// 用户openid
        /// 接受红包的用户用户在wxappid下的openid
        /// re_openid
        /// </summary>
        public string ReOpenid
        {
            get;
            set;
        }

        /// <summary>
        /// 付款金额，单位分
        /// total_amount
        /// </summary>
        public int TotalAmount
        {
            get;
            set;
        }

        private int _totalNum = 1;
        /// <summary>
        /// 红包发放总人数 total_num=1
        /// total_num
        /// </summary>
        public int TotalNum
        {
            get { return _totalNum; }
            set { _totalNum = value; }
        }

        /// <summary>
        /// 红包祝福语
        /// wishing
        /// </summary>
        public string Wishing
        {
            get;
            set;
        }

        /// <summary>
        /// 调用接口的机器Ip地址
        /// client_ip
        /// </summary>
        public string ClientIp
        {
            get;
            set;
        }

        /// <summary>
        /// 活动名称
        /// act_name
        /// </summary>
        public string ActName
        {
            get;
            set;
        }

        /// <summary>
        /// 备注 ，必填
        /// remark
        /// </summary>
        public string Remark
        {
            get;
            set;
        }
    }
}
