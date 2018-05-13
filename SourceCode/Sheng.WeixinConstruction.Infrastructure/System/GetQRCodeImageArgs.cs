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
    public class GetQRCodeImageArgs
    {
        public Guid Domain
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

    public class GetQRCodeImageResult
    {
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 文件服务器上的相对路径
        /// </summary>
        public string FileName
        {
            get;
            set;
        }
    }
}
