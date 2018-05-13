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
    public class Campaign_PictureVoteArgs
    {
        public Guid DomainId
        {
            get;
            set;
        }

        public Guid CampaignId
        {
            get;
            set;
        }

        public Guid ItemId
        {
            get;
            set;
        }

        public string OpenId
        {
            get;
            set;
        }

        public Guid? Member
        {
            get;
            set;
        }

        ///// <summary>
        ///// 如果Member为null
        ///// 则表示投票者没有关注公众号
        ///// </summary>
        //public MemberEntity Member
        //{
        //    get;
        //    set;
        //}
    }
}
