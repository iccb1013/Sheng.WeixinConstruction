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
    public class MemberQRCodeShareViewModel
    {
        public CampaignEntity Campaign
        {
            get;
            set;
        }

        public Campaign_MemberQRCodeItemEntity QRCodeItem
        {
            get;
            set;
        }

        //public WeixinJsApiConfig JsApiConfig
        //{
        //    get;
        //    set;
        //}
    }
}