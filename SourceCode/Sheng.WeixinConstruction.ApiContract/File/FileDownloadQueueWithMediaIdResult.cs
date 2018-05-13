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

namespace Sheng.WeixinConstruction.ApiContract
{
    public class FileDownloadQueueWithMediaIdResult
    {
        public bool Success
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 返回保存的相对路径（包括文件名）
        /// </summary>
        public string OutputFile
        {
            get;
            set;
        }

        /// <summary>
        /// 附加数据，返回结果中附带上
        /// </summary>
        public string Tag
        {
            get;
            set;
        }

        public string ContentType
        {
            get;
            set;
        }

        public string MediaId
        {
            get;
            set;
        }

        /// <summary>
        /// 文件大小（KB）
        /// </summary>
        public int FileLength
        {
            get;
            set;
        }
    }
}
