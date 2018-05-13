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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetCampaign_MemberQRCodeItemListArgs : GetItemListArgs
    {
        public Guid CampaignId
        {
            get;
            set;
        }

        public string MemberName
        {
            get;
            set;
        }

        public const string DefaultOrderBy = "CreateTime";
        private string _orderBy = DefaultOrderBy;
        public string OrderBy
        {
            get { return _orderBy; }
            set { _orderBy = value; }
        }
    }

   

}
