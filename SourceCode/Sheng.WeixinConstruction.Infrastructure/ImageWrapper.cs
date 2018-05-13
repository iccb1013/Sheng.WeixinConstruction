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
    /// <summary>
    /// 图片封装，用于上传多图时封装为json
    /// </summary>
    public class ImageWrapper
    {
        public Guid Id
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }
    }
}
