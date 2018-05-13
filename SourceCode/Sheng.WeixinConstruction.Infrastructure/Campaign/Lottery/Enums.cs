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
    public enum EnumCampaign_LotteryMode
    {
        Auto = 0,
        Manual = 1
    }

    public enum EnumLotterySignResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        Successful = 0,
        /// <summary>
        /// 失败
        /// </summary>
        Failed = 1,
        /// <summary>
        /// 活动或周期不在进行中
        /// </summary>
        NotOngoing = 2,
        /// <summary>
        /// 已经报过名了
        /// </summary>
        AlreadyLogged = 3,
        /// <summary>
        /// 活动或者周期不存在
        /// </summary>
        NotExist = 4,
        /// <summary>
        /// 积分不足
        /// </summary>
        NotEnoughPoint = 5,
        /// <summary>
        /// 达到参与者数量限制
        /// </summary>
        MaxParticipantLimited = 6
    }
}
