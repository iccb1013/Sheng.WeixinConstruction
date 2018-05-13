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
    public class SignInResult
    {
        /// <summary>
        /// 是否签到成功
        /// </summary>
        public bool Success
        {
            get { return Reason == 0; }
        }

        /// <summary>
        /// 签到失败时的原因代码
        /// 1:已签到过了
        /// </summary>
        public int Reason
        {
            get;
            set;
        }

        /// <summary>
        /// 签到获得多少积分
        /// </summary>
        public int SignInPoint
        {
            get;
            set;
        }

        /// <summary>
        /// 签到后最新积分
        /// </summary>
        public int Point
        {
            get;
            set;
        }
    }
}
