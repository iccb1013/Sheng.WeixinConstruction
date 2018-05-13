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
    public class GetCampaignListArgs : GetItemListArgs
    {
        public EnumCampaignStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 活动类型，留空取所有的
        /// </summary>
        public EnumCampaignType? Type
        {
            get;
            set;
        }

        public const string DefaultOrderBy = "StartTime";
        private string _orderBy = DefaultOrderBy;
        public string OrderBy
        {
            get { return _orderBy; }
            set { _orderBy = value; }
        }

        private EnumSort _sort = EnumSort.DESC;
        public EnumSort Sort
        {
            get { return _sort; }
            set { _sort = value; }
        }
    }
}
