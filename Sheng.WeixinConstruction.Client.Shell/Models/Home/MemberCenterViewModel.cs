using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class MemberCenterViewModel
    {
        /// <summary>
        /// 所属会员卡
        /// 可能为null
        /// </summary>
        public MemberCardLevelEntity MemberCard
        {
            get;
            set;
        }

        ///// <summary>
        ///// 有效卡券数量
        ///// </summary>
        //public int ValidCouponCount
        //{
        //    get;
        //    set;
        //}
    }
}