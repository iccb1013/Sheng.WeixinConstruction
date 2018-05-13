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
    [Table("Campaign_PictureVote")]
    public class Campaign_PictureVoteEntity
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

        public EnumCampaignPictureVotePublishType PublishType
        {
            get;
            set;
        }

        /// <summary>
        /// 每人最多可发布项目数量
        /// </summary>
        public int MaxPublishTimes
        {
            get;
            set;
        }

        /// <summary>
        /// 每人最多可投票数量
        /// </summary>
        public int MaxVoteTimes
        {
            get;
            set;
        }

        /// <summary>
        /// 参与该活动可得积分，审核通过后得到
        /// </summary>
        public int ApprovedPoint
        {
            get;
            set;
        }

        /// <summary>
        /// 分享活动任意页面，可自动得到票数
        /// </summary>
        public int ShareTimelineVote
        {
            get;
            set;
        }

        /// <summary>
        /// 分享活动任意页面给朋友可得票数
        /// </summary>
        public int ShareAppMessageVote
        {
            get;
            set;
        }

        /// <summary>
        /// 投票方式
        /// </summary>
        public EnumCampaignPictureVoteVoteType VoteType
        {
            get;
            set;
        }

        public bool AllowedNoAttentionVote
        {
            get;
            set;
        }

        private bool _allowedNewItem = true;
        /// <summary>
        /// 关注者参与通道是否开放
        /// </summary>
        public bool AllowedNewItem
        {
            get { return _allowedNewItem; }
            set { _allowedNewItem = value; }
        }
    }
}
