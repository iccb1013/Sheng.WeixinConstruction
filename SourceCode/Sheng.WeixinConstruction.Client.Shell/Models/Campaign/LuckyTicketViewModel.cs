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


using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class LuckyTicketViewModel
    {
        public Campaign_LuckyTicketBundle CampaignBundle
        {
            get;
            set;
        }

        public Campaign_LuckyTicketDataReport DataReport
        {
            get;
            set;
        }

        /// <summary>
        /// 当前用户的中奖号码
        /// </summary>
        public List<Campaign_LuckyTicketLogEntity> WinLogList
        {
            get;
            set;
        }

        public WeixinJsApiConfig JsApiConfig
        {
            get;
            set;
        }
    }
}