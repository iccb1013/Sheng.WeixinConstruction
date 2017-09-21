using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkup.DataRelationalMapping
{
    /// <summary>
    /// 指定的都是字段名，而不是数据库列名
    /// </summary>
    public class SqlExpressionArgs
    {
        /// <summary>
        /// 如果指定此 Table 名称
        /// 则忽略实体对象上的 Table 属性 和类型本身的名称
        /// </summary>
        public string Table
        {
            get;
            set;
        }

        private string _keyFields;
        /// <summary>
        /// 在Update,Delete语句里作为条件的字段
        /// 如果指定了此属性，则忽略加在对象上的KeyAttribute
        /// </summary>
        public string KeyFields
        {
            get { return _keyFields; }
            set { _keyFields = value; }
        }

        private string _includeFields;
        /// <summary>
        /// 在Update,Delete语句里包含的字段
        /// 如果指定了 IncludeFields ，则忽略 ExcludeFields，逗号分隔
        /// </summary>
        public string IncludeFields
        {
            get { return _includeFields; }
            set { _includeFields = value; }
        }

        private string _excludeFields;
        /// <summary>
        /// 在Update,Delete语句里排除的字段，逗号分隔
        /// </summary>
        public string ExcludeFields
        {
            get { return _excludeFields; }
            set { _excludeFields = value; }
        }

        private SqlExpressionType _type = SqlExpressionType.Insert;
        public SqlExpressionType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private bool _generateWhere = true;
        /// <summary>
        /// 是否生成where子句
        /// 在全量select时不生成
        /// 但对 AttachedWhere 不起作用
        /// </summary>
        public bool GenerateWhere
        {
            get { return _generateWhere; }
            set { _generateWhere = value; }
        }

        private List<AttachedWhereItem> _attachedWhere = new List<AttachedWhereItem>();
        /// <summary>
        /// 在自动生成的 where 语句的基础上 强制 额外附加的 where 条件
        /// 无视其它任何字段设定，包括 GenerateWhere 无视
        /// </summary>
        public List<AttachedWhereItem> AttachedWhere
        {
            get { return _attachedWhere; }
            set { _attachedWhere = value; }
        }

        /// <summary>
        /// 如果指定，则在select语句中生成分页语句并取得指定页的数据
        /// </summary>
        public SqlExpressionPagingArgs PagingArgs
        {
            get;
            set;
        }

        public SqlExpressionArgs()
        {

        }

        public SqlExpressionArgs(SqlExpressionType type)
        {
            Type = type;
        }
    }

    public class SqlExpressionPagingArgs
    {
        private int _page;
        /// <summary>
        /// 目标页
        /// </summary>
        public int Page
        {
            get { return _page; }
            set { _page = value; }
        }

        private int _pageSize = 10;
        /// <summary>
        /// 每页多少条数据
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (value <= 0)
                {
                    _pageSize = 10;
                }
                else
                {
                    _pageSize = value;
                }
            }
        }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage
        {
            get;
            set;
        }

        /// <summary>
        /// 总行数
        /// </summary>
        public int TotalRow
        {
            get;
            set;
        }
    }

    public enum SqlExpressionType
    {
        Select,
        Insert,
        Update,
        Delete
    }
}
