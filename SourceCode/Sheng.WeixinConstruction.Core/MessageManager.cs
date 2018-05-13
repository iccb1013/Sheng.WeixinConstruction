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
using Sheng.WeixinConstruction.ApiContract;
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
    public class MessageManager
    {
        private static readonly MessageManager _instance = new MessageManager();
        public static MessageManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private FileService _fileService = FileService.Instance;
        private LogService _log = LogService.Instance;

        private MessageManager()
        {

        }

        /// <summary>
        /// 记录消息
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="message"></param>
        public void AddMessage(Guid domainId, string appId, MessageEntity message)
        {
            if (message == null)
            {
                Debug.Assert(false, "message == null");
                return;
            }

            message.Domain = domainId;
            message.AppId = appId;

            _dataBase.Insert(message);
        }

        public void MessageFile(FileDownloadQueueWithMediaIdResult args)
        {
            Guid messageId = Guid.Parse(args.Tag);

            SqlStructureBuild sqlBuild = new SqlStructureBuild();
            sqlBuild.Table = "Message";
            sqlBuild.Type = SqlExpressionType.Update;
            sqlBuild.AddParameter("Id", messageId, true);
            sqlBuild.AddParameter("ContentType", args.ContentType);
            sqlBuild.AddParameter("FileUrl", _fileService.FileServiceUri + args.OutputFile);
            sqlBuild.AddParameter("FileLength", args.FileLength);
            _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());
        }

        public void MessageThumbFile(FileDownloadQueueWithMediaIdResult args)
        {
            Guid messageId = Guid.Parse(args.Tag);

            SqlStructureBuild sqlBuild = new SqlStructureBuild();
            sqlBuild.Table = "Message";
            sqlBuild.Type = SqlExpressionType.Update;
            sqlBuild.AddParameter("Id", messageId, true);
            sqlBuild.AddParameter("ThumbContentType", args.ContentType);
            sqlBuild.AddParameter("ThumbUrl", _fileService.FileServiceUri + args.OutputFile);
            sqlBuild.AddParameter("ThumbLength", args.FileLength);
            _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());
        }

        public void ReplyMessage()
        {

        }

        public GetItemListResult GetMessageList(GetMessageListArgs args)
        {
            string receivingMessageType = null;
            if (args.ReceivingMessageType.HasValue)
            {
                receivingMessageType = EnumHelper.GetEnumMemberValue(args.ReceivingMessageType);
            }

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@receivingMessageType", receivingMessageType));
            parameterList.Add(new CommandParameter("@content", args.Content));
            parameterList.Add(new CommandParameter("@memberOpenId", args.MemberOpenId));
            parameterList.Add(new CommandParameter("@memberNickName", args.MemberNickName));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetMessageList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetMessageList(args);
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
