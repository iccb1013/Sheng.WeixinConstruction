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

namespace Linkup.DataRelationalMapping
{
    /// <summary>
    /// 表示修饰的字段是从另一个关联数据集（Table）中取数据的
    /// </summary>
    public class RelationAttribute : RelationalMappingAttribute
    {
        public string Table
        {
            get;
            private set;
        }

        //"Id=@{RoleId}
        //Id 和 RoleId 都必须是数据库中的字段名
        public string FilterExpression
        {
            get;
            private set;
        }

        //如果修饰的对象是集合，需要指明集合的元素类型
        public Type ElementType
        {
            get;
            set;
        }

        public RelationAttribute(string table, string filterExpression)
        {
            Table = table;
            FilterExpression = filterExpression;
        }
    }
}
