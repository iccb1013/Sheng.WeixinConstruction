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
    [Table("Campaign")]
    public class CampaignEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Guid Domain
        {
            get;
            set;
        }

        public string AppId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public EnumCampaignType Type
        {
            get;
            set;
        }

        /// <summary>
        /// 简要说明
        /// </summary>
        public string Introduction
        {
            get;
            set;
        }

        /// <summary>
        /// 详细说明
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        public string ShareImageUrl
        {
            get;
            set;
        }

        public string ImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 分享到朋友圈标题
        /// </summary>
        public string ShareTimelineTitle
        {
            get;
            set;
        }

        /// <summary>
        /// 分享给好友标题
        /// </summary>
        public string ShareAppMessageTitle
        {
            get;
            set;
        }

        /// <summary>
        /// 分享给好友描述
        /// </summary>
        public string ShareAppMessageDescription
        {
            get;
            set;
        }

        public EnumCampaignStatus Status
        {
            get;
            set;
        }

        public DateTime? StartTime
        {
            get;
            set;
        }

        public DateTime? EndTime
        {
            get;
            set;
        }

        public bool AutoOngoing
        {
            get;
            set;
        }

        private bool _onlyMember = true;
        public bool OnlyMember
        {
            get { return _onlyMember; }
            set { _onlyMember = value; }
        }

        public string GuideSubscribeUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 分享活动任意页面到朋友圈可得积分
        /// </summary>
        public int ShareTimelinePoint
        {
            get;
            set;
        }

        /// <summary>
        /// 分享活动任意页面给朋友可得积分
        /// </summary>
        public int ShareAppMessagePoint
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public Guid CreateUser
        {
            get;
            set;
        }

        [NotMapped]
        public string CreateUserName
        {
            get;
            set;
        }

        public string Remark
        {
            get;
            set;
        }

        public int MaxParticipant
        {
            get;
            set;
        }
        
        /// <summary>
        /// 活动页面PV
        /// </summary>
        public int PageVisitCount
        {
            get;
            set;
        }
    }
}
