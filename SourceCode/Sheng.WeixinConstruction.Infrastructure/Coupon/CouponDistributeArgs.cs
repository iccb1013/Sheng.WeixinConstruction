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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class CouponDistributeArgs
    {
        public Guid DomainId
        {
            get;
            set;
        }

        public string AppId
        {
            get;
            set;
        }

        public Guid CouponId
        {
            get;
            set;
        }

        public Guid MemberId
        {
            get;
            set;
        }

        public Guid DistributeUser
        {
            get;
            set;
        }

        public DateTime? LimitedTime
        {
            get;
            set;
        }

        public string DistributeIP
        {
            get;
            set;
        }

        /// <summary>
        /// 派发张数
        /// </summary>
        public int Count
        {
            get;
            set;
        }
    }
}
