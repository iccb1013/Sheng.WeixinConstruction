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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Management.Shell.Models
{
    public class MemberInfoViewModel
    {
        public MemberCardLevelEntity MemberCard
        {
            get;
            set;
        }

        public List<MemberCardLevelEntity> MemberCardLevelList
        {
            get;
            set;
        }

        public MemberEntity Member
        {
            get;
            set;
        }
    }
}