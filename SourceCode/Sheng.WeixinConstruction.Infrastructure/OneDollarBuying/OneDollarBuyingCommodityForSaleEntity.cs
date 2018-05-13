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


using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    /// <summary>
    /// 1元抢购商品在售信息
    /// </summary>
    [Table("OneDollarBuyingCommodityForSale")]
    public class OneDollarBuyingCommodityForSaleEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Guid Domain
        {
            get;
            set;
        }

        public string AppId
        {
            get;
            set;
        }

        public Guid CommodityId
        {
            get;
            set;
        }

        /// <summary>
        /// 期号
        /// </summary>
        public string PeriodNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 总份数（价格）
        /// </summary>
        public int TotalPart
        {
            get;
            set;
        }

        /// <summary>
        /// 已销售份数
        /// </summary>
        public int SoldPart
        {
            get;
            set;
        }

        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime ForSaleTime
        {
            get;
            set;
        }

        /// <summary>
        /// 售罄时间
        /// </summary>
        public DateTime? SoldOutTime
        {
            get;
            set;
        }

        /// <summary>
        /// 中奖号码
        /// </summary>
        public string LuckyPartNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 中奖人
        /// </summary>
        public Guid? LuckyMember
        {
            get;
            set;
        }

        /// <summary>
        /// 中奖人参与人次
        /// </summary>
        public int LuckyMemberPartNumberCount
        {
            get;
            set;
        }

        public DateTime? DrawTime
        {
            get;
            set;
        }

        /// <summary>
        /// 是否已领取
        /// </summary>
        public bool Deal
        {
            get;
            set;
        }

        /// <summary>
        /// 领取时间
        /// </summary>
        public DateTime? DealTime
        {
            get;
            set;
        }

        public Guid DealUser
        {
            get;
            set;
        }
    }
}
