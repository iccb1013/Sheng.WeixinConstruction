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
    [Table("Campaign_ShakingLottery")]
    public class Campaign_ShakingLotteryEntity
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

        public int Point
        {
            get;
            set;
        }

        public EnumCampaign_ShakingLotteryMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// 可摇奖次数
        /// </summary>
        public int ChanceTimes
        {
            get;
            set;
        }

        /// <summary>
        /// 是否开始摇一摇了，主持人按下开始按钮
        /// </summary>
        public bool Started
        {
            get;
            set;
        }

        public Guid? Period
        {
            get;
            set;
        }
    }
}
