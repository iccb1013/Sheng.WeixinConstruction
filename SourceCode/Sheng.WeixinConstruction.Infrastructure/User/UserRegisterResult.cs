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
    public class UserRegisterResult
    {
        public UserRegisterResultEnum Result
        {
            get;
            set;
        }

        public UserEntity User
        {
            get;
            set;
        }

        public DomainEntity Domain
        {
            get;
            set;
        }
    }

    public enum UserRegisterResultEnum
    {
        Unknow,
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 用户名被占用
        /// </summary>
        AccountInUse,
        /// <summary>
        /// 用户信息无效
        /// </summary>
        UserInfoInvalid,
    }
}
