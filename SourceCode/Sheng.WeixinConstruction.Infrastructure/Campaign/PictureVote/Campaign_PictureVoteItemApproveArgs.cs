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
    public class Campaign_PictureVoteItemApproveArgs
    {
        public Guid CampaignId
        {
            get;
            set;
        }

        public Guid? MemberId
        {
            get;
            set;
        }

        public Guid ItemId
        {
            get;
            set;
        }

        //public EnumCampaignPictureVoteItemApproveStatus ApproveStatus
        //{
        //    get;
        //    set;
        //}

        //public string Message
        //{
        //    get;
        //    set;
        //}
    }
}
