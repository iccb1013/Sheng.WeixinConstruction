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
    [Table("Campaign_ShakingLotteryPeriod")]
    public class Campaign_ShakingLotteryPeriodEntity
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

        public string Name
        {
            get;
            set;
        }

        private DateTime _createTime = DateTime.Now;
        [OrderBy(OrderBy = OrderBy.ASC)]
        public DateTime CreateTime
        {
            get { return _createTime;  }
            set { _createTime = value; }
        }
    }
}
