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

namespace Linkup.Common
{
    public class HttpDownloadArgs
    {
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// 要接收数据的本地路径。
        /// 绝对路径
        /// </summary>
        public string TargetDir
        {
            get;
            set;
        }
    }
}
