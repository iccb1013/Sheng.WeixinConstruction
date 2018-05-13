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
    public class CashAccountCashRefundArgs
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
      
        public Guid Member
        {
            get;
            set;
        }

        /// <summary>
        /// 充值金额 分
        /// </summary>
        public int Fee
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

        public DateTime DateTime
        {
            get;
            set;
        }
    }
}
