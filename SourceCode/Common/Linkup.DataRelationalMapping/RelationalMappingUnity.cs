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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Linkup.DataRelationalMapping
{
    public class RelationalMappingUnity
    {
        static Regex _fieldPatternRegex = new Regex(@"@{\S+}");
        static Regex _fieldRegex = new Regex(@"(?<=@{)\S+(?=})");

        static Regex _validFieldNameRegex = new Regex("^[_a-zA-Z][_a-zA-Z0-9]*$");

        public static List<T> Select<T>(DataSet ds) where T : class
        {
            if (ds == null)
                return null;

            TypeMappingDescription typeCacheCodon = TypeMappingCache.Get(typeof(T));

            if (ds.Tables.Contains(typeCacheCodon.Table) == false)
            {
                throw new ArgumentOutOfRangeException("数据源里没有指定的表：" + typeCacheCodon.Table);
            }

            return Select<T>(ds.Tables[typeCacheCodon.Table]);
        }

        public static List<T> Select<T>(DataTable dt) where T : class
        {
            if (dt == null)
                return null;

            List<T> list = new List<T>();

            if (dt.Rows.Count == 0)
                return list;

            foreach (DataRow row in dt.Rows)
            {
                T t = Select<T>(row);
                if (t != null)
                    list.Add(t);
            }

            return list;
        }

        public static T Select<T>(DataRow dr) where T : class
        {
            return Select(dr, typeof(T)) as T;
        }

        public static List<T> Select<T>(DataRow[] drArray) where T : class
        {
            List<T> list = new List<T>();

            foreach (DataRow dr in drArray)
            {
                T obj = Select(dr, typeof(T)) as T;
                list.Add(obj);
            }

            return list;
        }

        public static object Select(DataRow dr, Type type)
        {
            return Select(dr, type, null);
        }

        public static object Select(DataRow dr, Type type, Dictionary<string, string> fieldPair)
        {
            if (dr == null)
                return null;

            if (type == null)
                throw new ArgumentNullException("没有指定type");

            object resultObj = Activator.CreateInstance(type);

            DataTable dt = dr.Table;

            if (dt.Columns.Count == 0)
                return resultObj;

            TypeMappingDescription typeCache = TypeMappingCache.Get(type);

            Dictionary<string, object> dataDictionary = GetDataDictionary(dr);

            //List<PropertyMappingDescription> propertyList = (from c in typeCache.PropertyList
            //                                              where c.CanWrite && c.NotMapped == false
            //                                              select c).ToList();

            //SELECT 的时候不考虑 NotMapped ，因为数据源可能是 JOIN 出来的，并不是对应着一张物理表
            //只有在插入数据时需要考虑 NotMapped 的情况
            List<PropertyMappingDescription> propertyList = (from c in typeCache.PropertyList
                                                             where c.CanWrite
                                                             select c).ToList();

            foreach (PropertyMappingDescription property in propertyList)
            {
                string columnName = property.Column;

                //fieldPair：额外指定一个列对应关系，优先级高于对象属性名和ColumnAttribute
                if (fieldPair != null && fieldPair.Keys.Contains(property.Name))
                {
                    columnName = fieldPair[property.Name];
                }

                //SELECT 不考虑 NotMapped，有的就选出来
                //if (property.NotMapped)
                //    continue;

                if (property.IsRelation)
                {
                    #region IsRelation

                    DataSet ds = dr.Table.DataSet;
                    if (ds == null)
                    {
                        throw new ArgumentOutOfRangeException("数据源里不存在其它表");
                    }

                    if (ds.Tables.Contains(property.Relation.Table) == false)
                        throw new ArgumentOutOfRangeException("数据源里找不到对应的表："
                            + property.Relation.Table);

                    DataTable relationTable = ds.Tables[property.Relation.Table];

                    string filterExpression = property.Relation.FilterExpression;

                    foreach (Match match in _fieldPatternRegex.Matches(filterExpression))
                    {
                        string fieldPattern = match.Value;
                        string field = _fieldRegex.Match(fieldPattern).Value;

                        if (dataDictionary.Keys.Contains(field) == false)
                        {
                            continue;
                            //throw new ArgumentOutOfRangeException("数据源里找不到对应的数据列：" + field);
                        }

                        object value = dataDictionary[field];
                        if (value is System.DBNull)
                            value = null;

                        //此处不需要调用Covert，对于数据查询用原生数据就好
                        //Covert的主要作用是把比如 ImageId 直接转成 Image 对像
                        //而做表关联查询的时候自然还需要用原来的 ImageId

                        if (value == null)
                            value = String.Empty;

                        filterExpression = filterExpression.Replace(
                            fieldPattern, String.Format("'{0}'", value.ToString()));

                    }

                    DataRow[] dataRowList = relationTable.Select(filterExpression);

                    if (dataRowList.Length > 0)
                    {
                        Type targetlistType = property.PropertyInfo.PropertyType.GetInterface("IList");
                        if (targetlistType != null)
                        {
                            IList list = (IList)Activator.CreateInstance(property.PropertyInfo.PropertyType);
                            foreach (DataRow item in dataRowList)
                            {
                                object value = Select(item, property.Relation.ElementType);
                                if (value != null)
                                    list.Add(value);
                            }
                            property.PropertyInfo.SetValue(resultObj, list, null);
                        }
                        else
                        {
                            if (dataRowList.Length != 1)
                                throw new InvalidOperationException("关联表返回的结果大于1条，而对象非集合类型");

                            object value = Select(dataRowList[0], property.PropertyInfo.PropertyType);
                            property.PropertyInfo.SetValue(resultObj, value, null);
                        }
                    }

                    #endregion
                }
                else if (property.IsPartial)
                {
                    object value = Select(dr, property.PropertyInfo.PropertyType, property.Partial.FieldDictionary);
                    property.PropertyInfo.SetValue(resultObj, value, null);
                }
                else
                {
                    if (dataDictionary.Keys.Contains(columnName) == false)
                    {
                        //  Debug.Assert(false, "数据源里找不到对应的数据列");
                        //throw new ArgumentOutOfRangeException("数据源里找不到对应的数据列："
                        //    + typeCache.Type.FullName + "." + property.Name);
                        continue;
                    }

                    object value = dataDictionary[columnName];
                    if (value is System.DBNull)
                        value = null;

                    if (property.Convert != null)
                    {
                        value = property.Convert.CovertFrom(value);
                    }

                    if (property.Json && value !=null)
                    {
                        value = JsonHelper.Deserialize(value.ToString(), property.PropertyInfo.PropertyType);
                    }

                    //如果 value 是 DateTime 类型，虽然 value.ToString() 看不到毫秒
                    //但是如果直接设置到对象的 DateTime 属性上，这个毫秒会一直带着
                    //而其它地方如果用 SELECT 语句单独取出字段中的时间，然后用 DateTime.Parse() 去转
                    //会丢失掉毫秒，造成日期时间的比对因为毫秒的丢失而不一致

                    property.PropertyInfo.SetValue(resultObj, value, null);

                }
            }

            return resultObj;
        }

        private static Dictionary<string, object> GetDataDictionary(DataRow dr)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            DataTable dt = dr.Table;

            foreach (DataColumn column in dt.Columns)
            {
                dictionary.Add(column.ColumnName, dr[column]);
            }

            return dictionary;
        }

        public static SqlExpression GetSqlExpression(object obj, SqlExpressionType type)
        {
            SqlExpressionArgs args = new SqlExpressionArgs(type);
            return GetSqlExpression(obj, args);
        }

        /// <summary>
        /// 即使是 select 语句，也必须提供一个初始化过的obj
        /// 主要原因是需要从中取出 sql 的 where 部分
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static SqlExpression GetSqlExpression(object obj, SqlExpressionArgs args)
        {
            if (obj == null || args == null)
                throw new ArgumentNullException();

            Type objType = obj.GetType();

            TypeMappingDescription typeCache = TypeMappingCache.Get(objType);

            string tableName = typeCache.Table;

            if (String.IsNullOrEmpty(args.Table) == false)
            {
                tableName = args.Table;
            }

            StringBuilder sql = new StringBuilder();
            //用于SELECT 且需要分页时，统计总行数
            StringBuilder sqlCount = null;
            List<SqlParameter> parameterList = new List<SqlParameter>();

            if (args.Type == SqlExpressionType.Insert)
            {
                sql.Append("INSERT INTO [" + tableName + "]");
                sql.Append(" " + GetInsertSqlPartial(obj, args, typeCache, parameterList));
            }
            else if (args.Type == SqlExpressionType.Select ||
                args.Type == SqlExpressionType.Update || args.Type == SqlExpressionType.Delete)
            {
                List<string> keyFieldList = null;

                if (args.GenerateWhere)
                {
                    //KEY可以通过args指定也可以通过KeyAttribute在对象上标记
                    if (String.IsNullOrEmpty(args.KeyFields) == false)
                        keyFieldList = args.KeyFields.Split(',').ToList();
                    else
                        keyFieldList = new List<string>();

                    if (keyFieldList.Count == 0)
                    {
                        var keyProperties = (from c in typeCache.PropertyList
                                             where c.CanRead && c.Key
                                             select c.Name).ToList();
                        foreach (string item in keyProperties)
                        {
                            keyFieldList.Add(item);
                        }
                    }
                }

                if (args.Type == SqlExpressionType.Select)
                {
                    if (args.PagingArgs != null)
                    {
                        sqlCount = new StringBuilder();
                        sqlCount.Append("SELECT Count(*) ");
                    }

                    //生成 select 不支持关联属性，分部属性 
                    sql.Append("SELECT ");

                    string[] includeFieldList = new string[0];
                    if (String.IsNullOrEmpty(args.IncludeFields) == false)
                        includeFieldList = args.IncludeFields.Split(',');

                    string[] excludeFieldList = new string[0];
                    if (String.IsNullOrEmpty(args.ExcludeFields) == false)
                        excludeFieldList = args.ExcludeFields.Split(',');

                    List<PropertyMappingDescription> selectPropertyList =
                        GetPropertyList(objType, includeFieldList, excludeFieldList);

                    if (selectPropertyList.Count == 0)
                    {
                        sql.Append("'DATA' as Column1");
                    }
                    else
                    {
                        foreach (var item in selectPropertyList)
                        {
                            sql.Append("[" + item.Column + "]");
                            sql.Append(",");
                        }

                        sql.Remove(sql.Length - 1, 1);
                    }

                    sql.Append(" FROM [" + tableName + "]");

                    if (sqlCount != null)
                        sqlCount.Append(" FROM [" + tableName + "]");
                }
                else if (args.Type == SqlExpressionType.Update)
                {
                    sql.Append("UPDATE [" + tableName + "] SET");
                    sql.Append(" " + GetUpdateSqlPartial(obj, args, typeCache, keyFieldList, parameterList));
                }
                else
                {
                    sql.Append("DELETE FROM [" + tableName + "]");
                }

                StringBuilder sqlWhere = new StringBuilder();

                List<PropertyMappingDescription> whereProperties = null;
                if (args.GenerateWhere)
                {

                    whereProperties = (from c in typeCache.PropertyList
                                       where keyFieldList.Contains(c.Name)
                                       select c).ToList();
                    #region whereProperties

                    if (whereProperties.Count > 0)
                    {
                        sqlWhere.Append(" WHERE ");


                        foreach (PropertyMappingDescription property in whereProperties)
                        {
                            object parameterValue = property.PropertyInfo.GetValue(obj, null);
                            if (property.Convert != null)
                            {
                                parameterValue = property.Convert.CovertTo(parameterValue);
                            }
                            if (parameterValue == null)
                            {
                                parameterValue = System.DBNull.Value;
                            }
                            if (property.Json && parameterValue != null && parameterValue != DBNull.Value)
                            {
                                parameterValue = JsonHelper.Serializer(parameterValue);
                            }

                            sqlWhere.Append("[" + property.Column + "]");
                            sqlWhere.Append("=");

                            string parameterName = String.Format("@{0}", property.Column);

                            sqlWhere.Append(parameterName);
                            sqlWhere.Append(" AND ");

                            if ((from c in parameterList
                                 where c.ParameterName == parameterName
                                 select c).Count() == 0)
                            {
                                SqlParameter parameter = new SqlParameter();
                                parameter.ParameterName = parameterName;
                                parameter.Value = parameterValue;
                                parameterList.Add(parameter);
                            }
                        }

                        int andLength = " AND ".Length;

                        sqlWhere.Remove(sqlWhere.Length - andLength, andLength);

                        sql.Append(sqlWhere);
                        if (sqlCount != null)
                            sqlCount.Append(sqlWhere);
                    }

                    #endregion
                }

                //添加额外特别指定的 Where 条件
                if (args.AttachedWhere != null && args.AttachedWhere.Count > 0)
                {
                    //GenerateWhere 为 true 或 false不能作为判断要不要加 where 的标准
                    //因为为 true 也有可能不添加
                    if (whereProperties == null || whereProperties.Count == 0)
                    {
                        sqlWhere.Append(" WHERE ");
                    }

                    foreach (var attachedWhereItem in args.AttachedWhere)
                    {
                        if (attachedWhereItem.Value != null)
                        {
                            sqlWhere.Append("[" + attachedWhereItem.Field + "]");

                            if (attachedWhereItem.Type == AttachedWhereType.Equal)
                            {
                                sqlWhere.Append(" = ");
                            }
                            else if (attachedWhereItem.Type == AttachedWhereType.Like)
                            {
                                sqlWhere.Append(" LIKE ");
                            }
                            else
                            {
                                Debug.Assert(false, "attachedWhereItem.Type 不支持");
                            }

                            string parameterName = String.Format("@{0}", attachedWhereItem.Field);

                            sqlWhere.Append(parameterName);
                            sqlWhere.Append(" AND ");

                            SqlParameter parameter = new SqlParameter();
                            parameter.ParameterName = parameterName;
                            if (attachedWhereItem.Type == AttachedWhereType.Equal)
                            {
                                parameter.Value = attachedWhereItem.Value;
                            }
                            else if (attachedWhereItem.Type == AttachedWhereType.Like)
                            {
                                parameter.Value = "%" + attachedWhereItem.Value + "%";
                            }

                            parameterList.Add(parameter);
                        }
                        else if (attachedWhereItem.ValueArray != null && attachedWhereItem.ValueArray.Length > 0)
                        {
                            sqlWhere.Append(" ( ");

                            for (int i = 0; i < attachedWhereItem.ValueArray.Length; i++)
                            {
                                sqlWhere.Append("[" + attachedWhereItem.Field + "]");

                                if (attachedWhereItem.Type == AttachedWhereType.Equal)
                                {
                                    sqlWhere.Append(" = ");
                                }
                                else if (attachedWhereItem.Type == AttachedWhereType.Like)
                                {
                                    sqlWhere.Append(" LIKE ");
                                }
                                else
                                {
                                    Debug.Assert(false, "attachedWhereItem.Type 不支持");
                                }

                                string parameterName = String.Format("@{0}", attachedWhereItem.Field + i.ToString());

                                sqlWhere.Append(parameterName);
                                if (i < attachedWhereItem.ValueArray.Length - 1)
                                {
                                    sqlWhere.Append(" OR ");
                                }

                                SqlParameter parameter = new SqlParameter();
                                parameter.ParameterName = parameterName;
                                if (attachedWhereItem.Type == AttachedWhereType.Equal)
                                {
                                    parameter.Value = attachedWhereItem.ValueArray[i];
                                }
                                else if (attachedWhereItem.Type == AttachedWhereType.Like)
                                {
                                    parameter.Value = "%" + attachedWhereItem.ValueArray[i] + "%";
                                }

                                parameterList.Add(parameter);
                            }

                            sqlWhere.Append(" ) ");
                            sqlWhere.Append(" AND ");
                        }
                    }

                    int andLength = " AND ".Length;

                    sqlWhere.Remove(sqlWhere.Length - andLength, andLength);

                    sql.Append(sqlWhere);
                    if (sqlCount != null)
                        sqlCount.Append(sqlWhere);
                }

                if (args.Type == SqlExpressionType.Select)
                {
                    List<PropertyMappingDescription> orderByProperties = (from c in typeCache.PropertyList
                                                                          where c.CanRead && c.OrderBy != null
                                                                          select c).ToList();

                    Debug.Assert(orderByProperties.Count <= 1, "设置了多个 OrderByAttribute");

                    //分页
                    if (args.PagingArgs != null)
                    {
                        Debug.Assert(orderByProperties.Count == 1, "设置了 PagingArgs 的情况下必须指定一个 OrderByAttribute");

                        if (orderByProperties.Count == 1)
                        {
                            int startRowNum = (args.PagingArgs.Page - 1) * args.PagingArgs.PageSize + 1;
                            int endRowNum = startRowNum - 1 + args.PagingArgs.PageSize;

                            PropertyMappingDescription orderByPropery = orderByProperties[0];

                            sql.Replace("SELECT ",
                                "SELECT ROW_NUMBER() OVER ( ORDER BY " + orderByPropery.Column
                                + (orderByPropery.OrderBy.OrderBy == OrderBy.ASC ? " ASC" : " DESC") + " ) AS rownum, ");

                            sql.Insert(0, "SELECT * FROM ( ");
                            sql.Append(" ) AS temp WHERE temp.rownum BETWEEN " + startRowNum + " AND " + endRowNum);

                            sql.Append(";");
                            sql.Append(sqlCount);
                        }
                    }
                    else
                    {
                        if (orderByProperties.Count > 0)
                        {
                            PropertyMappingDescription orderByPropery = orderByProperties[0];
                            sql.Append(" ORDER BY [");
                            sql.Append(orderByPropery.Column);
                            sql.Append("] ");

                            if (orderByPropery.OrderBy.OrderBy == OrderBy.ASC)
                                sql.Append(" ASC");
                            else
                                sql.Append(" DESC");
                        }
                    }
                }
            }
            else
            {
                throw new NotImplementedException("不支持的SqlPairType");
            }

            SqlExpression sqlExpression = new SqlExpression();
            sqlExpression.Sql = sql.ToString();
            sqlExpression.ParameterList = parameterList;

            return sqlExpression;
        }


        private static string GetInsertSqlPartial(object obj, SqlExpressionArgs args, TypeMappingDescription typeCache,
            List<SqlParameter> parameterList)
        {
            StringBuilder fieldString = new StringBuilder();
            StringBuilder valueString = new StringBuilder();

            fieldString.Append("(");
            valueString.Append("(");

            string[] includeFieldList = new string[0];
            if (String.IsNullOrEmpty(args.IncludeFields) == false)
                includeFieldList = args.IncludeFields.Split(',');

            string[] excludeFieldList = new string[0];
            if (String.IsNullOrEmpty(args.ExcludeFields) == false)
                excludeFieldList = args.ExcludeFields.Split(',');

            Dictionary<PropertyMappingDescription, object> propertyList =
                GetPropertyValueList(obj, includeFieldList, excludeFieldList);

            foreach (var property in propertyList)
            {
                if (property.Value == null)
                    continue;

                fieldString.Append("[" + property.Key.Column + "]");
                fieldString.Append(",");

                string parameterName = String.Format("@{0}", property.Key.Column);

                valueString.Append(parameterName);
                valueString.Append(",");

                if ((from c in parameterList
                     where c.ParameterName == parameterName
                     select c).Count() == 0)
                {
                    SqlParameter parameter = new SqlParameter();
                    parameter.ParameterName = parameterName;
                    parameter.Value = property.Value;
                    parameterList.Add(parameter);
                }
            }

            fieldString.Remove(fieldString.Length - 1, 1);
            valueString.Remove(valueString.Length - 1, 1);

            fieldString.Append(") VALUES").Append(valueString).Append(")");

            return fieldString.ToString();
        }

        private static string GetUpdateSqlPartial(object obj, SqlExpressionArgs args, TypeMappingDescription typeCache,
            List<string> keyFieldList, List<SqlParameter> parameterList)
        {
            StringBuilder fieldString = new StringBuilder();

            string[] includeFieldList = new string[0];
            if (String.IsNullOrEmpty(args.IncludeFields) == false)
                includeFieldList = args.IncludeFields.Split(',');

            string[] excludeFieldList = new string[0];
            if (String.IsNullOrEmpty(args.ExcludeFields) == false)
                excludeFieldList = args.ExcludeFields.Split(',');

            Dictionary<PropertyMappingDescription, object> propertyList =
                GetPropertyValueList(obj, includeFieldList, excludeFieldList);

            foreach (var property in propertyList)
            {
                object parameterValue = property.Value;

                if (parameterValue == null)
                {
                    parameterValue = System.DBNull.Value;
                }

                //在 GetPropertyValueList 方法中已经转成JSON字符串了
                //if (property.Key.Json)
                //{
                //    parameterValue = JsonHelper.Serializer(parameterValue);
                //}

                fieldString.Append("[" + property.Key.Column + "]");
                fieldString.Append("=");

                string parameterName = String.Format("@{0}", property.Key.Column);

                fieldString.Append(parameterName);
                fieldString.Append(",");

                if ((from c in parameterList
                     where c.ParameterName == parameterName
                     select c).Count() == 0)
                {
                    SqlParameter parameter = new SqlParameter();
                    parameter.ParameterName = parameterName;
                    parameter.Value = parameterValue;
                    parameterList.Add(parameter);
                }
            }

            fieldString.Remove(fieldString.Length - 1, 1);

            return fieldString.ToString();
        }


        private static Dictionary<PropertyMappingDescription, object> GetPropertyValueList(object obj,
            string[] includeFieldList, string[] excludeFieldList)
        {
            Dictionary<PropertyMappingDescription, object> list = new Dictionary<PropertyMappingDescription, object>();

            if (obj != null)
                GetPropertyValueList(obj, list, includeFieldList, excludeFieldList);

            return list;
        }

        private static void GetPropertyValueList(object obj, Dictionary<PropertyMappingDescription, object> list,
            string[] includeFieldList, string[] excludeFieldList)
        {
            if (obj == null || list == null)
                return;

            Type objType = obj.GetType();
            TypeMappingDescription typeCache = TypeMappingCache.Get(objType);

            List<PropertyMappingDescription> properties =
                (from c in typeCache.PropertyList
                 where c.CanRead && c.NotMapped == false && c.IsRelation == false
                 select c).ToList();

            List<PropertyMappingDescription> propertyList;

            if (includeFieldList.Length > 0)
            {
                propertyList = (from c in properties
                                where includeFieldList.Contains(c.Name)
                                select c).ToList();
            }
            else if (excludeFieldList.Length > 0)
            {
                propertyList = (from c in properties
                                where excludeFieldList.Contains(c.Name) == false
                                select c).ToList();
            }
            else
            {
                propertyList = properties.ToList();
            }

            foreach (PropertyMappingDescription property in propertyList)
            {
                //先看有没有同名的 Property 已经被添加过了，如果有，忽略后面同名的
                //如站点参数设置，本身有 SiteId，后面每一个 Partial 都有自己的 SiteId
                var sameProperty = from c in list.Keys where c.Name == property.Name select c;
                if (sameProperty.Count() > 0)
                    continue;

                object parameterValue = property.PropertyInfo.GetValue(obj, null);

                if (property.IsPartial)
                {
                    GetPropertyValueList(parameterValue, list, includeFieldList, excludeFieldList);
                }
                else
                {
                    if (property.Convert != null)
                    {
                        parameterValue = property.Convert.CovertTo(parameterValue);
                    }

                    if (property.Json && parameterValue != null && parameterValue != DBNull.Value)
                    {
                        parameterValue = JsonHelper.Serializer(parameterValue);
                    }

                    list.Add(property, parameterValue);
                }
            }
        }

        private static List<PropertyMappingDescription> GetPropertyList(Type objType,
            string[] includeFieldList, string[] excludeFieldList)
        {
            List<PropertyMappingDescription> list = new List<PropertyMappingDescription>();

            if (objType != null)
                GetPropertyList(objType, list, includeFieldList, excludeFieldList);

            return list;
        }

        private static void GetPropertyList(Type objType, List<PropertyMappingDescription> list,
            string[] includeFieldList, string[] excludeFieldList)
        {
            if (objType == null || list == null)
                return;

            //   Type objType = obj.GetType();
            TypeMappingDescription typeCache = TypeMappingCache.Get(objType);

            List<PropertyMappingDescription> properties =
                (from c in typeCache.PropertyList
                 where c.CanRead && c.NotMapped == false && c.IsRelation == false
                 select c).ToList();

            List<PropertyMappingDescription> propertyList;

            if (includeFieldList.Length > 0)
            {
                propertyList = (from c in properties
                                where includeFieldList.Contains(c.Name)
                                select c).ToList();
            }
            else if (excludeFieldList.Length > 0)
            {
                propertyList = (from c in properties
                                where excludeFieldList.Contains(c.Name) == false
                                select c).ToList();
            }
            else
            {
                propertyList = properties.ToList();
            }

            foreach (PropertyMappingDescription property in propertyList)
            {
                if (property.IsPartial)
                {
                    GetPropertyList(property.PropertyInfo.PropertyType,
                        list, includeFieldList, excludeFieldList);
                }
                else
                {
                    list.Add(property);
                }
            }
        }

        /// <summary>
        /// 用于验证字符串是否是有效的字段名或表名（变量名）
        /// 主要用于验证前端发来的orderby参数，防止注入攻击
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValidFieldName(string value)
        {
            return _validFieldNameRegex.IsMatch(value);
        }
    }
}
