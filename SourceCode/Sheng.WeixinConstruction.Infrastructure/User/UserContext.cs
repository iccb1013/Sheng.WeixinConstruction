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
    public class UserContext
    {
        public UserEntity User
        {
            get;
            set;
        }

        //Domain信息不能放在SESSION中，因为会修改关键信息
        //需要考虑同步问题，使用 DomainPool 来实现
        //public Domain Domain
        //{
        //    get;
        //    set;
        //}

        public UserContext(UserEntity user)
        {
            User = user;
        }
    }
}
