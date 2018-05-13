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
    [Table("Campaign_MemberQRCode")]
    public class Campaign_MemberQRCodeEntity
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

        public string BackgroundImageUrl
        {
            get;
            set;
        }

        public Guid BackgroundImageId
        {
            get;
            set;
        }

        /// <summary>
        /// 落地页不直接生成到二维码中，二维码指向的跳转URL只带活动ID和会员ID
        /// 扫码时根据活动ID查找落地页面地址
        /// 以便后台随时调整落地页
        /// </summary>
        public string LandingUrl
        {
            get;
            set;
        }

        public int LandingPoint
        {
            get;
            set;
        }
    }
}
