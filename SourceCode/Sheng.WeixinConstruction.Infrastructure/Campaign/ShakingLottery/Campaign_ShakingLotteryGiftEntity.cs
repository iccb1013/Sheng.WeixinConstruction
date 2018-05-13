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
    [Table("Campaign_ShakingLotteryGift")]
    public class Campaign_ShakingLotteryGiftEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Guid CampaignId
        {
            get;
            set;
        }

        public Guid Domain
        {
            get;
            set;
        }

        public Guid? Period
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string ImageUrl
        {
            get;
            set;
        }

        public int Probability
        {
            get;
            set;
        }

        /// <summary>
        /// 库存
        /// </summary>
        public int Stock
        {
            get;
            set;
        }

        public bool IsGift
        {
            get;
            set;
        }
    }
}
