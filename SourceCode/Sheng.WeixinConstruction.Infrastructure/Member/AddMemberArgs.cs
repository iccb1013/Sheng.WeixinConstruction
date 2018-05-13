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


using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class AddMemberArgs
    {
        public WeixinUser WeixinUser
        {
            get;
            set;
        }

        public Guid? ScenicQRCodeId
        {
            get;
            set;
        }

        /// <summary>
        /// 如果是通过RecomendUrl关注的
        /// 这里存储上级会员Id
        /// </summary>
        public Guid? RefereeMemberId
        {
            get;
            set;
        }

        public DateTime? SyncFlag
        {
            get;
            set;
        }
    }
}
