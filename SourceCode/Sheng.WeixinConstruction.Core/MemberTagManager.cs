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
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class MemberTagManager
    {
        private static readonly MemberTagManager _instance = new MemberTagManager();
        public static MemberTagManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private static LogService _log = LogService.Instance;

        private MemberTagManager()
        {

        }

        /// <summary>
        /// 刷新指定域下面分组信息，将调用微信API获取最新分组信息
        /// </summary>
        /// <param name="domainContext"></param>
        public void SyncTagList(DomainContext domainContext)
        {
            RequestApiResult<WeixinGetTagListResult> result = TagsApiWrapper.GetTagList(domainContext);
            if (result.Success)
            {
                List<CommandParameter> parameterList = new List<CommandParameter>();
                parameterList.Add(new CommandParameter("@domain", domainContext.Domain.Id));
                parameterList.Add(new CommandParameter("@appId", domainContext.AppId));

                _dataBase.ExecuteNonQuery("DELETE FROM [MemberTag] WHERE [Domain] = @domain AND [AppId] = @appId",
                   parameterList);

                foreach (WeixinGetTagListResult_Tag item in result.ApiResult.TagList)
                {
                    MemberTagEntity tag = new MemberTagEntity();
                    tag.TagId = item.Id;
                    tag.Name = item.Name;
                    tag.Domain = domainContext.Domain.Id;
                    tag.AppId = domainContext.AppId;
                    _dataBase.Insert(tag);
                }

                //过滤用户，将已不存在的分组置为0
            }
            else
            {
                _log.Write("RefreshTagList 失败", result.Message, TraceEventType.Warning);
            }
        }

        public List<MemberTagEntity> GetMemberTagList(Guid domainId, string appId)
        {
            List<AttachedWhereItem> attachedWhere = new List<AttachedWhereItem>();
            attachedWhere.Add(new AttachedWhereItem("Domain", domainId));
            attachedWhere.Add(new AttachedWhereItem("AppId", appId));

            List<MemberTagEntity> list = _dataBase.Select<MemberTagEntity>(attachedWhere);
            return list;
        }

        public MemberTagEntity GetMemberTag(Guid id)
        {
            MemberTagEntity memberTag = new MemberTagEntity();
            memberTag.Id = id;

            if (_dataBase.Fill<MemberTagEntity>(memberTag))
                return memberTag;
            else
                return null;
        }

        public MemberTagEntity GetMemberTag(Guid domainId, string appId, int tagId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domain", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@tagId", tagId));

            List<MemberTagEntity> memberTagList = _dataBase.Select<MemberTagEntity>(
                "SELECT * FROM [MemberTag] WHERE [Domain] = @domain AND [AppID] = @appId AND [TagId] = @tagId", parameterList);
            if (memberTagList.Count == 0)
                return null;
            else
                return memberTagList[0];
        }

        public NormalResult CreateMemberTag(DomainContext domainContext, MemberTagEntity tag)
        {
            NormalResult result = new NormalResult(false);

            if (tag == null)
            {
                result.Message = "参数错误。";
                return result;
            }

            //先往微信后台添加，成功后写数据库
            WeixinCreateTagArgs apiArgs = new WeixinCreateTagArgs();
            apiArgs.Tag.Name = tag.Name;
            RequestApiResult<WeixinTag> createResult = TagsApiWrapper.Create(domainContext, apiArgs);
            if (createResult.Success == false)
            {
                result.Message = createResult.Message;
                return result;
            }

            tag.Domain = domainContext.Domain.Id;
            tag.TagId = createResult.ApiResult.Tag.Id;
            _dataBase.Insert(tag);

            result.Success = true;
            return result;
        }

        public NormalResult UpdateMemberTag(DomainContext domainContext, MemberTagEntity tag)
        {
            NormalResult result = new NormalResult(false);

            if (tag == null)
            {
                result.Message = "参数错误。";
                return result;
            }

            //先往微信后台更新，成功后写数据库
            WeixinTag apiArgs = new WeixinTag();
            apiArgs.Tag.Id = tag.TagId;
            apiArgs.Tag.Name = tag.Name;
            RequestApiResult updateResult = TagsApiWrapper.Update(domainContext, apiArgs);
            if (updateResult.Success == false)
            {
                result.Message = updateResult.Message;
                return result;
            }

            _dataBase.Update(tag);

            result.Success = true;
            return result;
        }

        public NormalResult RemoveMemberTag(DomainContext domainContext, Guid id)
        {
            NormalResult result = new NormalResult(false);

            MemberTagEntity memberTag = GetMemberTag(id);
            if (memberTag == null)
            {
                result.Message = "分组不存在。";
                return result;
            }

            //先往微信后台更新，成功后写数据库
            RequestApiResult removeResult = TagsApiWrapper.Remove(domainContext, memberTag.TagId);
            if (removeResult.Success == false)
            {
                result.Message = removeResult.Message;
                return result;
            }

            //移除此标签下的用户的这个标签
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domain", domainContext.Domain.Id));
            parameterList.Add(new CommandParameter("@appId", domainContext.AppId));
            parameterList.Add(new CommandParameter("@tagId", memberTag.TagId.ToString() + ","));

            _dataBase.ExecuteNonQuery(
                "UPDATE [Member] SET [TagList] =  REPLACE([TagList],@tagId,'') WHERE [Domain] = @domain AND [AppId] = @appId AND CHARINDEX(@tagId,[TagList]) > 0",
                parameterList);

            _dataBase.Remove(memberTag);

            result.Success = true;
            return result;
        }

        public NormalResult BatchTagging(DomainContext domainContext, MemberBatchTaggingArgs args)
        {
            NormalResult result = new NormalResult(false);

            if (args == null || args.OpenIdList == null || args.OpenIdList.Length == 0)
            {
                result.Message = "没有指定要更新的会员。";
                return result;
            }

            WeixinTagBatchTaggingArgs apiArgs = new WeixinTagBatchTaggingArgs();
            apiArgs.TagId = args.TagId;
            apiArgs.Data = new WeixinTagBatchTaggingArgs_Data();
            apiArgs.Data.OpenIdList = args.OpenIdList;
            RequestApiResult updateResult = TagsApiWrapper.BatchTagging(domainContext, apiArgs);
            if (updateResult.Success == false)
            {
                result.Message = updateResult.Message;
                return result;
            }

            //更新数据库MEMBER表字段
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domain", domainContext.Domain.Id));
            parameterList.Add(new CommandParameter("@appId", domainContext.AppId));
            parameterList.Add(new CommandParameter("@tagId", args.TagId + ","));

            StringBuilder sqlWhere = new StringBuilder();
            for (int i = 0; i < args.OpenIdList.Length; i++)
            {
                sqlWhere.Append("@openId" + i);

                if (i < args.OpenIdList.Length - 1)
                {
                    sqlWhere.Append(",");
                }

                parameterList.Add(new CommandParameter("@openId" + i, args.OpenIdList[i]));
            }

            _dataBase.ExecuteNonQuery(
               "UPDATE [Member] SET [TagList] =  [TagList] + @tagId WHERE [Domain] = @domain AND [AppId] = @appId AND CHARINDEX(@tagId,[TagList]) = 0 AND [OpenId] IN (" + sqlWhere.ToString() + ")",
               parameterList);

            result.Success = true;
            return result;
        }

        public NormalResult BatchUntagging(DomainContext domainContext, MemberBatchTaggingArgs args)
        {
            NormalResult result = new NormalResult(false);

            if (args == null || args.OpenIdList == null || args.OpenIdList.Length == 0)
            {
                result.Message = "没有指定要更新的会员。";
                return result;
            }

            WeixinTagBatchTaggingArgs apiArgs = new WeixinTagBatchTaggingArgs();
            apiArgs.TagId = args.TagId;
            apiArgs.Data = new WeixinTagBatchTaggingArgs_Data();
            apiArgs.Data.OpenIdList = args.OpenIdList;
            RequestApiResult updateResult = TagsApiWrapper.BatchUntagging(domainContext, apiArgs);
            if (updateResult.Success == false)
            {
                result.Message = updateResult.Message;
                return result;
            }

            //更新数据库MEMBER表字段
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domain", domainContext.Domain.Id));
            parameterList.Add(new CommandParameter("@appId", domainContext.AppId));
            parameterList.Add(new CommandParameter("@tagId", args.TagId + ","));

            StringBuilder sqlWhere = new StringBuilder();
            for (int i = 0; i < args.OpenIdList.Length; i++)
            {
                sqlWhere.Append("@openId" + i);

                if (i < args.OpenIdList.Length - 1)
                {
                    sqlWhere.Append(",");
                }

                parameterList.Add(new CommandParameter("@openId" + i, args.OpenIdList[i]));
            }

            _dataBase.ExecuteNonQuery(
               "UPDATE [Member] SET [TagList] = REPLACE([TagList],@tagId,'') WHERE [Domain] = @domain AND [AppId] = @appId AND CHARINDEX(@tagId,[TagList]) > 0 AND [OpenId] IN (" + sqlWhere.ToString() + ")",
               parameterList);

            result.Success = true;
            return result;
        }

    }
}
