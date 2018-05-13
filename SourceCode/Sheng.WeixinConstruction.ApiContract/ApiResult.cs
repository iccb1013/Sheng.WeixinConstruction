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
    /// <summary>
    /// 默认false
    /// </summary>
    public class ApiResult
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
        /// 失败时返回一个原因代码
        /// 0成功
        /// 7001：Session过期，主要用于API请求
        /// </summary>
        public int Reason
        {
            get;
            set;
        }

        public ApiResult()
        {

        }

        public ApiResult(bool success)
        {
            Success = success;
        }

        public ApiResult(string message)
        {
            Success = false;
            Message = message;
        }
    }

    public class ApiResult<T> : ApiResult
    {
        public T Data
        {
            get;
            set;
        }
    }
}
