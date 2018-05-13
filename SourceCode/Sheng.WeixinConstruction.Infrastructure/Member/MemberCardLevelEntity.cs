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
    [Table("MemberCardLevel")]
    /// <summary>
    /// 会员卡级别
    /// </summary>
    public class MemberCardLevelEntity
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

        public string AppId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string ImageUrl
        {
            get;
            set;
        }

        public string PrivilegeUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 会员卡左下角卡号字颜色，十六进制颜色值
        /// </summary>
        public string CardNumberColor
        {
            get;
            set;
        }
    }
}
