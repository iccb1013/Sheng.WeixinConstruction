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

namespace Linkup.Common
{
    public class HttpRequestResult
    {
        public bool Success
        {
            get
            {
                return Exception == null;
            }
        }

        public Exception Exception
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }
    }
}
