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


using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("User")]
    public class UserEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Guid Domain
        {
            get;
            set;
        }

        public string Account
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Telphone
        {
            get;
            set;
        }

        public string MobilePhone
        {
            get;
            set;
        }

        public DateTime RegisterTime
        {
            get;
            set;
        }

        public Guid? MemberId
        {
            get;
            set;
        }

        public string Remark
        {
            get;
            set;
        }

        /// <summary>
        /// 自主注册的用户，域的首个用户
        /// </summary>
        public bool DomainOwner
        {
            get;
            set;
        }

        public bool Removed
        {
            get;
            set;
        }
    }
}
