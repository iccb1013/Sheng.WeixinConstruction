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
    [Table("Campaign_LotterySignLog")]
    public class Campaign_LotterySignLogEntity
    {
        [Key]
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

        [Key]
        public Guid Period
        {
            get;
            set;
        }

        [Key]
        public Guid Member
        {
            get;
            set;
        }

        public DateTime SignTime
        {
            get;
            set;
        }

        public bool Win
        {
            get;
            set;
        }
    }
}
