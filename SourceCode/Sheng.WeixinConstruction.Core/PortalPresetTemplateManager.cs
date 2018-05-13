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
using Linkup.DataRelationalMapping;
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
    /// <summary>
    /// 预置微主页模版管理 
    /// </summary>
    public class PortalPresetTemplateManager
    {
        private static readonly PortalPresetTemplateManager _instance = new PortalPresetTemplateManager();
        public static PortalPresetTemplateManager Instance
        {
            get { return _instance; }
        }

        private static DomainManager _domainManager = DomainManager.Instance;

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private PortalPresetTemplateManager()
        {

        }

        /// <summary>
        /// 模版摘要列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetItemListResult GetTemplateDigestList(GetItemListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetPortalPresetTemplateDigestList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetTemplateDigestList(args);
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

        public PortalPresetTemplateEntity GetTemplate(Guid id)
        {
            PortalPresetTemplateEntity template = new PortalPresetTemplateEntity();
            template.Id = id;

            if (_dataBase.Fill<PortalPresetTemplateEntity>(template))
                return template;
            else
                return null;
        }

        public void CreateTemplate(PortalPresetTemplateEntity template)
        {
            if (template == null)
            {
                Debug.Assert(false, "template 为空");
                return;
            }

            template.CreateTime = DateTime.Now;
            template.UpdateTime = DateTime.Now;
            _dataBase.Insert(template);
        }

        public void UpdateTemplate(PortalPresetTemplateEntity template)
        {
            if (template == null)
            {
                Debug.Assert(false, "template 为空");
                return;
            }

            template.UpdateTime = DateTime.Now;
            _dataBase.Update(template);
        }

        public void RemoveTemplate(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("DELETE FROM [PortalPresetTemplate] WHERE [Id] = @id", parameterList);
        }
    }
}
