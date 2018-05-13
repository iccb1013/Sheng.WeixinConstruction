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
    /// <summary>
    /// 对 RoleAuthorizationEntity 的集合，同一个 Role 下所有的 权限集合
    /// </summary>
    public class RoleAuthorizationRelation
    {
        public Guid Domain
        {
            get;
            set;
        }

        public Guid Role
        {
            get;
            set;
        }

        public List<Authorization> AuthorizationList
        {
            get;
            set;
        }
    }
}
