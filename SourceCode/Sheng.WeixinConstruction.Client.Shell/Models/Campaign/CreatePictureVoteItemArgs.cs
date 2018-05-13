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
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class CreatePictureVoteItemArgs
    {
        /// <summary>
        /// 需要下载的图片的服务器端ID，由uploadImage接口获得
        /// </summary>
        public string ImageServerId
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public Guid CampaignId
        {
            get;
            set;
        }
    }
}