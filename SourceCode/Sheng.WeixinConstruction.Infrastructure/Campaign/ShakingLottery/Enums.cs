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

namespace Sheng.WeixinConstruction.Infrastructure
{
    /// <summary>
    /// 抽奖模式
    /// </summary>
    public enum EnumCampaign_ShakingLotteryMode
    {
        /// <summary>
        /// 只可参与一次
        /// </summary>
        Once = 0,
        /// <summary>
        /// 可重复参与
        /// </summary>
        Repeatable = 1,
        /// <summary>
        /// 周期模式
        /// </summary>
        Period = 2
    }
}
