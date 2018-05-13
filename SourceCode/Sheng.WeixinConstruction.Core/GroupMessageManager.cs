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
using Linkup.DataRelationalMapping;
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
    public class GroupMessageManager
    {
        private static readonly GroupMessageManager _instance = new GroupMessageManager();
        public static GroupMessageManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly MaterialManager _materialManager = MaterialManager.Instance;
        private static readonly MemberGroupManager _memberGroupManager = MemberGroupManager.Instance;

        private GroupMessageManager()
        {

        }

        public string SendGroupMessage(DomainContext domainContext, SendGroupMessageArgs args)
        {
            if (args == null)
            {
                return "参数错误。";
            }

            GroupMessageEntity groupMessage = new GroupMessageEntity();
            groupMessage.AppId = domainContext.AppId;

            if (args.GroupId >= 0)
            {
                groupMessage.IsToAll = false;
                groupMessage.GroupId = args.GroupId;
            }
            else
            {
                groupMessage.IsToAll = true;
            }

            groupMessage.Type = args.Type;
            groupMessage.Content = args.Content;
            groupMessage.Url = args.Url;
            groupMessage.Name = args.Name;
            groupMessage.MediaId = args.MediaId;
            groupMessage.ArticleId = args.ArticleId;

            WeixinGroupMessageSendAllArgs apiArgs = null;
            switch (groupMessage.Type)
            {
                case EnumGroupMessageType.Text:
                    WeixinGroupMessageSendAllArgs_Text textMsg = new WeixinGroupMessageSendAllArgs_Text();
                    textMsg.Content.Content = groupMessage.Content;
                    apiArgs = textMsg;
                    break;
                case EnumGroupMessageType.Image:
                    WeixinGroupMessageSendAllArgs_Image imageMsg = new WeixinGroupMessageSendAllArgs_Image();
                    imageMsg.Image.MediaId = groupMessage.MediaId;
                    apiArgs = imageMsg;
                    break;
                case EnumGroupMessageType.Article:
                    string mediaId = _materialManager.GetArticleMaterialMediaId(args.ArticleId.Value);
                    if (String.IsNullOrEmpty(mediaId) == false)
                    {
                        WeixinGroupMessageSendAllArgs_Mpnews articleMsg = new WeixinGroupMessageSendAllArgs_Mpnews();
                        articleMsg.Mpnews.MediaId = mediaId;
                        apiArgs = articleMsg;
                    }
                    break;
                default:
                    break;
            }

            if (apiArgs == null)
            {
                return "参数错误。";
            }

            //先往微信后台添加，成功后写数据库
            apiArgs.Filter.IsToAll = groupMessage.IsToAll;
            if (groupMessage.GroupId.HasValue && groupMessage.GroupId >= 0)
            {
                apiArgs.Filter.GroupId = groupMessage.GroupId.ToString();
            }
            // apiArgs.MsgType
            RequestApiResult<WeixinGroupMessageSendAllResult> sendAllResult =
                GroupMessageApiWrapper.SendAll(domainContext, apiArgs);
            if (sendAllResult.Success == false)
            {
                return sendAllResult.Message;
            }

            groupMessage.MsgId = sendAllResult.ApiResult.MsgId;
            groupMessage.MsgDataId = sendAllResult.ApiResult.MsgDataId;
            groupMessage.Domain = domainContext.Domain.Id;
            _dataBase.Insert(groupMessage);

            return null;
        }

        public void Finish(Guid domainId, string appId, ReceivingXMLMessage_GroupMessageFinishEvent message)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domain", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@msgId", message.MsgId));
            parameterList.Add(new CommandParameter("@status", message.Status));
            parameterList.Add(new CommandParameter("@totalCount", message.TotalCount));
            parameterList.Add(new CommandParameter("@filterCount", message.FilterCount));
            parameterList.Add(new CommandParameter("@sentCount", message.SentCount));
            parameterList.Add(new CommandParameter("@errorCount", message.ErrorCount));

            _dataBase.ExecuteNonQuery(
                "UPDATE [GroupMessage] SET [Status] = @status, [TotalCount] = @totalCount,[FilterCount] = @filterCount,[SentCount] = @sentCount, [ErrorCount] = @errorCount WHERE [Domain] = @domain AND [AppId] = @appId AND [MsgId] = @msgId",
                parameterList);

        }

        public GetItemListResult GetSentGroupMessageList(Guid domainId, string appId, GetItemListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetSentGroupMessageList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetSentGroupMessageList(domainId, appId, args);
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

        public GroupMessageEntity GetGroupMessage(Guid id)
        {
            GroupMessageEntity groupMessage = new GroupMessageEntity();
            groupMessage.Id = id;

            if (_dataBase.Fill<GroupMessageEntity>(groupMessage))
            {
                if (groupMessage.GroupId.HasValue)
                {
                    groupMessage.FilterOption = "分组：";
                    MemberGroupEntity memberGroup = _memberGroupManager.GetMemberGroup(groupMessage.Domain,groupMessage.AppId, groupMessage.GroupId.Value);
                    if (memberGroup != null)
                    {
                        groupMessage.FilterOption += memberGroup.Name;
                    }
                }
                else
                {
                    groupMessage.FilterOption = "全部";
                }
                return groupMessage;
            }
            else
                return null;
        }
    }
}
