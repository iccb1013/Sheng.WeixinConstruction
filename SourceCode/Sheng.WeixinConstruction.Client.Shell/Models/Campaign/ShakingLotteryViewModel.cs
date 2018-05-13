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


using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class ShakingLotteryViewModel
    {
        public Campaign_ShakingLotteryBundle CampaignBundle
        {
            get;
            set;
        }

        public Campaign_ShakingLotteryDataReport DataReport
        {
            get;
            set;
        }

        /// <summary>
        /// 是否参与过
        /// </summary>
        public int PlayedTimes
        {
            get;
            set;
        }

        /// <summary>
        /// 当前用户的中奖
        /// </summary>
        public List<Campaign_ShakingLotteryGiftEntity> GiftList
        {
            get;
            set;
        }

        /// <summary>
        /// 周期模式时，当前周期
        /// </summary>
        public Campaign_ShakingLotteryPeriodEntity CurrentPeriod
        {
            get;
            set;
        }

    }
}