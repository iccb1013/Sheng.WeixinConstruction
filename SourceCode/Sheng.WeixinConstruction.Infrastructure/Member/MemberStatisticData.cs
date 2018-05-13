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
    public class MemberStatisticData
    {
        /// <summary>
        /// 关注人数
        /// </summary>
        public int SubscribeCount
        {
            get;
            set;
        }

        /// <summary>
        /// 取消关注人数
        /// </summary>
        public int UnsubscribeCount
        {
            get;
            set;
        }

        /// <summary>
        /// 净增人数
        /// </summary>
        public int NewAttentionCount
        {
            get;
            set;
        }

        /// <summary>
        /// 总关注人数
        /// </summary>
        public int TotalAttentionCount
        {
            get;
            set;
        }

        /// <summary>
        /// 今日签到人数
        /// </summary>
        public int TodaySigninCount
        {
            get;
            set;
        }
    }
}
