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
    public class GetOneDollarBuyingCommodityParticipatedListArgs : GetItemListArgs
    {
        public Guid Member
        {
            get;
            set;
        }

        /// <summary>
        /// 是否只取幸运记录
        /// </summary>
        public bool Lucky
        {
            get;
            set;
        }
    }
}