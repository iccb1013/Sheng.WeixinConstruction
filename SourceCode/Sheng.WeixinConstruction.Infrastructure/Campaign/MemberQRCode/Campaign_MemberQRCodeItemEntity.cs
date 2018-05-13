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
    [Table("Campaign_MemberQRCodeItem")]
    public class Campaign_MemberQRCodeItemEntity
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

        [Key]
        public Guid Member
        {
            get;
            set;
        }

        public string QRCodeUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 媒体库Id
        /// 如果为空，表示还没有上传到素材库
        /// </summary>
        public string MediaId
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public int LandingCount
        {
            get;
            set;
        }

        public int LandingPersonCount
        {
            get;
            set;
        }
    }
}
