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
    /*
     * 一个域下面有可能存在多个 授权过 的公众 号
     * 但只允许一个是 当前 正在运营的
     */
    [Table("Domain")]
    public class DomainEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public EnumDomainType Type
        {
            get;
            set;
        }

        /// <summary>
        /// 付费有效期
        /// </summary>
        public DateTime? LimitedDate
        {
            get;
            set;
        }

        /// <summary>
        /// 微信支付对接有效期
        /// </summary>
        public DateTime? PayLimitedDate
        {
            get;
            set;
        }
        
        /// <summary>
        /// 最后更新配置时间
        /// 管理端更新，客户端读取并判断要不要重新从数据库中读取信息
        /// </summary>
        public DateTime LastUpdateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 最后一次授权对接的时间
        /// 或最后一次解除授权对接的时间
        /// </summary>
        public DateTime? LastDockingTime
        {
            get;
            set;
        }
    }
}
