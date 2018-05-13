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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetItemListArgs
    {
        /// <summary>
        /// 取第几页的数据
        /// </summary>
        public int Page
        {
            get;
            set;
        }

        private int _pageSize = 10;
        /// <summary>
        /// 每页多少条数据，默认10
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public Guid DomainId
        {
            get;
            set;
        }

        public string AppId
        {
            get;
            set;
        }

    }

    /// <summary>
    /// 获取分页数据的通过结果
    /// </summary>
    public class GetItemListResult
    {
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage
        {
            get;
            set;
        }

        /// <summary>
        /// 当前页
        /// </summary>
        public int Page
        {
            get;
            set;
        }

        /// <summary>
        /// 总条目数
        /// </summary>
        public int TotalCount
        {
            get;
            set;
        }

        public DataTable ItemList
        {
            get;
            set;
        }
    }

    public class GetItemListResult<T>
    {
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage
        {
            get;
            set;
        }

        /// <summary>
        /// 当前页
        /// </summary>
        public int Page
        {
            get;
            set;
        }

        public List<T> ItemList
        {
            get;
            set;
        }
    }
}
