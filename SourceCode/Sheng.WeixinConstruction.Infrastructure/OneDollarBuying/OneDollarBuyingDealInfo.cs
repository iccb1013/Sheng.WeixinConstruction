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
    /// 1元抢购的领取信息
    /// </summary>
    [Table("OneDollarBuyingCommodityForSale")]
    public class OneDollarBuyingDealInfo
    {
        [Key]
        [Column("Id")]
        public Guid SaleId
        {
            get;
            set;
        }

        public Guid CommodityId
        {
            get;
            set;
        }

        public string PeriodNumber
        {
            get;
            set;
        }

        public bool Deal
        {
            get;
            set;
        }

        public DateTime ForSaleTime
        {
            get;
            set;
        }

        public DateTime? DealTime
        {
            get;
            set;
        }

        public DateTime? DrawTime
        {
            get;
            set;
        }

        public string LuckyPartNumber
        {
            get;
            set;
        }

        [Column("LuckyMember")]
        public Guid? LuckyMemberId
        {
            get;
            set;
        }

        [NotMapped]
        public MemberEntity LuckyMember
        {
            get;
            set;
        }

        [NotMapped]
        public string CommodityName
        {
            get;
            set;
        }

    }
}
