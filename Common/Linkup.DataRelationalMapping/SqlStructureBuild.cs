using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Linkup.DataRelationalMapping
{
    /// <summary>
    /// 用于通过给定参数列表的方式生成SqlExpression
    /// 也就是不用对象生成，而是直接指定字段，参数列表
    /// </summary>
    public class SqlStructureBuild
    {
        private List<SqStructureBuildParameter> _parameterList = new List<SqStructureBuildParameter>();

        private SqlExpressionType _type = SqlExpressionType.Insert;
        public SqlExpressionType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Table
        {
            get;
            set;
        }

        public void AddParameter(SqStructureBuildParameter parameter)
        {
            if (parameter == null || _parameterList.Contains(parameter))
                return;

            _parameterList.Add(parameter);
        }

        public void AddParameter(string field, object value)
        {
            if (value == null)
                value = DBNull.Value;

            AddParameter(field, value, false);
        }

        public void AddParameter(string field, object value, bool isWhere)
        {
            SqStructureBuildParameter parameter = new SqStructureBuildParameter();
            parameter.Field = field;
            parameter.Value = value;
            parameter.IsWhere = isWhere;
            _parameterList.Add(parameter);
        }

        public SqlExpression GetSqlExpression()
        {
            if (String.IsNullOrEmpty(Table) || _parameterList == null || _parameterList.Count == 0)
                return null;

            if (Type == SqlExpressionType.Insert)
            {
                return GetInsertSqlExpression();
            }
            else if (Type == SqlExpressionType.Update)
            {
                return GetUpdateSqlExpression();
            }
            else
            {
                Debug.Assert(false, "不支持的 SqlExpressionType");
                return null;
            }
        }

        private SqlExpression GetInsertSqlExpression()
        {
            StringBuilder sql = new StringBuilder();

            List<SqlParameter> sqlParameterList = new List<SqlParameter>();

            sql.Append("INSERT INTO [" + Table + "] ");

            StringBuilder fieldString = new StringBuilder();
            StringBuilder valueString = new StringBuilder();

            fieldString.Append("(");
            valueString.Append("(");

            foreach (var item in _parameterList)
            {
                if (item == null || String.IsNullOrEmpty(item.Field))
                    continue;

                fieldString.Append("[");
                fieldString.Append(item.Field);
                fieldString.Append("],");

                string parameterName = String.Format("@{0}", item.Field);

                valueString.Append(parameterName);
                valueString.Append(",");

                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = parameterName;
                if (item.Value != null)
                    parameter.Value = item.Value;
                else
                    parameter.Value = DBNull.Value;
                sqlParameterList.Add(parameter);
            }

            fieldString.Remove(fieldString.Length - 1, 1);
            valueString.Remove(valueString.Length - 1, 1);

            sql.Append(fieldString).Append(") VALUES ").Append(valueString).Append(")");

            SqlExpression sqlExpression = new SqlExpression();
            sqlExpression.Sql = sql.ToString();
            sqlExpression.ParameterList = sqlParameterList;

            return sqlExpression;
        }

        private SqlExpression GetUpdateSqlExpression()
        {
            StringBuilder sql = new StringBuilder();

            List<SqlParameter> sqlParameterList = new List<SqlParameter>();

            sql.Append("UPDATE [" + Table + "] SET ");

            foreach (var item in _parameterList)
            {
                if (item.IsWhere)
                    continue;

                sql.Append("[" + item.Field + "]");
                sql.Append("=");

                string parameterName = String.Format("@{0}", item.Field);

                sql.Append(parameterName);
                sql.Append(",");

                if ((from c in sqlParameterList
                     where c.ParameterName == parameterName
                     select c).Count() == 0)
                {
                    SqlParameter parameter = new SqlParameter();
                    parameter.ParameterName = parameterName;
                    parameter.Value = item.Value;
                    sqlParameterList.Add(parameter);
                }
            }

            sql.Remove(sql.Length - 1, 1);

            var whereParameter = (from c in _parameterList
                     where c.IsWhere select c).ToList();

            if (whereParameter.Count > 0)
            {
                sql.Append(" WHERE ");

                foreach (var item in whereParameter)
                {
                    sql.Append("[" + item.Field + "]");
                    sql.Append("=");

                    string parameterName = String.Format("@{0}", item.Field);

                    sql.Append(parameterName);
                    sql.Append(" AND ");

                    if ((from c in sqlParameterList
                         where c.ParameterName == parameterName
                         select c).Count() == 0)
                    {
                        SqlParameter parameter = new SqlParameter();
                        parameter.ParameterName = parameterName;
                        parameter.Value = item.Value;
                        sqlParameterList.Add(parameter);
                    }
                }

                int andLength = " AND ".Length;

                sql.Remove(sql.Length - andLength, andLength);
            }

            SqlExpression sqlExpression = new SqlExpression();
            sqlExpression.Sql = sql.ToString();
            sqlExpression.ParameterList = sqlParameterList;

            return sqlExpression;
        } 

    }

    public class SqStructureBuildParameter
    {
        /// <summary>
        /// 数据库中的字段名
        /// </summary>
        public string Field
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }

        private bool _isWhere = false;
        /// <summary>
        /// 是否作为where条件使用
        /// </summary>
        public bool IsWhere
        {
            get { return _isWhere; }
            set { _isWhere = value; }
        }

        public SqStructureBuildParameter()
        {

        }

        public SqStructureBuildParameter(string name, object value)
        {
            Field = name;
            Value = value;
        }
    }
}
