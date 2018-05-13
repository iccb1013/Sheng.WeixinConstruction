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
    [Table("Campaign_PictureVoteItem")]
    public class Campaign_PictureVoteItemEntity
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

        public Guid CampaignId
        {
            get;
            set;
        }

        public Guid? Member
        {
            get;
            set;
        }

        public Guid? User
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        public DateTime UploadTime
        {
            get;
            set;
        }

        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNumber
        {
            get;
            set;
        }

        public EnumCampaignPictureVoteItemApproveStatus ApproveStatus
        {
            get;
            set;
        }

        [NotMapped]
        public string ApproveStatusString
        {
            get
            {
                switch (ApproveStatus)
                {
                    case EnumCampaignPictureVoteItemApproveStatus.Waiting:
                        return "未审核";
                    case EnumCampaignPictureVoteItemApproveStatus.Approved:
                        return "审核通过";
                    case EnumCampaignPictureVoteItemApproveStatus.Rejected:
                        return "已拒绝";
                    default:
                        return "未知";
                }
            }
        }

        public int VoteQuantity
        {
            get;
            set;
        }

        [NotMapped]
        public MemberEntity MemberEntity
        {
            get;
            set;
        }

        public bool Lock
        {
            get;
            set;
        }

        public string RejectedMessage
        {
            get;
            set;
        }
    }
}
