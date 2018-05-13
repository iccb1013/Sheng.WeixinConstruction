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
    public class CampaignManager_PictureVote
    {
        private CampaignManager _campaignManager;

        public CampaignManager_PictureVote(CampaignManager campaignManager)
        {
            _campaignManager = campaignManager;
        }

        /// <summary>
        /// 获取最美投票活动列表
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="appId"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetItemListResult GetCampaign_PictureVoteList(Guid domainId, string appId, GetCampaign_PictureVoteListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@status", args.Status));

            DataSet dsResult =
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_PictureVoteList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetCampaign_PictureVoteList(domainId, appId, args);
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

        public void CreatePictureVote(CampaignEntity campaign, Campaign_PictureVoteEntity pictureVote)
        {
            if (campaign == null || pictureVote == null)
            {
                Debug.Assert(false, "campaign == null || pictureVote ==null");
                return;
            }

            pictureVote.CampaignId = campaign.Id;
            pictureVote.Domain = campaign.Domain;

            _campaignManager.DataBase.InsertList(campaign, pictureVote);
        }

        public void UpdatePictureVote(CampaignEntity campaign, Campaign_PictureVoteEntity pictureVote)
        {
            if (campaign == null || pictureVote == null)
            {
                Debug.Assert(false, "campaign == null || pictureVote ==null");
                return;
            }

            pictureVote.CampaignId = campaign.Id;
            pictureVote.Domain = campaign.Domain;

            _campaignManager.DataBase.UpdateList(campaign, pictureVote);
        }

        /// <summary>
        /// 删除整个投票活动
        /// </summary>
        /// <param name="campaignId"></param>
        public void RemovePictureVote(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", campaignId));

            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign] WHERE [Id] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_PictureVote] WHERE [CampaignId] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_PictureVoteItem] WHERE [CampaignId] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_PictureVoteLog] WHERE [CampaignId] = @id", parameterList);
        }

        public Campaign_PictureVoteBundle GetPictureVoteBundle(Guid id)
        {
            Campaign_PictureVoteBundle bundle = new Campaign_PictureVoteBundle();

            bundle.Campaign = _campaignManager.GetCampaign(id);
            bundle.PictureVote = GetPictureVote(id);

            return bundle;
        }

        public Campaign_PictureVoteEntity GetPictureVote(Guid campaignId)
        {
            Campaign_PictureVoteEntity campaign = new Campaign_PictureVoteEntity();
            campaign.CampaignId = campaignId;

            if (_campaignManager.DataBase.Fill<Campaign_PictureVoteEntity>(campaign))
                return campaign;
            else
                return null;
        }

        /// <summary>
        /// 创建一个被投票项目
        /// </summary>
        /// <param name="item"></param>
        public EnumCampaignCreatePictureVoteItemResult CreatePictureVoteItem(Campaign_PictureVoteItemEntity args)
        {
            if (args == null)
            {
                Debug.Assert(false, "args == null");
                return EnumCampaignCreatePictureVoteItemResult.Failed;
            }

            //是否允许多发布
            Campaign_PictureVoteEntity campaign_PictureVote = GetPictureVote(args.CampaignId);
            if (campaign_PictureVote == null)
                return EnumCampaignCreatePictureVoteItemResult.Failed;

            //参与通道是否还打开
            if (campaign_PictureVote.AllowedNewItem == false)
                return EnumCampaignCreatePictureVoteItemResult.NewItemClosed;

            //如果是会员发布，判断是否发过了及是否可多发布
            if (args.Member.HasValue && args.Member.Value != Guid.Empty)
            {
                List<CommandParameter> parameterList = new List<CommandParameter>();
                parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));
                parameterList.Add(new CommandParameter("@memberId", args.Member.Value));

                int intCount = 0;
                _campaignManager.DataBase.ExecuteScalar<int>(
                    "SELECT Count(1) FROM [Campaign_PictureVoteItem] WHERE [CampaignId] = @campaignId AND [Member] = @memberId",
                    parameterList, (scalarValue) => { intCount = scalarValue; });

                if (intCount >= campaign_PictureVote.MaxPublishTimes)
                {
                    return EnumCampaignCreatePictureVoteItemResult.AlreadyPublished;
                }
            }

            int? serialNumber = _campaignManager.DomainManager.GetSerialNumber("PictureVote");
            if (serialNumber.HasValue == false)
                return EnumCampaignCreatePictureVoteItemResult.Failed;

            args.SerialNumber = serialNumber.Value.ToString();

            _campaignManager.DataBase.Insert(args);

            return EnumCampaignCreatePictureVoteItemResult.Successful;
        }

        public Campaign_PictureVoteItemEntity GetPictureVoteItem(Guid id)
        {
            Campaign_PictureVoteItemEntity item = new Campaign_PictureVoteItemEntity();
            item.Id = id;
            if (_campaignManager.DataBase.Fill<Campaign_PictureVoteItemEntity>(item))
            {
                if (item.Member.HasValue)
                {
                    MemberEntity member = _campaignManager.MemberManager.GetMember(item.Member.Value);
                    if (member != null)
                        item.MemberEntity = member;
                }
                return item;
            }
            else
                return null;
        }

        public Campaign_PictureVoteItemEntity GetPictureVoteItemByMemberId(Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@memberId", memberId));

            List<Campaign_PictureVoteItemEntity> itemList = _campaignManager.DataBase.Select<Campaign_PictureVoteItemEntity>(
                "SELECT * FROM [Campaign_PictureVoteItem] WHERE [Member] = @memberId", parameterList);

            if (itemList.Count > 0)
                return itemList[0];
            else
                return null;
        }

        //TODO:要把domainId和appId带进去
        public Guid? GetPictureVoteItemIdBySerialNumber(string serialNumber)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@serialNumber", serialNumber));

            Guid id = Guid.Empty;
            if (_campaignManager.DataBase.ExecuteScalar<Guid>(
                 "SELECT [Id] FROM [Campaign_PictureVoteItem] WHERE [SerialNumber] = @serialNumber AND [ApproveStatus] = 1",
                 parameterList, (scalarString) => { id = scalarString; }))
            {
                return id;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 删除投票项目
        /// </summary>
        /// <param name="id"></param>
        public void RemovePictureVoteItem(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_PictureVoteItem] WHERE [Id] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_PictureVoteLog] WHERE [ItemId] = @id", parameterList);
        }

        /// <summary>
        /// 删除投票项目，指定memberId，用户上传者从微信端自己删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="memberId"></param>
        public void RemovePictureVoteItem(Guid id, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            _campaignManager.DataBase.ExecuteNonQuery(
                "DELETE FROM [Campaign_PictureVoteItem] WHERE [Id] = @id AND [Member] = @memberId", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery(
                "DELETE FROM [Campaign_PictureVoteLog] WHERE [ItemId] = @id AND [Member] = @memberId", parameterList);
        }

        /// <summary>
        /// 获取指定会员所发布过的项目列表
        /// </summary>
        /// <param name="campaignId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public List<Campaign_PictureVoteItemEntity> GetMemberPictureVoteItemList(Guid campaignId, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            List<Campaign_PictureVoteItemEntity> itemList = _campaignManager.DataBase.Select<Campaign_PictureVoteItemEntity>(
                "SELECT * FROM [Campaign_PictureVoteItem] WHERE [CampaignId] = @campaignId AND [Member] = @memberId",
                parameterList);

            return itemList;
        }

        public int GetMemberPictureVoteItemCount(Guid campaignId, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            int intStatus = 0;
            _campaignManager.DataBase.ExecuteScalar<int>("SELECT Count(1) FROM [Campaign_PictureVoteItem] WHERE [CampaignId] = @campaignId AND [Member] = @memberId",
                parameterList, (scalarValue) => { intStatus = scalarValue; });
            return intStatus;
        }

        public int GetMemberPictureVoteItemCount(Guid campaignId, Guid memberId, EnumCampaignPictureVoteItemApproveStatus approveStatus)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));
            parameterList.Add(new CommandParameter("@memberId", memberId));
            parameterList.Add(new CommandParameter("@approveStatus", (int)approveStatus));

            int intStatus = 0;
            _campaignManager.DataBase.ExecuteScalar<int>("SELECT Count(1) FROM [Campaign_PictureVoteItem] WHERE [CampaignId] = @campaignId AND [Member] = @memberId AND [ApproveStatus] = @approveStatus",
                parameterList, (scalarValue) => { intStatus = scalarValue; });
            return intStatus;
        }

        /// <summary>
        /// 投票
        /// </summary>
        /// <param name="args"></param>
        public EnumCampaignPictureVoteResult PictureVote(Campaign_PictureVoteArgs args)
        {
            if (args == null)
            {
                Debug.Assert(false, "args == null");
                return EnumCampaignPictureVoteResult.Failed;
            }

            CampaignEntity campaign = _campaignManager.GetCampaign(args.CampaignId);
            if (campaign == null)
                return EnumCampaignPictureVoteResult.Failed;

            //状态是否进行中
            switch (campaign.Status)
            {
                case EnumCampaignStatus.Preparatory:
                    return EnumCampaignPictureVoteResult.NotStarted;
                case EnumCampaignStatus.End:
                    return EnumCampaignPictureVoteResult.AlreadyEnded;
            }

            //判断要投的项目是否已被锁定
            Campaign_PictureVoteItemEntity item = GetPictureVoteItem(args.ItemId);
            if (item == null)
            {
                return EnumCampaignPictureVoteResult.Failed;
            }

            if (item.Lock)
            {
                return EnumCampaignPictureVoteResult.Lock;
            }

            Campaign_PictureVoteEntity campaign_PictureVote = GetPictureVote(args.CampaignId);
            if (campaign_PictureVote == null)
                return EnumCampaignPictureVoteResult.Failed;

            //是否允许非会员投票
            if (campaign_PictureVote.AllowedNoAttentionVote == false && args.Member == null)
                return EnumCampaignPictureVoteResult.OnlyMember;

            //查出当前在此活动下的投票记录
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));
            parameterList.Add(new CommandParameter("@openId", args.OpenId));

            //根据投票方式判断是否按天投
            if (campaign_PictureVote.VoteType == EnumCampaignPictureVoteVoteType.Day)
            {
                List<Campaign_PictureVoteLogEntity> logList = _campaignManager.DataBase.Select<Campaign_PictureVoteLogEntity>(
                    @"SELECT * FROM [Campaign_PictureVoteLog] WHERE [CampaignId] = @campaignId AND [OpenId] = @openId 
                        AND Convert(varchar(10),[VoteTime],120) = Convert(varchar(10),CONVERT(datetime,'" + DateTime.Now.ToShortDateString() + "'),120)",
                    parameterList);

                //是否达到最大投票数
                if (logList.Count >= campaign_PictureVote.MaxVoteTimes)
                {
                    return EnumCampaignPictureVoteResult.OverVoteTimesByDay;
                }

                if (logList.Count > 0)
                {
                    //是否给同一个项目投过票
                    if ((from c in logList where c.ItemId == args.ItemId select c).Count() > 0)
                    {
                        return EnumCampaignPictureVoteResult.VotedByDay;
                    }
                }
            }
            else //NoRepetition
            {
                List<Campaign_PictureVoteLogEntity> logList = _campaignManager.DataBase.Select<Campaign_PictureVoteLogEntity>(
                    "SELECT * FROM [Campaign_PictureVoteLog] WHERE [CampaignId] = @campaignId AND [OpenId] = @openId",
                    parameterList);

                //是否达到最大投票数
                if (logList.Count >= campaign_PictureVote.MaxVoteTimes)
                {
                    return EnumCampaignPictureVoteResult.OverVoteTimes;
                }

                if (logList.Count > 0)
                {
                    //是否给同一个项目投过票
                    if ((from c in logList where c.ItemId == args.ItemId select c).Count() > 0)
                    {
                        return EnumCampaignPictureVoteResult.Voted;
                    }
                }
            }

            //投票
            Campaign_PictureVoteLogEntity log = new Campaign_PictureVoteLogEntity();
            log.CampaignId = args.CampaignId;
            log.Domain = args.DomainId;
            log.ItemId = args.ItemId;
            log.OpenId = args.OpenId;
            log.Member = args.Member;
            log.VoteTime = DateTime.Now;

            _campaignManager.DataBase.Insert(log);

            parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@itemId", args.ItemId));

            _campaignManager.DataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVoteItem] SET [VoteQuantity] = [VoteQuantity] + 1 WHERE [Id] = @itemId",
                parameterList);

            return EnumCampaignPictureVoteResult.Successful;
        }

        /// <summary>
        /// 获取指定的最美投票活动的项目列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetItemListResult GetPictureVoteItemList(GetCampaign_PictureVoteItemListArgs args)
        {
            if (RelationalMappingUnity.IsValidFieldName(args.OrderBy) == false)
            {
                args.OrderBy = GetCampaign_PictureVoteItemListArgs.DefaultOrderBy;
            }

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@approveStatus", args.ApproveStatus));
            parameterList.Add(new CommandParameter("@title", args.Title));
            parameterList.Add(new CommandParameter("@memberName", args.MemberName));
            parameterList.Add(new CommandParameter("@userName", args.UserName));
            parameterList.Add(new CommandParameter("@orderby", args.OrderBy));
            parameterList.Add(new CommandParameter("@sort", "DESC"));


            DataSet dsResult =
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_PictureVoteItemList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetPictureVoteItemList(args);
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

        /// <summary>
        /// 修改审核状态 ，通过
        /// </summary>
        /// <param name="args"></param>
        public NormalResult PictureVoteItemApprove(DomainContext domainContext, Campaign_PictureVoteItemApproveArgs args)
        {
            //判断是否达到最大可参与人数
            if (PictureVoteIsFullParticipant(args.CampaignId))
            {
                return new NormalResult("该活动已达最大允许参与人数，如需调整允许参与的人数，请编辑此活动基本信息。");
            }

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", args.ItemId));
            parameterList.Add(new CommandParameter("@approveStatus", EnumCampaignPictureVoteItemApproveStatus.Approved));

            _campaignManager.DataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVoteItem] SET [ApproveStatus] = @approveStatus WHERE [Id] = @id",
                parameterList);

            //奖励参与者积分
            if (args.MemberId.HasValue)
            {
                Campaign_PictureVoteEntity campaignPictureVote = GetPictureVote(args.CampaignId);
                if (campaignPictureVote.ApprovedPoint > 0)
                {
                    CampaignEntity campaign = _campaignManager.GetCampaign(args.CampaignId);

                    PointTrackArgs pointTrackArgs = new PointTrackArgs();
                    pointTrackArgs.DomainId = campaign.Domain;
                    pointTrackArgs.MemberId = args.MemberId.Value;
                    pointTrackArgs.Quantity = campaignPictureVote.ApprovedPoint;
                    pointTrackArgs.Type = MemberPointTrackType.Campaign;
                    pointTrackArgs.TagName = campaign.Name;
                    pointTrackArgs.TagId = campaign.Id;

                    _campaignManager.MemberManager.PointTrack(pointTrackArgs);
                }
            }

            return new NormalResult();
        }

        /// <summary>
        /// 修改审核状态 ，拒绝
        /// </summary>
        /// <param name="args"></param>
        public NormalResult PictureVoteItemRejected(DomainContext domainContext, Campaign_PictureVoteItemRejectedArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", args.ItemId));
            parameterList.Add(new CommandParameter("@approveStatus", EnumCampaignPictureVoteItemApproveStatus.Rejected));
            parameterList.Add(new CommandParameter("@rejectedMessage", args.Message));

            _campaignManager.DataBase.ExecuteNonQuery(
                "UPDATE [Campaign_PictureVoteItem] SET [ApproveStatus] = @approveStatus,[RejectedMessage] = @rejectedMessage WHERE [Id] = @id",
                parameterList);

            return new NormalResult();
        }

        public void PictureVoteItemLock(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _campaignManager.DataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVoteItem] SET [Lock] = 1 WHERE [Id] = @id",
                parameterList);
        }

        public void PictureVoteItemUnLock(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _campaignManager.DataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVoteItem] SET [Lock] = 0 WHERE [Id] = @id",
                parameterList);
        }

        /// <summary>
        /// 获取为某个投票项目投票的会员列表
        /// </summary>
        public GetItemListResult GetPictureVoteItemMemberList(GetCampaign_PictureVoteItemLogMemberListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@itemId", args.ItemId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));

            DataSet dsResult =
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_PictureVoteItemMemberList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetPictureVoteItemMemberList(args);
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

        /// <summary>
        /// 指定最美投票活动的活动数据
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        public GetPictureVoteDataAnalyseResult GetPictureVoteDataAnalyse(GetCampaign_PictureVoteDataAnalyseArgs args)
        {
            if ((args.EndDate - args.StartDate).TotalDays >= 300)
            {
                args.StartDate = args.EndDate.AddDays(-300);
            }

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));
            parameterList.Add(new CommandParameter("@startDate", args.StartDate));
            parameterList.Add(new CommandParameter("@endDate", args.EndDate));

            DataSet dsResult =
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_PictureVoteDataAnalyse", parameterList,
                new string[] { "dayUpload", "dayVote", "approveStatus" });

            GetPictureVoteDataAnalyseResult result = new GetPictureVoteDataAnalyseResult();

            result.DayUpload = dsResult.Tables["dayUpload"];
            result.DayVote = dsResult.Tables["dayVote"];

            if (dsResult.Tables["approveStatus"].Rows.Count > 0)
            {
                string strWaitingCount = dsResult.Tables["approveStatus"].Rows[0]["0"].ToString();
                string strApprovedCount = dsResult.Tables["approveStatus"].Rows[0]["1"].ToString();
                string strRejectedCount = dsResult.Tables["approveStatus"].Rows[0]["2"].ToString();

                if (string.IsNullOrEmpty(strWaitingCount) == false)
                    result.WaitingCount = int.Parse(strWaitingCount);

                if (string.IsNullOrEmpty(strApprovedCount) == false)
                    result.ApprovedCount = int.Parse(strApprovedCount);

                if (string.IsNullOrEmpty(strRejectedCount) == false)
                    result.RejectedCount = int.Parse(strRejectedCount);
            }

            return result;
        }

        public Campaign_PictureVoteDataReport GetPictureVoteDataReport(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));

            DataSet dsResult =
               _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_PictureVoteDataReport", parameterList,
               new string[] { "approvedItemsCount", "voteQuantityCount", "pageVisitCount" });

            Campaign_PictureVoteDataReport result = new Campaign_PictureVoteDataReport();

            if (dsResult.Tables["approvedItemsCount"].Rows.Count > 0)
            {
                string strApprovedItemsCount = dsResult.Tables["approvedItemsCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strApprovedItemsCount) == false)
                    result.ApprovedItemsCount = int.Parse(strApprovedItemsCount);
            }

            if (dsResult.Tables["voteQuantityCount"].Rows.Count > 0)
            {
                string strVoteQuantityCount = dsResult.Tables["voteQuantityCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strVoteQuantityCount) == false)
                    result.VoteQuantityCount = int.Parse(strVoteQuantityCount);
            }

            if (dsResult.Tables["pageVisitCount"].Rows.Count > 0)
            {
                string strPageVisitCount = dsResult.Tables["pageVisitCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strPageVisitCount) == false)
                    result.PageVisitCount = int.Parse(strPageVisitCount);
            }

            return result;
        }

        /// <summary>
        /// 分享最美投票到朋友圈
        /// </summary>
        /// <param name="campaignId"></param>
        /// <param name="memberId"></param>
        public Campaign_PictureVoteShareResult PictureVoteShareTimeline(Guid campaignId, Guid? memberId, string openId)
        {
            Campaign_PictureVoteShareResult result = new Campaign_PictureVoteShareResult();

            CampaignEntity campaign = _campaignManager.GetCampaign(campaignId);
            if (campaign == null || campaign.Status != EnumCampaignStatus.Ongoing)
                return result;

            //分享活动奖励积分
            PointTrackResult shareTimelineResult = _campaignManager.ShareCampaignToTimeline(campaign, memberId, openId);
            if (shareTimelineResult.Success == false)
                return result;

            result.Point = campaign.ShareTimelinePoint;

            List<Campaign_PictureVoteItemEntity> itemList = GetMemberPictureVoteItemList(campaignId, memberId.Value);
            if (itemList.Count > 0)
            {
                //分享活动奖励票数
                //当前已经发布出来的投票项目全部加指定票数
                Campaign_PictureVoteEntity pictureVote = GetPictureVote(campaignId);
                if (pictureVote == null)
                    return result;

                List<CommandParameter> parameterList;
                foreach (var item in itemList)
                {
                    parameterList = new List<CommandParameter>();
                    parameterList.Add(new CommandParameter("@itemId", item.Id));
                    parameterList.Add(new CommandParameter("@quantity", pictureVote.ShareTimelineVote));

                    _campaignManager.DataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVoteItem] SET [VoteQuantity] = [VoteQuantity] + @quantity WHERE [Id] = @itemId",
                        parameterList);
                }
                result.Vote = pictureVote.ShareTimelineVote;
            }

            return result;
        }

        public Campaign_PictureVoteShareResult PictureVoteShareAppMessage(Guid campaignId, Guid? memberId, string openId)
        {
            Campaign_PictureVoteShareResult result = new Campaign_PictureVoteShareResult();

            CampaignEntity campaign = _campaignManager.GetCampaign(campaignId);
            if (campaign == null || campaign.Status != EnumCampaignStatus.Ongoing)
                return result;

            //分享活动奖励积分
            PointTrackResult shareTimelineResult = _campaignManager.ShareCampaignToAppMessage(campaign, memberId, openId);
            if (shareTimelineResult.Success == false)
                return result;

            result.Point = campaign.ShareAppMessagePoint;

            List<Campaign_PictureVoteItemEntity> itemList = GetMemberPictureVoteItemList(campaignId, memberId.Value);
            if (itemList.Count > 0)
            {
                //分享活动奖励票数
                //当前已经发布出来的投票项目全部加指定票数
                Campaign_PictureVoteEntity pictureVote = GetPictureVote(campaignId);
                if (pictureVote == null)
                    return result;

                List<CommandParameter> parameterList;
                foreach (var item in itemList)
                {
                    parameterList = new List<CommandParameter>();
                    parameterList.Add(new CommandParameter("@itemId", item.Id));
                    parameterList.Add(new CommandParameter("@quantity", pictureVote.ShareAppMessageVote));

                    _campaignManager.DataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVoteItem] SET [VoteQuantity] = [VoteQuantity] + @quantity WHERE [Id] = @itemId",
                        parameterList);
                }
                result.Vote = pictureVote.ShareAppMessageVote;
            }

            return result;
        }

        public void OpenPictureVoteNewItem(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _campaignManager.DataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVote] SET [AllowedNewItem] = 1 WHERE [CampaignId] = @id",
                parameterList);
        }

        public void ClosePictureVoteNewItem(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _campaignManager.DataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVote] SET [AllowedNewItem] = 0 WHERE [CampaignId] = @id",
                parameterList);
        }

        /// <summary>
        /// 获取指定最美投票活动已经审批通过的条目数
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        public int GetPictureVoteItemApprovedCount(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));

            int intCount = 0;
            _campaignManager.DataBase.ExecuteScalar<int>(
                "SELECT Count(1) FROM [Campaign_PictureVoteItem] WHERE [CampaignId] = @campaignId AND [ApproveStatus] = 1",
                parameterList, (scalarValue) => { intCount = scalarValue; });

            return intCount;
        }

        /// <summary>
        /// 判断指定的指标活动是否达到最大允许参与人数
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        public bool PictureVoteIsFullParticipant(Guid campaignId)
        {
            CampaignEntity campaign = _campaignManager.GetCampaign(campaignId);
            if (campaign.MaxParticipant > 0 && GetPictureVoteItemApprovedCount(campaignId) >= campaign.MaxParticipant)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
