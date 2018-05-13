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
    public class InformationManager
    {
        private static readonly InformationManager _instance = new InformationManager();
        public static InformationManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private InformationManager()
        {

        }

        public void CreateInformation(InformationEntity information)
        {
            if (information == null)
            {
                Debug.Assert(false, "information 为空");
                return;
            }

            information.CreateTime = DateTime.Now;
            _dataBase.Insert(information);
        }

        public void UpdateInformation(InformationEntity information)
        {
            if (information == null)
            {
                Debug.Assert(false, "information 为空");
                return;
            }

            _dataBase.Update(information);
        }

        public void RemoveInformation(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("DELETE FROM [Information] WHERE [Id] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [InformationCategory] WHERE [Information] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [InformationItem] WHERE [Information] = @id", parameterList);
        }

        public InformationEntity GetInformation(Guid id)
        {
            InformationEntity information = new InformationEntity();
            information.Id = id;

            if (_dataBase.Fill<InformationEntity>(information))
                return information;
            else
                return null;
        }

        public GetItemListResult GetInformationList(GetItemListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetInformationList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetInformationList(args);
            }

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            int totalCount = int.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            result.TotalPage = totalCount / args.PageSize;
            if (totalCount % args.PageSize > 0)
            {
                result.TotalPage++;
            }
            result.Page = args.Page;

            return result;
        }

        public void CreateCategory(InformationCategoryEntity informationCategory)
        {
            if (informationCategory == null)
            {
                Debug.Assert(false, "informationCategory 为空");
                return;
            }

            informationCategory.CreateTime = DateTime.Now;
            _dataBase.Insert(informationCategory);
        }

        public void UpdateCategory(InformationCategoryEntity informationCategory)
        {
            if (informationCategory == null)
            {
                Debug.Assert(false, "informationCategory 为空");
                return;
            }

            _dataBase.Update(informationCategory);
        }

        public void RemoveCategory(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("DELETE FROM [InformationCategory] WHERE [Id] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [InformationItem] WHERE [Category] = @id", parameterList);
        }

        public void BatchRemoveCategory(List<String> idList)
        {
            if (idList == null)
            {
                Debug.Assert(false, "idList 为空");
                return;
            }

            foreach (string id in idList)
            {
                RemoveCategory(Guid.Parse(id));
            }
        }

        public InformationCategoryEntity GetCategory(Guid id)
        {
            InformationCategoryEntity category = new InformationCategoryEntity();
            category.Id = id;

            if (_dataBase.Fill<InformationCategoryEntity>(category))
                return category;
            else
                return null;
        }

        public GetItemListResult GetCategoryList(GetCategoryListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@informationId", args.InformationId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetInformationCategoryList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetCategoryList(args);
            }

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            int totalCount = int.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            result.TotalPage = totalCount / args.PageSize;
            if (totalCount % args.PageSize > 0)
            {
                result.TotalPage++;
            }
            result.Page = args.Page;

            return result;
        }

        public void CreateInformationItem(InformationItemEntity informationItem)
        {
            if (informationItem == null)
            {
                Debug.Assert(false, "informationItem 为空");
                return;
            }

            informationItem.CreateTime = DateTime.Now;
            _dataBase.Insert(informationItem);
        }

        public void UpdateInformationItem(InformationItemEntity informationItem)
        {
            if (informationItem == null)
            {
                Debug.Assert(false, "informationItem 为空");
                return;
            }

            _dataBase.Update(informationItem);
        }

        public void RemoveInformationItem(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("DELETE FROM [InformationItem] WHERE [Id] = @id", parameterList);
        }

        public void BatchRemoveInformationItem(List<String> idList)
        {
            if (idList == null)
            {
                Debug.Assert(false, "idList 为空");
                return;
            }

            foreach (string id in idList)
            {
                RemoveInformationItem(Guid.Parse(id));
            }
        }

        public InformationItemEntity GetInformationItem(Guid id)
        {
            InformationItemEntity item = new InformationItemEntity();
            item.Id = id;

            if (_dataBase.Fill<InformationItemEntity>(item))
                return item;
            else
                return null;
        }

        public GetItemListResult GetInformationItemList(GetInformationItemListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@categoryId", args.CategoryId));
            parameterList.Add(new CommandParameter("@keyword", args.Keyword));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetInformationItemList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetInformationItemList(args);
            }

            GetItemListResult result = new GetItemListResult();

            result.ItemList = dsResult.Tables[0];

            int totalCount = int.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            result.TotalPage = totalCount / args.PageSize;
            if (totalCount % args.PageSize > 0)
            {
                result.TotalPage++;
            }
            result.Page = args.Page;

            return result;
        }
    }
}
