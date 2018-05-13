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
using Newtonsoft.Json;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class ScenicQRCodeManager
    {
        private static readonly ScenicQRCodeManager _instance = new ScenicQRCodeManager();
        public static ScenicQRCodeManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;
        private LogService _log = LogService.Instance;
        private FileService _fileService = FileService.Instance;

        private ScenicQRCodeManager()
        {

        }

        public NormalResult Create(DomainContext domainContext, ScenicQRCodeEntity scenicQRCode)
        {
            if (scenicQRCode == null)
            {
                Debug.Assert(false, "scenicQRCode 为空");
                return new NormalResult("参数错误");
            }

            //if (domainContext.Domain.Type == EnumDomainType.Free)
            //{
            //    //最大数量不允许超过10个
            //    if (GetTotalCount(domainContext) >= 5)
            //    {
            //        return new NormalResult("免费帐户创建场景二维码数量最多不可超过5个。");
            //    }
            //}

            WeixinCreateQRCodeArgs createArgs = new WeixinCreateQRCodeArgs();
            createArgs.ActionInfo.Scene.SceneId = scenicQRCode.Id.ToString();
            RequestApiResult<WeixinCreateQRCodeResult> createResult = QRCodeApiWrapper.Create(domainContext, createArgs);
            if (createResult.Success == false)
            {
                return new NormalResult(createResult.Message);
            }

            scenicQRCode.Ticket = createResult.ApiResult.Ticket;
            scenicQRCode.Url = createResult.ApiResult.Url;

            //下载二维码图片
            FileDownloadAgentArgs downloadAgentArgs = new FileDownloadAgentArgs();
            downloadAgentArgs.Domain = domainContext.Domain.Id;
            downloadAgentArgs.Url = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" +
                System.Web.HttpUtility.UrlEncode(createResult.ApiResult.Ticket);
            FileDownloadAgentResult downloadAgentResult = _fileService.DownloadAgent(downloadAgentArgs);
            if (downloadAgentResult.Success)
            {
                _log.Write("下载二维码返回", JsonConvert.SerializeObject(downloadAgentResult), TraceEventType.Verbose);
                scenicQRCode.QRCodeImageUrl = _fileService.FileServiceUri + downloadAgentResult.OutputFile;
            }
            else
            {
                return new NormalResult(downloadAgentResult.Message);
            }

            _dataBase.Insert(scenicQRCode);

            return new NormalResult();
        }

        public NormalResult Update(ScenicQRCodeEntity scenicQRCode)
        {
            if (scenicQRCode == null)
            {
                Debug.Assert(false, "scenicQRCode 为空");
                return new NormalResult("参数错误");
            }

            SqlStructureBuild sqlBuild = new SqlStructureBuild();
            sqlBuild.Table = "ScenicQRCode";
            sqlBuild.Type = SqlExpressionType.Update;
            sqlBuild.AddParameter("Id", scenicQRCode.Id, true);
            sqlBuild.AddParameter("Name", scenicQRCode.Name);
            sqlBuild.AddParameter("Remark", scenicQRCode.Remark);
            _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());

            return new NormalResult();
        }

        public void Remove(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("DELETE FROM [ScenicQRCode] WHERE [Id] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [ScenicQRCodeLandingLog] WHERE [QRCodeId] = @id", parameterList);
        }

        public ScenicQRCodeEntity GetScenicQRCode(Guid id)
        {
            ScenicQRCodeEntity scenicQRCodeEntity = new ScenicQRCodeEntity();
            scenicQRCodeEntity.Id = id;

            if (_dataBase.Fill<ScenicQRCodeEntity>(scenicQRCodeEntity))
                return scenicQRCodeEntity;
            else
                return null;
        }

        public GetItemListResult GetScenicQRCodeList(GetScenicQRCodeListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@name", args.Name));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetScenicQRCodeList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetScenicQRCodeList(args);
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

        public void IncrementLanding(Guid id, ScenicQRCodeLandingLogEntity log)
        {
            if (id == Guid.Empty || log == null)
                return;

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("UPDATE [ScenicQRCode] SET [LandingCount] = [LandingCount] + 1 WHERE [Id] = @id", parameterList);

            _dataBase.Insert(log);
        }

        public void IncrementAttentionPerson(Guid id, ScenicQRCodeLandingLogEntity log)
        {
            if (id == Guid.Empty || log == null)
                return;

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("UPDATE [ScenicQRCode] SET [LandingCount] = [LandingCount] + 1,[AttentionPersonCount] = [AttentionPersonCount] + 1 WHERE [Id] = @id", parameterList);

            _dataBase.Insert(log);
        }

        public int GetTotalCount(DomainContext domainContext)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainContext.Domain.Id));
            parameterList.Add(new CommandParameter("@appId", domainContext.AppId));

            int intCount = 0;
            _dataBase.ExecuteScalar<int>(
                "SELECT Count(1) FROM [ScenicQRCode] WHERE [Domain] = @domainId AND [AppId] = @appId",
                parameterList, (scalarValue) => { intCount = scalarValue; });

            return intCount;
        }
    }
}
