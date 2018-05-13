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
    [Table("NewMemberGiftbagSettings")]
    public class NewMemberGiftbagSettingsEntity
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

        private EnumGiftbagPayment _payment = EnumGiftbagPayment.Attention;
        /// <summary>
        /// 领取方式
        /// </summary>
        public EnumGiftbagPayment Payment
        {
            get { return _payment; }
            set { _payment = value; }
        }

        public int Point
        {
            get;
            set;
        }

        /// <summary>
        /// 单位 分
        /// </summary>
        public int Cash
        {
            get;
            set;
        }

        [Json]
        public List<GiftbagCouponWrapper> CouponList
        {
            get;
            set;
        }
    }

}
