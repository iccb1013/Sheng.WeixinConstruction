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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    // 前端页面的展示是JS判断的，没有取EnumMember
    //因为数据是以表的形势返回的，不是实对象
    public enum MemberPointTrackType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EnumMember(Value = "未知")]
        Unknow = 0,
        /// <summary>
        /// 积分商城
        /// </summary>
        [EnumMember(Value = "积分商城")]
        PointCommodityOrder = 1,
        /// <summary>
        /// 人工操作
        /// </summary>
        [EnumMember(Value = "人工操作")]
        UserOperate = 2,
        /// <summary>
        /// 每日签到
        /// </summary>
        [EnumMember(Value = "每日签到")]
        SignIn = 3,
        /// <summary>
        /// 注册初始
        /// </summary>
        [EnumMember(Value = "注册初始")]
        Register = 4,
        /// <summary>
        /// 活动奖励
        /// </summary>
        [EnumMember(Value = "活动奖励")]
        Campaign = 5,
        /// <summary>
        /// 分享奖励
        /// </summary>
        [EnumMember(Value = "分享奖励")]
        Share = 6,
        /// <summary>
        /// 推广奖励
        /// </summary>
        [EnumMember(Value = "推广奖励")]
        RecommendUrl = 7
    }

   
}
