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
    public class MemberContext
    {
        public MemberEntity Member
        {
            get;
            set;
        }

        /// <summary>
        /// 所关联的User（管理员）
        /// </summary>
        public UserEntity User
        {
            get;
            set;
        }

        public MemberContext(MemberEntity member)
        {
            Member = member;
        }
    }
}
