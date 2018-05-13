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
    public class CustomFormManager
    {
        private static readonly CustomFormManager _instance = new CustomFormManager();
        public static CustomFormManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private CustomFormManager()
        {

        }

        public void CreateCustomForm(CustomFormEntity customForm)
        {
            if (customForm == null)
            {
                Debug.Assert(false, "customForm 为空");
                return;
            }

            customForm.CreateTime = DateTime.Now;
            _dataBase.Insert(customForm);
        }

        public void UpdateCustomForm(CustomFormEntity customForm)
        {
            if (customForm == null)
            {
                Debug.Assert(false, "customForm 为空");
                return;
            }

            _dataBase.Update(customForm);
        }

        public void RemoveCustomForm(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("DELETE FROM [CustomForm] WHERE [Id] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [CustomFormContent] WHERE [Form] = @id", parameterList);
        }

        public CustomFormEntity GetCustomForm(Guid id)
        {
            CustomFormEntity customForm = new CustomFormEntity();
            customForm.Id = id;

            if (_dataBase.Fill<CustomFormEntity>(customForm))
                return customForm;
            else
                return null;
        }

        public GetItemListResult GetCustomFormList(GetCustomFormListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@name", args.Name));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCustomFormList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetCustomFormList(args);
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

        public void SaveCustomFormContent(CustomFormContentEntity contentArgs,SaveCustomFormContentArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", args.MemberId));
            parameterList.Add(new CommandParameter("@name", args.Name));
            parameterList.Add(new CommandParameter("@birthday", args.Birthday));
            parameterList.Add(new CommandParameter("@mobilePhone", args.MobilePhone));
            parameterList.Add(new CommandParameter("@email", args.Email));

            _dataBase.ExecuteNonQuery(
                "UPDATE [Member] SET [Name] = @name,[Birthday] = @birthday,[MobilePhone] = @mobilePhone,[Email]=@email WHERE [Id] = @id",
                parameterList);

            RemoveCustomFormContent(args.FormId,contentArgs.Id);
            CreateCustomFormContent(contentArgs);
        }

        public void CreateCustomFormContent(CustomFormContentEntity customFormContent)
        {
            if (customFormContent == null)
            {
                Debug.Assert(false, "customFormContent 为空");
                return;
            }

            customFormContent.FillinTime = DateTime.Now;
            _dataBase.Insert(customFormContent);

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@formId", customFormContent.Form));

            _dataBase.ExecuteNonQuery("UPDATE [CustomForm] SET [FillinCount] = [FillinCount] +1 WHERE [Id] = @formId", parameterList);
            
        }

        public void UpdateCustomFormContent(CustomFormContentEntity customFormContent)
        {
            if (customFormContent == null)
            {
                Debug.Assert(false, "customFormContent 为空");
                return;
            }

            _dataBase.Update(customFormContent);
        }

        public void RemoveCustomFormContent(Guid formId,Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));
            parameterList.Add(new CommandParameter("@formId", formId));

            _dataBase.ExecuteNonQuery("DELETE FROM [CustomFormContent] WHERE [Id] = @id", parameterList);
            _dataBase.ExecuteNonQuery("UPDATE [CustomForm] SET [FillinCount] = [FillinCount] -1 WHERE [Id] = @formId", parameterList);
        }

        public CustomFormContentEntity GetCustomFormContent(Guid id)
        {
            CustomFormContentEntity customFormContent = new CustomFormContentEntity();
            customFormContent.Id = id;

            if (_dataBase.Fill<CustomFormContentEntity>(customFormContent))
                return customFormContent;
            else
                return null;
        }

        public CustomFormContentEntity GetCustomFormContentByMember(Guid formId, string memberOpenId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@formId", formId));
            parameterList.Add(new CommandParameter("@memberOpenId", memberOpenId));

            List<CustomFormContentEntity> contentList = _dataBase.Select<CustomFormContentEntity>(
                "SELECT * FROM [CustomFormContent] WHERE [Form] = @formId  AND [MemberOpenId] = @memberOpenId",
                parameterList);

            if (contentList.Count == 0)
                return null;
            else
                return contentList[0];
        }

        public GetItemListResult GetCustomFormContentList(GetCustomFormContentListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@form", args.Form));
            parameterList.Add(new CommandParameter("@name", args.Name));
            parameterList.Add(new CommandParameter("@nickName", args.NickName));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCustomFormContentList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetCustomFormContentList(args);
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


    }
}
