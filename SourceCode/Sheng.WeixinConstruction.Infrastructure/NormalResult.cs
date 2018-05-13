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
    /// 一般方法返回
    /// 默认true
    /// </summary>
    public class NormalResult
    {
        /// <summary>
        /// 是否成功
        /// 构造函数里默认为true了
        /// </summary>
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// 返回的消息
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 用于前端根据不同的原因进行不同的处理
        /// </summary>
        public int Reason
        {
            get;
            set;
        }

        /// <summary>
        /// 默认true
        /// </summary>
        public NormalResult()
        {
            Success = true;
        }

        public NormalResult(bool success)
        {
            this.Success = success;
        }

        /// <summary>
        /// Success = false;
        /// </summary>
        /// <param name="message"></param>
        public NormalResult(string message)
        {
            Success = false;
            Message = message;
        }

        public NormalResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }

    public class NormalResult<T> : NormalResult
    {
        public T Data
        {
            get;
            set;
        }

        public NormalResult()
        {

        }

        public NormalResult(bool success)
            : base(success)
        {

        }
    }
}
