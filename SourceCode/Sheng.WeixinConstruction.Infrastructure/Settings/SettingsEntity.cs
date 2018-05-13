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
    /// <summary>
    /// 设置需要独立保存，这样才能兼容独立运行模式和第三方平台运营模式
    /// </summary>
    [Table("Settings")]
    public class SettingsEntity
    {
        [Key]
        public Guid Domain
        {
            get;
            set;
        }

        [Key]
        public string AppId
        {
            get;
            set;
        }

        ///// <summary>
        ///// 会员卡图片
        ///// </summary>
        //public string MemberCardImageUrl
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 会员特权页面URL
        ///// </summary>
        //public string MemberPrivilegeUrl
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 默认会员卡级别
        /// </summary>
        public Guid? DefaultMemberCardLevel
        {
            get;
            set;
        }

        private int _initialMemberPoint = 10;
        public int InitialMemberPoint
        {
            get { return _initialMemberPoint; }
            set { _initialMemberPoint = value; }
        }

        private int _signInPoint = 1;
        public int SignInPoint
        {
            get { return _signInPoint; }
            set { _signInPoint = value; }
        }

        public string PointCommodityGetWay
        {
            get;
            set;
        }

        public string GuideSubscribeUrl
        {
            get;
            set;
        }
    }
}
