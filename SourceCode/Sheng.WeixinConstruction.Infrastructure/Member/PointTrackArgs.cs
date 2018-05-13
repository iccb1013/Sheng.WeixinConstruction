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
    public class PointTrackArgs
    {
        public Guid DomainId
        {
            get;
            set;
        }

        public Guid MemberId
        {
            get;
            set;
        }

        /// <summary>
        /// 变化数量（正值增加负值减小）
        /// </summary>
        public int Quantity
        {
            get;
            set;
        }

        public MemberPointTrackType Type
        {
            get;
            set;
        }

        public string TagName
        {
            get;
            set;
        }

        public Guid? TagId
        {
            get;
            set;
        }

        public Guid? OperatorUser
        {
            get;
            set;
        }

        public string Remark
        {
            get;
            set;
        }
    }

    public class PointTrackResult
    {
        /// <summary>
        /// 是否兑换成功
        /// </summary>
        public bool Success
        {
            get;
            set;
        }

        ///// <summary>
        ///// 订购失败时的原因代码
        ///// </summary>
        //public int Reason
        //{
        //    get;
        //    set;
        //}

        public int LeftPoint
        {
            get;
            set;
        }
    }
}
