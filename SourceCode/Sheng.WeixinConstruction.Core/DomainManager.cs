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
using Linkup.Data;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class DomainManager
    {
        private static readonly DomainManager _instance = new DomainManager();
        public static DomainManager Instance
        {
            get { return _instance; }
        }

        private LogService _log = LogService.Instance;
        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;


        private DomainManager()
        {

        }

        public void Create(DomainEntity domain)
        {
            if (domain == null)
            {
                Debug.Assert(false, "domain == null");
                return;
            }

            _dataBase.Insert(domain);

        }

        public DomainEntity GetDomain(Guid id)
        {
            DomainEntity domain = new DomainEntity();
            domain.Id = id;

            if (_dataBase.Fill<DomainEntity>(domain))
                return domain;
            else
                return null;
        }

        public void UpdateLastUpdateTime(Guid domainId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", domainId));

            _dataBase.ExecuteNonQuery("UPDATE [Domain] SET [LastUpdateTime]=GETDATE() WHERE [Id] = @id", parameterList);
        }

        public DateTime GetLastUpdateTime(Guid domainId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", domainId));

            DateTime lastUpdateTime = DateTime.MinValue;
            _dataBase.ExecuteScalar<DateTime>(
                "SELECT [LastUpdateTime] FROM [Domain] WHERE [Id] = @id", parameterList,
                (scalarValue) =>
                {
                    lastUpdateTime = scalarValue;
                });
            return lastUpdateTime;
        }

        public void UpdateLastDockingTime(Guid domainId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", domainId));

            _dataBase.ExecuteNonQuery("UPDATE [Domain] SET [LastDockingTime]=GETDATE() WHERE [Id] = @id", parameterList);
        }

        public DateTime? GetLastDockingTime(Guid domainId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", domainId));

            DateTime? lastDockingTime = null;
            _dataBase.ExecuteScalar<DateTime>(
                "SELECT [LastDockingTime] FROM [Domain] WHERE [Id] = @id", parameterList,
                (scalarValue) => { lastDockingTime = scalarValue; });
            return lastDockingTime;
        }

        public OperationData GetOperationData(Guid domainId, string appId, DateTime date)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@date", date));

            DataSet dsResult =
               _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetOperationData", parameterList,
               new string[] { "newAttentionCount", "totalAttentionCount" });

            OperationData result = new OperationData();

            if (dsResult.Tables["newAttentionCount"].Rows.Count > 0)
            {
                string strNewAttentionCount = dsResult.Tables["newAttentionCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strNewAttentionCount) == false)
                    result.NewAttentionCount = int.Parse(strNewAttentionCount);
            }

            if (dsResult.Tables["totalAttentionCount"].Rows.Count > 0)
            {
                string strTotalAttentionCount = dsResult.Tables["totalAttentionCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strTotalAttentionCount) == false)
                    result.TotalAttentionCount = int.Parse(strTotalAttentionCount);
            }

            return result;
        }

        #region 处理第三方授权公众号相关

        /// <summary>
        /// 获取当前在线的公众号授权信息
        /// </summary>
        /// <param name="domainId"></param>
        /// <returns></returns>
        public AuthorizerEntity GetOnlineAuthorizer(Guid domainId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domain", domainId));

            List<AuthorizerEntity> list = _dataBase.Select<AuthorizerEntity>(
                "SELECT * FROM [Authorizer] WHERE [Domain] = @domain AND [Online] = 1 ORDER BY [AuthorizationTime] DESC",
                parameterList);

            if (list.Count == 0)
            {
                return null;
            }
            else if (list.Count > 1)
            {
                _log.Write("在线的授权公众号多于1个", domainId.ToString(), TraceEventType.Warning);
                return list[0];
            }
            else
            {
                return list[0];
            }
        }

        public AuthorizerPayConfig GetAuthorizerPayConfig(Guid domainId, string appId)
        {
            AuthorizerPayConfig config = new AuthorizerPayConfig();
            config.Domain = domainId;
            config.AppId = appId;

            if (_dataBase.Fill<AuthorizerPayConfig>(config))
            {
                return config;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取所有已解除授权的公众号信息
        /// </summary>
        /// <param name="domainId"></param>
        /// <returns></returns>
        public List<AuthorizerEntity> GetUndockingAuthorizerList(Guid domainId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domain", domainId));

            List<AuthorizerEntity> list = _dataBase.Select<AuthorizerEntity>(
                "SELECT * FROM [Authorizer] WHERE [Domain] = @domain AND [Online] = 0 ORDER BY [AuthorizationTime] DESC",
                parameterList);

            return list;

        }

        #endregion

        /// <summary>
        /// 获取一个全局流水号，跨Domain，APP的
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public int? GetSerialNumber(string module)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@module", module));

            int serialNumber = 0;
            _dataBase.ExecuteScalar<int>(CommandType.StoredProcedure, "GetSerialNumber",
                parameterList, (scalarValue) => { serialNumber = scalarValue; });

            if (serialNumber != 0)
                return serialNumber;
            else
                return null;
        }

        /// <summary>
        /// 获取加入了日期和随机数的流水号，跨Domain，APP的
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public string GetRandomSerialNumber(string module)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@module", module));

            string serialNumber = null;
            _dataBase.ExecuteScalar<string>(CommandType.StoredProcedure, "GetRandomSerialNumber",
                parameterList, (scalarString) => { serialNumber = scalarString; });

            return serialNumber;
        }

        public int GetSort(Guid domainId, string table)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));

            object objSort = _dataBase.ExecuteScalar(
                "SELECT TOP 1 [Sort] FROM [" + table + "] WHERE [Domain] = @domainId ORDER BY [Sort] DESC",
                parameterList);

            if (objSort == null || objSort == DBNull.Value)
                return 0;

            return int.Parse(objSort.ToString()) + 1;
        }

        public void SwapSort(Guid id1, Guid id2, string table)
        {
            SortDigest sort1 = new SortDigest();
            sort1.Id = id1;
            _dataBase.Fill<SortDigest>(sort1, table);

            SortDigest sort2 = new SortDigest();
            sort2.Id = id2;
            _dataBase.Fill<SortDigest>(sort2, table);

            int swap = sort1.Sort;
            sort1.Sort = sort2.Sort;
            sort2.Sort = swap;

            _dataBase.Update(sort1, table);
            _dataBase.Update(sort2, table);
        }
    }
}
