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
using Linkup.DataRelationalMapping;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkup.Data
{
    /*
     * 配合 Linkup.DataRelationalMapping 对数据库进行操作
     * 
     */

    public class DatabaseWrapper
    {
        private LogService _log = LogService.Instance;
        private ExceptionHandlingService _exceptionHandling = ExceptionHandlingService.Instance;

        private SqlDatabase _database;

        /// <summary>
        /// 用配置文件中 DefaultConnection 创建数据库连接
        /// </summary>
        public DatabaseWrapper()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            _database = new SqlDatabase(connectionString);
        }

        public DatabaseWrapper(string connectionStringConfig)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionStringConfig].ConnectionString;
            _database = new SqlDatabase(connectionString);

        }

        /// <summary>
        ///  int affectedRowCount
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, null);
        }

        public int ExecuteNonQuery(string commandText, List<CommandParameter> parameterList)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, parameterList);
        }

        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(commandType, commandText, null);
        }

        public int ExecuteNonQuery(CommandType commandType, string commandText, List<CommandParameter> parameterList)
        {
            if (_database == null)
                return 0;

            DbCommand cmd;

            if (commandType == CommandType.Text)
            {
                cmd = _database.GetSqlStringCommand(commandText);
            }
            else if (commandType == CommandType.StoredProcedure)
            {
                cmd = _database.GetStoredProcCommand(commandText);
            }
            else
            {
                throw new NotImplementedException("不支持的CommandType");
            }

            if (parameterList != null && parameterList.Count > 0)
            {
                foreach (CommandParameter item in parameterList)
                {
                    DbParameter parameter = cmd.CreateParameter();
                    parameter.ParameterName = item.ParameterName;
                    parameter.Value = item.Value;
                    cmd.Parameters.Add(parameter);
                }
            }

            try
            {
                return _database.ExecuteNonQuery(cmd);
            }
            catch (Exception exception)
            {
                string logMessage = commandText;
                if (parameterList != null)
                {
                    logMessage += Environment.NewLine + JsonHelper.Serializer(parameterList);
                }
                logMessage += Environment.NewLine + exception.StackTrace;
                _log.Write(exception.Message, logMessage, TraceEventType.Error);

                Exception exceptionToThrow;
                _exceptionHandling.HandleException(exception, out exceptionToThrow);
                throw exceptionToThrow;
            }
        }

        public object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(CommandType.Text, commandText, null);
        }

        public bool ExecuteScalar<T>(string commandText, Action<T> callback)
        {
            return ExecuteScalar<T>(CommandType.Text, commandText, null, callback);
        }

        public object ExecuteScalar(string commandText, List<CommandParameter> parameterList)
        {
            return ExecuteScalar(CommandType.Text, commandText, parameterList);
        }

        public bool ExecuteScalar<T>(string commandText, List<CommandParameter> parameterList, Action<T> callback)
        {
            return ExecuteScalar<T>(CommandType.Text, commandText, parameterList, callback);
        }

        public object ExecuteScalar(CommandType commandType, string commandText, List<CommandParameter> parameterList)
        {
            if (_database == null)
                return null;


            DbCommand cmd;

            if (commandType == CommandType.Text)
            {
                cmd = _database.GetSqlStringCommand(commandText);
            }
            else if (commandType == CommandType.StoredProcedure)
            {
                cmd = _database.GetStoredProcCommand(commandText);
            }
            else
            {
                throw new NotImplementedException("不支持的CommandType");
            }

            if (parameterList != null && parameterList.Count > 0)
            {
                foreach (CommandParameter item in parameterList)
                {
                    DbParameter parameter = cmd.CreateParameter();
                    parameter.ParameterName = item.ParameterName;
                    parameter.Value = item.Value;
                    cmd.Parameters.Add(parameter);
                }
            }

            try
            {
                return _database.ExecuteScalar(cmd);
            }
            catch (Exception exception)
            {
                string logMessage = commandText;
                if (parameterList != null)
                {
                    logMessage += Environment.NewLine + JsonHelper.Serializer(parameterList);
                }
                logMessage += Environment.NewLine + exception.StackTrace;
                _log.Write(exception.Message, logMessage, TraceEventType.Error);

                Exception exceptionToThrow;
                _exceptionHandling.HandleException(exception, out exceptionToThrow);
                throw exceptionToThrow;
            }
        }

        public bool ExecuteScalar<T>(CommandType commandType, string commandText, List<CommandParameter> parameterList,
            Action<T> callback)
        {
            object scalarValue = ExecuteScalar(commandType, commandText, parameterList);
            if (scalarValue == null || scalarValue == DBNull.Value)
                return false;
            else
            {
                if (scalarValue.GetType() == typeof(byte) && typeof(T) == typeof(int))
                {
                    scalarValue = Convert.ToInt32(scalarValue);
                }
                else if (typeof(T) == typeof(int))
                {
                    scalarValue = Convert.ToInt32(scalarValue);
                }

                callback((T)scalarValue);
                return true;
            }
        }

        public DataSet ExecuteDataSet(string commandText)
        {
            return ExecuteDataSet(CommandType.Text, commandText, new[] { "Table" });
        }

        public DataSet ExecuteDataSet(string commandText, string[] tableNameArray)
        {
            return ExecuteDataSet(CommandType.Text, commandText, tableNameArray);
        }

        public DataSet ExecuteDataSet(CommandType commandType, string commandText, string[] tableNameArray)
        {
            return ExecuteDataSet(CommandType.Text, commandText, null, tableNameArray);
        }

        public DataSet ExecuteDataSet(string commandText,
           List<CommandParameter> parameterList, string[] tableNameArray)
        {
            return ExecuteDataSet(CommandType.Text, commandText, parameterList, tableNameArray);
        }

        public DataSet ExecuteDataSet(CommandType commandType, string commandText,
            List<CommandParameter> parameterList, string[] tableNameArray)
        {
            if (_database == null)
                return null;

            DbCommand cmd;

            if (commandType == CommandType.Text)
            {
                cmd = _database.GetSqlStringCommand(commandText);
            }
            else if (commandType == CommandType.StoredProcedure)
            {
                cmd = _database.GetStoredProcCommand(commandText);
            }
            else
            {
                throw new NotImplementedException("不支持的CommandType");
            }

            if (parameterList != null && parameterList.Count > 0)
            {
                foreach (CommandParameter item in parameterList)
                {
                    DbParameter parameter = cmd.CreateParameter();
                    parameter.ParameterName = item.ParameterName;
                    parameter.Value = item.Value;
                    cmd.Parameters.Add(parameter);
                }
            }

            DataSet ds;

            try
            {
                ds = _database.ExecuteDataSet(cmd);
            }
            catch (Exception exception)
            {
                string logMessage = commandText;
                if (parameterList != null)
                {
                    logMessage += Environment.NewLine + JsonHelper.Serializer(parameterList);
                }
                logMessage += Environment.NewLine + exception.StackTrace;
                _log.Write(exception.Message, logMessage, TraceEventType.Error);

                Exception exceptionToThrow;
                _exceptionHandling.HandleException(exception, out exceptionToThrow);
                throw exceptionToThrow;
            }

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    if (tableNameArray.Length <= i)
                        break;

                    ds.Tables[i].TableName = tableNameArray[i];
                }
            }
            return ds;

        }

        /// <summary>
        /// 返回受影响的行数
        /// </summary>
        /// <param name="sqlExpression"></param>
        public int ExcuteSqlExpression(SqlExpression sqlExpression)
        {
            int affectedRowCount = 0;
            DbConnection connection = null;
            try
            {
                connection = _database.CreateConnection();
                DbCommand cmd = _database.GetSqlStringCommand(sqlExpression.Sql);
                cmd.Parameters.AddRange(sqlExpression.ParameterList.ToArray());
                cmd.Connection = connection;
                connection.Open();
                affectedRowCount = cmd.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                string logMessage = sqlExpression.ToString();
                logMessage += Environment.NewLine + exception.StackTrace;
                _log.Write(exception.Message, logMessage, TraceEventType.Error);

                Exception exceptionToThrow;
                _exceptionHandling.HandleException(exception, out exceptionToThrow);
                throw exceptionToThrow;
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                    connection.Dispose();
                }
            }

            return affectedRowCount;
        }

        public void ExcuteSqlExpression(List<SqlExpression> sqlExpressionList)
        {
            DbConnection connection = null;
            DbTransaction transaction = null;
            try
            {
                connection = _database.CreateConnection();
                connection.Open();
                transaction = connection.BeginTransaction();

                foreach (var item in sqlExpressionList)
                {
                    try
                    {
                        DbCommand cmd = _database.GetSqlStringCommand(item.Sql);
                        cmd.Parameters.AddRange(item.ParameterList.ToArray());
                        cmd.Connection = connection;
                        cmd.Transaction = transaction;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        if (transaction != null)
                            transaction.Rollback();

                        string logMessage = item.ToString();
                        logMessage += Environment.NewLine + exception.StackTrace;
                        _log.Write(exception.Message, logMessage, TraceEventType.Error);

                        Exception exceptionToThrow;
                        _exceptionHandling.HandleException(exception, out exceptionToThrow);
                        throw exceptionToThrow;
                    }
                }

                transaction.Commit();
            }
            catch (Exception exception)
            {
                if (transaction != null)
                    transaction.Rollback();

                Exception exceptionToThrow;
                _exceptionHandling.HandleException(exception, out exceptionToThrow);
                throw exceptionToThrow;
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                    connection.Dispose();
                }
            }
        }

        private DataSet ExcuteDataSetSqlExpression(SqlExpression sqlExpression)
        {
            return ExecuteDataSet(CommandType.Text, sqlExpression.Sql,
                GetCommandParameterList(sqlExpression.ParameterList), new string[] { "Table" });
        }

        private List<CommandParameter> GetCommandParameterList(List<SqlParameter> list)
        {
            List<CommandParameter> resultList = new List<CommandParameter>();

            if (list != null)
            {
                foreach (var item in list)
                {
                    CommandParameter cmd = new CommandParameter();
                    cmd.ParameterName = item.ParameterName;
                    cmd.Value = item.Value;

                    resultList.Add(cmd);
                }
            }

            return resultList;
        }

        /// <summary>
        /// 填充一个对象，根据对象上有keyAttribute的属性生成where
        /// 如果取出的数据集不是唯一的一条记录，则无法填充
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public bool Fill<T>(object obj) where T : class,new()
        {
            return Fill<T>(obj, null, null);
        }

        public bool Fill<T>(object obj, Dictionary<string, object> attachedWhere) where T : class,new()
        {
            return Fill<T>(obj, null, null);
        }

        public bool Fill<T>(object obj, string table) where T : class,new()
        {
            return Fill<T>(obj, table, null);
        }

        public bool Fill<T>(object obj, string table, Dictionary<string, object> attachedWhere) where T : class,new()
        {
            SqlExpressionArgs args = new SqlExpressionArgs();
            args.Table = table;
            args.Type = SqlExpressionType.Select;
            if (attachedWhere == null)
            {
                args.GenerateWhere = true;
            }
            else
            {
                args.GenerateWhere = false;
                args.AttachedWhere = AttachedWhereItem.Parse(attachedWhere);
            }

            //不能用 default(T) ，会是null
            SqlExpression sqlExpression = RelationalMappingUnity.GetSqlExpression(obj, args);

            DataSet ds = ExcuteDataSetSqlExpression(sqlExpression);
            List<T> dataList = RelationalMappingUnity.Select<T>(ds.Tables[0]);

            Debug.Assert(dataList.Count <= 1, "Fill 时取出的记录大于1条");

            if (dataList.Count != 1)
            {
                return false;
            }

            T dataObj = dataList[0];

            ReflectionHelper.Inject(obj, dataObj);

            return true;
        }


        public List<T> Select<T>() where T : class,new()
        {
            return Select<T>(new Dictionary<string, object>(), null);
        }

        public List<T> Select<T>(Dictionary<string, object> attachedWhere) where T : class,new()
        {
            return Select<T>(AttachedWhereItem.Parse(attachedWhere), null);
        }

        public List<T> Select<T>(Dictionary<string, object> attachedWhere, SqlExpressionPagingArgs pagingArgs) where T : class,new()
        {
            return Select<T>(AttachedWhereItem.Parse(attachedWhere), pagingArgs);
        }

        public List<T> Select<T>(List<AttachedWhereItem> attachedWhere) where T : class,new()
        {
            return Select<T>(attachedWhere, null);
        }

        public List<T> Select<T>(SqlExpressionPagingArgs pagingArgs) where T : class,new()
        {
            return Select<T>(new List<AttachedWhereItem>(), pagingArgs);
        }

        public List<T> Select<T>(List<AttachedWhereItem> attachedWhere, SqlExpressionPagingArgs pagingArgs) where T : class,new()
        {
            SqlExpressionArgs args = new SqlExpressionArgs();
            args.Type = SqlExpressionType.Select;
            args.GenerateWhere = false;
            args.AttachedWhere = attachedWhere;
            args.PagingArgs = pagingArgs;

            //不能用 default(T) ，会是null
            SqlExpression sqlExpression = RelationalMappingUnity.GetSqlExpression(new T(), args);

            DataSet ds = ExcuteDataSetSqlExpression(sqlExpression);
            List<T> dataList = RelationalMappingUnity.Select<T>(ds.Tables[0]);

            if (pagingArgs != null)
            {
                if (ds.Tables.Count > 1)
                {
                    pagingArgs.TotalRow = int.Parse(ds.Tables[1].Rows[0][0].ToString());
                }
                else
                {
                    pagingArgs.TotalRow = ds.Tables[0].Rows.Count;
                }
                pagingArgs.TotalPage = pagingArgs.TotalRow / pagingArgs.PageSize;
                if (pagingArgs.TotalRow % pagingArgs.PageSize > 0)
                    pagingArgs.TotalPage++;
            }

            return dataList;
        }

        public List<T> Select<T>(string sql) where T : class
        {
            DataSet ds = ExecuteDataSet(sql);
            List<T> dataList = RelationalMappingUnity.Select<T>(ds.Tables[0]);
            return dataList;
        }

        public List<T> Select<T>(string sql, List<CommandParameter> parameterList) where T : class
        {
            DataSet ds = ExecuteDataSet(sql, parameterList, new string[] { "Table" });
            List<T> dataList = RelationalMappingUnity.Select<T>(ds.Tables[0]);
            return dataList;
        }

        /// <summary>
        /// 插入失败以异常形式抛出
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Insert(object obj)
        {
            if (obj == null)
                return false;

            SqlExpressionArgs args = new SqlExpressionArgs();
            args.Type = SqlExpressionType.Insert;

            SqlExpression sqlExpression = RelationalMappingUnity.GetSqlExpression(obj, args);
            return ExcuteSqlExpression(sqlExpression) == 1;
        }

        /// <summary>
        /// 封装为一个事务进行写入
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="obj"></param>
        public void InsertList(object obj1, object obj2, params object[] obj)
        {
            List<object> objList = new List<object>();
            objList.Add(obj1);
            objList.Add(obj2);
            if (obj != null && obj.Length > 0)
            {
                foreach (var item in obj)
                {
                    objList.Add(item);
                }
            }
            InsertList(objList);
        }

        /// <summary>
        /// 封装为一个事务进行写入
        /// </summary>
        /// <param name="objList"></param>
        public void InsertList(List<object> objList)
        {
            if (objList == null)
                return;

            List<SqlExpression> sqlExpressionList = new List<SqlExpression>();

            SqlExpressionArgs args = new SqlExpressionArgs();
            args.Type = SqlExpressionType.Insert;

            foreach (var item in objList)
            {
                if (item == null)
                {
                    Debug.Assert(false, "insert obj 为 null");
                    continue;
                }

                SqlExpression sqlExpression = RelationalMappingUnity.GetSqlExpression(item, args);
                sqlExpressionList.Add(sqlExpression);
            }

            ExcuteSqlExpression(sqlExpressionList);
        }

        public int Update(object obj)
        {
            return Update(obj, null);
        }

        public int Update(object obj, string table)
        {
            return Update(obj, table, null);
        }

        public int Update(object obj, string table, string excludeFields)
        {
            if (obj == null)
                return 0;

            SqlExpressionArgs args = new SqlExpressionArgs();
            args.Table = table;
            args.Type = SqlExpressionType.Update;
            args.ExcludeFields = excludeFields;
            SqlExpression sqlExpression = RelationalMappingUnity.GetSqlExpression(obj, args);
            return ExcuteSqlExpression(sqlExpression);
        }

        public void UpdateList(object obj1, object obj2, params object[] obj)
        {
            List<object> objList = new List<object>();
            objList.Add(obj1);
            objList.Add(obj2);
            if (obj != null && obj.Length > 0)
            {
                foreach (var item in obj)
                {
                    objList.Add(item);
                }
            }
            UpdateList(objList);
        }

        public void UpdateList(List<object> objList)
        {
            if (objList == null)
                return;

            List<SqlExpression> sqlExpressionList = new List<SqlExpression>();

            SqlExpressionArgs args = new SqlExpressionArgs();
            args.Type = SqlExpressionType.Update;

            foreach (var item in objList)
            {
                if (item == null)
                {
                    Debug.Assert(false, "insert obj 为 null");
                    continue;
                }

                SqlExpression sqlExpression = RelationalMappingUnity.GetSqlExpression(item, args);
                sqlExpressionList.Add(sqlExpression);
            }

            ExcuteSqlExpression(sqlExpressionList);
        }

        public int Remove(object obj)
        {
            if (obj == null)
                return 0;

            SqlExpressionArgs args = new SqlExpressionArgs();
            args.Type = SqlExpressionType.Delete;
            SqlExpression sqlExpression = RelationalMappingUnity.GetSqlExpression(obj, args);
            return ExcuteSqlExpression(sqlExpression);
        }

        public List<SqlParameter> CommandParameterToSqlParameter(List<CommandParameter> parameterList)
        {
            List<SqlParameter> list = new List<SqlParameter>();

            if (parameterList == null || parameterList.Count == 0)
                return list;

            foreach (var item in parameterList)
            {
                SqlParameter sqlParameter = new SqlParameter(item.ParameterName, item.Value);
                list.Add(sqlParameter);
            }

            return list;
        }

       
    }
}
