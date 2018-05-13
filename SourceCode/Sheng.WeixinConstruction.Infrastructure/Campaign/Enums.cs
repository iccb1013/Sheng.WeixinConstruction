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
    /// 开始活动结果
    /// </summary>
    public enum EnumCampaignStartResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        Successful = 0,
        /// <summary>
        /// 已经开始过了
        /// </summary>
        AlreadyStarted = 1,
        /// <summary>
        /// 已经结束了
        /// </summary>
        AlreadyEnded = 2,
        /// <summary>
        /// 达到免费帐户限制数量
        /// </summary>
        FreeDomainLimited = 3
    }

    /// <summary>
    /// 结束活动结果
    /// </summary>
    public enum EnumCampaignEndResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        Successful = 0,
        /// <summary>
        /// 已经结束过了
        /// </summary>
        AlreadyEnded = 1,
        /// <summary>
        /// 还没开始
        /// </summary>
        NotStarted = 2
    }

    /// <summary>
    /// 活动类型
    /// </summary>
    public enum EnumCampaignType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknow = 0,
        /// <summary>
        /// 最美投票
        /// </summary>
        PictureVote = 1,
        /// <summary>
        /// 粉丝海报
        /// </summary>
        MemberQRCode = 2,
        /// <summary>
        /// 抽奖
        /// </summary>
        Lottery = 3,
        /// <summary>
        /// 号码抽奖
        /// </summary>
        LuckyTicket = 4,
        /// <summary>
        /// 摇一摇抽奖
        /// </summary>
        ShakingLottery = 5,
        /// <summary>
        /// 在线捐款
        /// </summary>
        Donation = 6
    }

    /// <summary>
    /// 活动状态
    /// </summary>
    public enum EnumCampaignStatus
    {
        /// <summary>
        /// 未开始
        /// </summary>
        Preparatory = 0,
        /// <summary>
        /// 进行中
        /// </summary>
        Ongoing = 1,
        /// <summary>
        /// 已结束
        /// </summary>
        End = 2
    }

    #region PictureVote

    /// <summary>
    /// 最美投票发布方式
    /// </summary>
    public enum EnumCampaignPictureVotePublishType
    {
        /// <summary>
        /// 用户发布
        /// </summary>
        MemberPublish = 0,
        /// <summary>
        /// 管理员后台发布
        /// </summary>
        OperatorPublish = 1
    }

    /// <summary>
    /// 投票方式
    /// </summary>
    public enum EnumCampaignPictureVoteVoteType
    {
        /// <summary>
        /// 不允许重复给同一个人投票
        /// </summary>
        NoRepetition = 0,
        /// <summary>
        /// 每天可重复给同一个人投票 
        /// </summary>
        Day = 1
    }

    public enum EnumCampaignPictureVoteResult
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
        /// 已经结束过了
        /// </summary>
        AlreadyEnded = 2,
        /// <summary>
        /// 还没开始
        /// </summary>
        NotStarted = 3,
        /// <summary>
        /// 已经投过票了
        /// </summary>
        Voted = 4,
        /// <summary>
        /// 达到最大投票次数
        /// </summary>
        OverVoteTimes = 5,
        /// <summary>
        /// 只有关注者才可以投票
        /// </summary>
        OnlyMember = 6,
        /// <summary>
        /// 指定的项目已被锁定
        /// </summary>
        Lock = 7,
        /// <summary>
        /// 达到最大投票次数（按天）
        /// </summary>
        OverVoteTimesByDay = 8,
        /// <summary>
        /// 已经投过票了（按天）
        /// </summary>
        VotedByDay = 9
    }

    public enum EnumCampaignCreatePictureVoteItemResult
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
        /// 已经发布过了，达到最大发布次数
        /// </summary>
        AlreadyPublished = 2,
        /// <summary>
        /// 参与通道已经关闭
        /// </summary>
        NewItemClosed = 3

    }

    public enum EnumCampaignPictureVoteItemApproveStatus
    {
        Waiting = 0,
        Approved = 1,
        Rejected = 2
    }

    #endregion
}
