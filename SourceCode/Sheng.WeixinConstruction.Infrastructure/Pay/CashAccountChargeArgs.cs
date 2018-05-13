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
    public class CashAccountChargeArgs
    {
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
      
        public Guid MemberId
        {
            get;
            set;
        }

        /// <summary>
        /// 充值金额 分
        /// 用float是为了前端能提供元为单位的数值，JS乘法有BUG不可放前端乘100变分
        /// </summary>
        public float Fee
        {
            get;
            set;
        }

        public string Remark
        {
            get;
            set;
        }

        public Guid OperatorUser
        {
            get;
            set;
        }

        /// <summary>
        /// 操作员操作时IP
        /// </summary>
        public string IP
        {
            get;
            set;
        }

        //public DateTime DateTime
        //{
        //    get;
        //    set;
        //}
    }

}
