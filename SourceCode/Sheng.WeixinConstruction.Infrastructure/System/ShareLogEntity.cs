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
    [Table("ShareLog")]
    public class ShareLogEntity
    {
        /// <summary>
        /// 只有一个作用，未关注前分享的页面关注后再次分享的判断
        /// </summary>
        public Guid? Member
        {
            get;
            set;
        }

        [Key]
        /// <summary>
        /// 因为存在未关注者分享的情况，所以要用OpenId记录
        /// </summary>
        public string OpenId
        {
            get;
            set;
        }

        [Key]
        /// <summary>
        /// 如果通用页面也要分享记录，为通用页面指定固定的ID
        /// </summary>
        public Guid PageId
        {
            get;
            set;
        }

        /// <summary>
        /// 是否在朋友圈分享了该页面
        /// </summary>
        public bool ShareTimeline
        {
            get;
            set;
        }

        /// <summary>
        /// 是否通过消息分享了该页面
        /// </summary>
        public bool ShareAppMessage
        {
            get;
            set;
        }
    }
}
