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
    public class AdvertisingManager
    {
        private static readonly AdvertisingManager _instance = new AdvertisingManager();
        public static AdvertisingManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private static MemberManager _memberManager = MemberManager.Instance;
        private static ShareManager _shareManager = ShareManager.Instance;

        private AdvertisingManager()
        {

        }

        public void CreateAdvertising(AdvertisingEntity advertisingEntity)
        {
            if (advertisingEntity == null)
            {
                Debug.Assert(false, "advertisingEntity 为空");
                return;
            }

            advertisingEntity.CreateTime = DateTime.Now;
            advertisingEntity.UpdateTime = DateTime.Now;
            _dataBase.Insert(advertisingEntity);
        }

        public void UpdateAdvertising(AdvertisingEntity advertisingEntity)
        {
            if (advertisingEntity == null)
            {
                Debug.Assert(false, "advertisingEntity 为空");
                return;
            }

            _dataBase.Update(advertisingEntity, null, "Clicks");
        }

        public void RemoveAdvertising(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("DELETE FROM [Advertising] WHERE [Id] = @id", parameterList);
        }

        public AdvertisingEntity GetAdvertising(Guid id)
        {
            AdvertisingEntity advertising = new AdvertisingEntity();
            advertising.Id = id;

            if (_dataBase.Fill<AdvertisingEntity>(advertising))
                return advertising;
            else
                return null;
        }

        public GetItemListResult GetAdvertisingList(GetGetAdvertisingListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@name", args.Name));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetAdvertisingList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetAdvertisingList(args);
            }

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            int totalCount = int.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            result.TotalCount = totalCount;
            result.TotalPage = totalCount / args.PageSize;
            if (totalCount % args.PageSize > 0)
            {
                result.TotalPage++;
            }
            result.Page = args.Page;

            return result;
        }

        public void AdvertisingClick(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("UPDATE [Advertising] SET [Clicks] = [Clicks] + 1 WHERE [Id] = @id",
                parameterList);
        }
    }
}
