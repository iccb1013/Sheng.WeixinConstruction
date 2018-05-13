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


using Linkup.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract.PayApi
{
    /*
     * 这里的类用以封装微信支付API的返回
     */
    public class RequestPayApiResult<T> where T : class
    {
        public HttpRequestResult HttpRequestResult
        {
            get;
            set;
        }

        public T ApiResult
        {
            get;
            set;
        }

        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool Success
        {
            get;
            set;
            //get
            //{
            //    return HttpRequestResult.Success;
            //}
        }

        public string Message
        {
            get;
            set;
            //get
            //{
            //    if (HttpRequestResult != null && HttpRequestResult.Success == false)
            //        return HttpRequestResult.Exception.Message;

            //    return String.Empty;
            //}
        }
    }
}
