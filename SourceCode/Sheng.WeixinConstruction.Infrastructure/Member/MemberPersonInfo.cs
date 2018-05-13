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
    /// <summary>
    /// 会员个人信息
    /// MemberEntity的子集
    /// </summary>
    [Table("Member")]
    public class MemberPersonalInfo
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        #region 微信字段

        public string OpenId
        {
            get;
            set;
        }

        public string NickName
        {
            get;
            set;
        }

       

        #endregion

        public string Name
        {
            get;
            set;
        }

        public DateTime? Birthday
        {
            get;
            set;
        }

        public string MobilePhone
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }
    }
}
