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
    public class CampaignManager
    {
        private static readonly CampaignManager _instance = new CampaignManager();
        public static CampaignManager Instance
        {
            get { return _instance; }
        }

        internal static DomainManager _domainManager = DomainManager.Instance;
        internal static UserManager _userManager = UserManager.Instance;
        internal static MemberManager _memberManager = MemberManager.Instance;
        internal static ShareManager _shareManager = ShareManager.Instance;
        internal static FileService _fileService = FileService.Instance;
        internal static SettingsManager _settingsManager = SettingsManager.Instance;
        internal static LogService _log = LogService.Instance;

        internal DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private CampaignManager()
        {

        }

        private CampaignManager_ShakingLottery _shakingLottery;
        public CampaignManager_ShakingLottery ShakingLottery
        {
            get { return _shakingLottery; }
        }


        #region Campaign

        public void PageVisit(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));

            _dataBase.ExecuteNonQuery("UPDATE [Campaign] SET [PageVisitCount] = [PageVisitCount] + 1 WHERE [Id] = @campaignId",
                parameterList);

        }

        /// <summary>
        /// 获取指定活动的引导落地页面，如果没有设置，则获取域下的引导关注页面
        /// 再没有，就返回默认引导关注页面
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        //public string GetGuideSubscribeUrl(Guid campaignId, DomainContext domainContext)
        //{
        //    List<CommandParameter> parameterList = new List<CommandParameter>();
        //    parameterList.Add(new CommandParameter("@campaignId", campaignId));

        //    string url = null;
        //    _dataBase.ExecuteScalar<string>("SELECT [GuideSubscribeUrl] FROM [Campaign] WHERE [Id] = @campaignId",
        //        parameterList, (scalarString) => { url = scalarString; });

        //    if (String.IsNullOrEmpty(url) == false)
        //        return url;
        //    else
        //        return domainContext.GuideSubscribeUrl;
        //}

        public GetItemListResult GetCampaignList(GetCampaignListArgs args)
        {
            if (RelationalMappingUnity.IsValidFieldName(args.OrderBy) == false)
            {
                args.OrderBy = GetCampaignListArgs.DefaultOrderBy;
            }

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@type", args.Type));
            parameterList.Add(new CommandParameter("@status", args.Status));
            parameterList.Add(new CommandParameter("@orderby", args.OrderBy));
            parameterList.Add(new CommandParameter("@sort", args.Sort.ToString()));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaignList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetCampaignList(args);
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

        public EnumCampaignStartResult StartCampaign(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "StartCampaign", parameterList, new string[] { "result" });

            int reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());

            EnumCampaignStartResult result = (EnumCampaignStartResult)reason;

            return result;
        }

        public EnumCampaignEndResult EndCampaign(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "EndCampaign", parameterList, new string[] { "result" });

            int reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());

            EnumCampaignEndResult result = (EnumCampaignEndResult)reason;

            return result;
        }

        public CampaignEntity GetCampaign(Guid id)
        {
            CampaignEntity campaign = new CampaignEntity();
            campaign.Id = id;

            if (_dataBase.Fill<CampaignEntity>(campaign))
            {
                UserEntity userEntity = _userManager.GetUser(campaign.CreateUser);
                if (userEntity != null)
                    campaign.CreateUserName = userEntity.Name;
                return campaign;
            }
            else
                return null;
        }

        public GetCampaignCountResult GetCampaignCount(Guid domainId, string appId, EnumCampaignType type)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@type", (int)type));

            //注意，此存储过程在没有取到任何数据时，不会返回任何行
            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaignCount", parameterList, new string[] { "result" });

            GetCampaignCountResult result = new GetCampaignCountResult();
            if (dsResult.Tables[0].Rows.Count > 0)
            {
                string strPreparatoryCount = dsResult.Tables[0].Rows[0]["0"].ToString();
                string strOngoingCount = dsResult.Tables[0].Rows[0]["1"].ToString();
                string strEndCount = dsResult.Tables[0].Rows[0]["2"].ToString();

                if (string.IsNullOrEmpty(strPreparatoryCount) == false)
                    result.PreparatoryCount = int.Parse(strPreparatoryCount);

                if (string.IsNullOrEmpty(strOngoingCount) == false)
                    result.OngoingCount = int.Parse(strOngoingCount);

                if (string.IsNullOrEmpty(strEndCount) == false)
                    result.EndCount = int.Parse(strEndCount);
            }

            return result;
        }

        private EnumCampaignStatus? GetStatus(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            int intStatus = 0;
            if (_dataBase.ExecuteScalar<int>("SELECT [Status] FROM [Campaign] WHERE [Id] = @id", parameterList,
                (scalarValue) => { intStatus = scalarValue; }))
            {
                return (EnumCampaignStatus)intStatus;
            }
            else
            {
                return null;
            }
        }

        public string GetCampaignDescription(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            object objStr = _dataBase.ExecuteScalar(
                "SELECT [Description] FROM [Campaign] WHERE [Id] = @id", parameterList);

            if (objStr == null)
            {
                return "查询失败";
            }
            else
            {
                return objStr.ToString();
            }
        }

        public ShareResult ShareTimeline(Guid campaignId, Guid? memberId, string openId)
        {
            ShareResult result = new ShareResult();

            CampaignEntity campaign = GetCampaign(campaignId);
            if (campaign == null || campaign.Status != EnumCampaignStatus.Ongoing)
                return result;

            PointTrackResult pointTrackResult = ShareCampaignToTimeline(campaign, memberId, openId);
            if (pointTrackResult.Success == false)
                return result;

            result.Point = campaign.ShareTimelinePoint;
            return result;
        }

        public ShareResult ShareAppMessage(Guid campaignId, Guid? memberId, string openId)
        {
            ShareResult result = new ShareResult();

            CampaignEntity campaign = GetCampaign(campaignId);
            if (campaign == null || campaign.Status != EnumCampaignStatus.Ongoing)
                return result;

            PointTrackResult pointTrackResult = ShareCampaignToAppMessage(campaign, memberId, openId);
            if (pointTrackResult.Success == false)
                return result;

            result.Point = campaign.ShareAppMessagePoint;
            return result;
        }

        /// <summary>
        /// 分享活动到朋友圈
        /// </summary>
        private PointTrackResult ShareCampaignToTimeline(CampaignEntity campaign, Guid? memberId, string openId)
        {
            PointTrackResult result = new PointTrackResult();

            if (campaign == null || campaign.Status != EnumCampaignStatus.Ongoing)
                return result;

            if (campaign.ShareTimelinePoint <= 0)
                return result;

            #region 判断有没有分享过

            ShareLogEntity log = _shareManager.GetShareLog(campaign.Id, openId);
            if (log == null)
            {
                log = new ShareLogEntity();
                log.Member = memberId;
                log.OpenId = openId;
                log.PageId = campaign.Id;
                log.ShareTimeline = true;
                _shareManager.Create(log);
            }
            else
            {
                if (log.ShareTimeline && log.Member.HasValue)
                {
                    return result;
                }

                if (log.ShareTimeline && memberId.HasValue == false)
                {
                    return result;
                }

                log.Member = memberId;
                log.ShareTimeline = true;
                _shareManager.Update(log);
            }

            #endregion

            if (memberId.HasValue)
            {
                PointTrackArgs args = new PointTrackArgs();
                args.DomainId = campaign.Domain;
                args.MemberId = memberId.Value;
                args.Quantity = campaign.ShareTimelinePoint;
                args.Type = MemberPointTrackType.Campaign;
                args.TagName = campaign.Name;
                args.TagId = campaign.Id;
                result = _memberManager.PointTrack(args);
            }

            return result;
        }

        private PointTrackResult ShareCampaignToAppMessage(CampaignEntity campaign, Guid? memberId, string openId)
        {
            PointTrackResult result = new PointTrackResult();

            if (campaign == null || campaign.Status != EnumCampaignStatus.Ongoing)
                return result;

            if (campaign.ShareAppMessagePoint <= 0)
                return result;

            #region 判断有没有分享过

            ShareLogEntity log = _shareManager.GetShareLog(campaign.Id, openId);
            if (log == null)
            {
                log = new ShareLogEntity();
                log.Member = memberId;
                log.OpenId = openId;
                log.PageId = campaign.Id;
                log.ShareAppMessage = true;
                _shareManager.Create(log);
            }
            else
            {
                if (log.ShareAppMessage && log.Member.HasValue)
                {
                    return result;
                }

                if (log.ShareAppMessage && memberId.HasValue == false)
                {
                    return result;
                }

                log.Member = memberId;
                log.ShareAppMessage = true;
                _shareManager.Update(log);
            }

            #endregion

            if (memberId.HasValue)
            {
                PointTrackArgs args = new PointTrackArgs();
                args.DomainId = campaign.Domain;
                args.MemberId = memberId.Value;
                args.Quantity = campaign.ShareAppMessagePoint;
                args.Type = MemberPointTrackType.Campaign;
                args.TagName = campaign.Name;
                args.TagId = campaign.Id;
                result = _memberManager.PointTrack(args);
            }

            return result;
        }

        #endregion

        #region PictureVote

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
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_PictureVoteList", parameterList, new string[] { "result" });

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

            _dataBase.InsertList(campaign, pictureVote);
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

            _dataBase.UpdateList(campaign, pictureVote);
        }

        /// <summary>
        /// 删除整个投票活动
        /// </summary>
        /// <param name="campaignId"></param>
        public void RemovePictureVote(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", campaignId));

            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign] WHERE [Id] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_PictureVote] WHERE [CampaignId] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_PictureVoteItem] WHERE [CampaignId] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_PictureVoteLog] WHERE [CampaignId] = @id", parameterList);
        }

        public Campaign_PictureVoteBundle GetPictureVoteBundle(Guid id)
        {
            Campaign_PictureVoteBundle bundle = new Campaign_PictureVoteBundle();

            bundle.Campaign = GetCampaign(id);
            bundle.PictureVote = GetPictureVote(id);

            return bundle;
        }

        public Campaign_PictureVoteEntity GetPictureVote(Guid campaignId)
        {
            Campaign_PictureVoteEntity campaign = new Campaign_PictureVoteEntity();
            campaign.CampaignId = campaignId;

            if (_dataBase.Fill<Campaign_PictureVoteEntity>(campaign))
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
                _dataBase.ExecuteScalar<int>(
                    "SELECT Count(1) FROM [Campaign_PictureVoteItem] WHERE [CampaignId] = @campaignId AND [Member] = @memberId",
                    parameterList, (scalarValue) => { intCount = scalarValue; });

                if (intCount >= campaign_PictureVote.MaxPublishTimes)
                {
                    return EnumCampaignCreatePictureVoteItemResult.AlreadyPublished;
                }
            }

            int? serialNumber = _domainManager.GetSerialNumber("PictureVote");
            if (serialNumber.HasValue == false)
                return EnumCampaignCreatePictureVoteItemResult.Failed;

            args.SerialNumber = serialNumber.Value.ToString();

            _dataBase.Insert(args);

            return EnumCampaignCreatePictureVoteItemResult.Successful;
        }

        public Campaign_PictureVoteItemEntity GetPictureVoteItem(Guid id)
        {
            Campaign_PictureVoteItemEntity item = new Campaign_PictureVoteItemEntity();
            item.Id = id;
            if (_dataBase.Fill<Campaign_PictureVoteItemEntity>(item))
            {
                if (item.Member.HasValue)
                {
                    MemberEntity member = _memberManager.GetMember(item.Member.Value);
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

            List<Campaign_PictureVoteItemEntity> itemList = _dataBase.Select<Campaign_PictureVoteItemEntity>(
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
            if (_dataBase.ExecuteScalar<Guid>(
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

            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_PictureVoteItem] WHERE [Id] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_PictureVoteLog] WHERE [ItemId] = @id", parameterList);
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

            _dataBase.ExecuteNonQuery(
                "DELETE FROM [Campaign_PictureVoteItem] WHERE [Id] = @id AND [Member] = @memberId", parameterList);
            _dataBase.ExecuteNonQuery(
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

            List<Campaign_PictureVoteItemEntity> itemList = _dataBase.Select<Campaign_PictureVoteItemEntity>(
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
            _dataBase.ExecuteScalar<int>("SELECT Count(1) FROM [Campaign_PictureVoteItem] WHERE [CampaignId] = @campaignId AND [Member] = @memberId",
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
            _dataBase.ExecuteScalar<int>("SELECT Count(1) FROM [Campaign_PictureVoteItem] WHERE [CampaignId] = @campaignId AND [Member] = @memberId AND [ApproveStatus] = @approveStatus",
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

            CampaignEntity campaign = GetCampaign(args.CampaignId);
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
                List<Campaign_PictureVoteLogEntity> logList = _dataBase.Select<Campaign_PictureVoteLogEntity>(
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
                List<Campaign_PictureVoteLogEntity> logList = _dataBase.Select<Campaign_PictureVoteLogEntity>(
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

            _dataBase.Insert(log);

            parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@itemId", args.ItemId));

            _dataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVoteItem] SET [VoteQuantity] = [VoteQuantity] + 1 WHERE [Id] = @itemId",
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
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_PictureVoteItemList", parameterList, new string[] { "result" });

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

            _dataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVoteItem] SET [ApproveStatus] = @approveStatus WHERE [Id] = @id",
                parameterList);

            //奖励参与者积分
            if (args.MemberId.HasValue)
            {
                Campaign_PictureVoteEntity campaignPictureVote = GetPictureVote(args.CampaignId);
                if (campaignPictureVote.ApprovedPoint > 0)
                {
                    CampaignEntity campaign = GetCampaign(args.CampaignId);

                    PointTrackArgs pointTrackArgs = new PointTrackArgs();
                    pointTrackArgs.DomainId = campaign.Domain;
                    pointTrackArgs.MemberId = args.MemberId.Value;
                    pointTrackArgs.Quantity = campaignPictureVote.ApprovedPoint;
                    pointTrackArgs.Type = MemberPointTrackType.Campaign;
                    pointTrackArgs.TagName = campaign.Name;
                    pointTrackArgs.TagId = campaign.Id;

                    _memberManager.PointTrack(pointTrackArgs);
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

            _dataBase.ExecuteNonQuery(
                "UPDATE [Campaign_PictureVoteItem] SET [ApproveStatus] = @approveStatus,[RejectedMessage] = @rejectedMessage WHERE [Id] = @id",
                parameterList);

            return new NormalResult();
        }

        public void PictureVoteItemLock(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVoteItem] SET [Lock] = 1 WHERE [Id] = @id",
                parameterList);
        }

        public void PictureVoteItemUnLock(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVoteItem] SET [Lock] = 0 WHERE [Id] = @id",
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
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_PictureVoteItemMemberList", parameterList, new string[] { "result" });

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
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_PictureVoteDataAnalyse", parameterList,
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
               _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_PictureVoteDataReport", parameterList,
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

            CampaignEntity campaign = GetCampaign(campaignId);
            if (campaign == null || campaign.Status != EnumCampaignStatus.Ongoing)
                return result;

            //分享活动奖励积分
            PointTrackResult shareTimelineResult = ShareCampaignToTimeline(campaign, memberId, openId);
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

                    _dataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVoteItem] SET [VoteQuantity] = [VoteQuantity] + @quantity WHERE [Id] = @itemId",
                        parameterList);
                }
                result.Vote = pictureVote.ShareTimelineVote;
            }

            return result;
        }

        public Campaign_PictureVoteShareResult PictureVoteShareAppMessage(Guid campaignId, Guid? memberId, string openId)
        {
            Campaign_PictureVoteShareResult result = new Campaign_PictureVoteShareResult();

            CampaignEntity campaign = GetCampaign(campaignId);
            if (campaign == null || campaign.Status != EnumCampaignStatus.Ongoing)
                return result;

            //分享活动奖励积分
            PointTrackResult shareTimelineResult = ShareCampaignToAppMessage(campaign, memberId, openId);
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

                    _dataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVoteItem] SET [VoteQuantity] = [VoteQuantity] + @quantity WHERE [Id] = @itemId",
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

            _dataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVote] SET [AllowedNewItem] = 1 WHERE [CampaignId] = @id",
                parameterList);
        }

        public void ClosePictureVoteNewItem(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("UPDATE [Campaign_PictureVote] SET [AllowedNewItem] = 0 WHERE [CampaignId] = @id",
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
            _dataBase.ExecuteScalar<int>(
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
            CampaignEntity campaign = GetCampaign(campaignId);
            if (campaign.MaxParticipant > 0 && GetPictureVoteItemApprovedCount(campaignId) >= campaign.MaxParticipant)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region MemberQRCode

        public GetItemListResult GetCampaign_MemberQRCodeList(Guid domainId, string appId, GetCampaign_MemberQRCodeListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@status", args.Status));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_MemberQRCodeList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetCampaign_MemberQRCodeList(domainId, appId, args);
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

        public void CreateMemberQRCode(CampaignEntity campaign, Campaign_MemberQRCodeEntity memberQRCode)
        {
            if (campaign == null || memberQRCode == null)
            {
                Debug.Assert(false, "campaign == null || memberQRCode ==null");
                return;
            }

            memberQRCode.CampaignId = campaign.Id;
            memberQRCode.Domain = campaign.Domain;

            _dataBase.InsertList(campaign, memberQRCode);
        }

        public void UpdateMemberQRCode(CampaignEntity campaign, Campaign_MemberQRCodeEntity memberQRCode)
        {
            if (campaign == null || memberQRCode == null)
            {
                Debug.Assert(false, "campaign == null || memberQRCode ==null");
                return;
            }

            memberQRCode.CampaignId = campaign.Id;
            memberQRCode.Domain = campaign.Domain;

            _dataBase.UpdateList(campaign, memberQRCode);
        }

        /// <summary>
        /// 删除整个活动所有数据
        /// </summary>
        /// <param name="campaignId"></param>
        public void RemoveMemberQRCode(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", campaignId));

            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign] WHERE [Id] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_MemberQRCode] WHERE [CampaignId] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_MemberQRCodeItem] WHERE [CampaignId] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_MemberQRCodeLandingLog] WHERE [CampaignId] = @id", parameterList);
        }

        public Campaign_MemberQRCodeBundle GetMemberQRCodeBundle(Guid id)
        {
            Campaign_MemberQRCodeBundle bundle = new Campaign_MemberQRCodeBundle();

            bundle.Campaign = GetCampaign(id);
            bundle.MemberQRCode = GetMemberQRCode(id);

            return bundle;
        }

        public Campaign_MemberQRCodeEntity GetMemberQRCode(Guid campaignId)
        {
            Campaign_MemberQRCodeEntity campaign = new Campaign_MemberQRCodeEntity();
            campaign.CampaignId = campaignId;

            if (_dataBase.Fill<Campaign_MemberQRCodeEntity>(campaign))
                return campaign;
            else
                return null;
        }

        public Campaign_MemberQRCodeItemEntity GetMemberQRCodeItem(Guid campaignId, Guid memberId)
        {
            Campaign_MemberQRCodeItemEntity campaign = new Campaign_MemberQRCodeItemEntity();
            campaign.CampaignId = campaignId;
            campaign.Member = memberId;

            if (_dataBase.Fill<Campaign_MemberQRCodeItemEntity>(campaign))
            {
                return campaign;
            }
            else
            {
                return null;
            }
        }

        public NormalResult<Campaign_MemberQRCodeItemEntity> CreateMemberQRCodeItem(DomainContext domainContext, Guid campaignId, Guid memberId)
        {
            NormalResult<Campaign_MemberQRCodeItemEntity> result = new NormalResult<Campaign_MemberQRCodeItemEntity>();

            //判断有没有超过最大参与人数
            //if (MemberQRCodeIsFullParticipant(campaignId))
            //{
            //    result.Success = false;
            //    result.Message = "该活动已达最大允许参与人数。";
            //    return result;
            //}

            Campaign_MemberQRCodeEntity campaignMemberQRCode = GetMemberQRCode(campaignId);
            if (campaignMemberQRCode == null)
            {
                result.Success = false;
                result.Message = "指定的活动不存在。";
                return result;
            }

            string landingUrl = String.Format(_settingsManager.GetClientAddress(domainContext) +
                "Campaign/MemberQRCodeLanding/{0}?campaignId={1}&memberId={2}",
                campaignMemberQRCode.Domain, campaignId, memberId);

            GetCampaign_MemberQRCodeImageArgs args = new GetCampaign_MemberQRCodeImageArgs();
            args.BackgroundImageId = campaignMemberQRCode.BackgroundImageId;
            args.LandingUrl = landingUrl;
            args.Domain = campaignMemberQRCode.Domain;
            args.MemberId = memberId;

            GetMemberQRCodeImageResult getMemberQRCodeImageResult = _fileService.GetMemberQRCodeImage(args);
            if (getMemberQRCodeImageResult.Success == false)
            {
                result.Success = false;
                result.Message = getMemberQRCodeImageResult.Message;
                return result;
            }

            Campaign_MemberQRCodeItemEntity item = new Campaign_MemberQRCodeItemEntity();
            item.CampaignId = campaignId;
            item.CreateTime = DateTime.Now;
            item.Domain = campaignMemberQRCode.Domain;
            item.Member = memberId;
            item.QRCodeUrl = _fileService.FileServiceUri + getMemberQRCodeImageResult.FileName;

            _dataBase.Insert(item);

            result.Success = true;
            result.Data = item;
            return result;
        }

        public GetItemListResult GetMemberQRCodeItemList(GetCampaign_MemberQRCodeItemListArgs args)
        {
            if (RelationalMappingUnity.IsValidFieldName(args.OrderBy) == false)
            {
                args.OrderBy = GetCampaign_MemberQRCodeItemListArgs.DefaultOrderBy;
            }

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@memberName", args.MemberName));
            parameterList.Add(new CommandParameter("@orderby", args.OrderBy));
            parameterList.Add(new CommandParameter("@sort", "DESC"));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_MemberQRCodeItemList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetMemberQRCodeItemList(args);
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
        public GetMemberQRCodeDataAnalyseResult GetMemberQRCodeDataAnalyse(GetCampaign_MemberQRCodeDataAnalyseArgs args)
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
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_MemberQRCodeDataAnalyse", parameterList,
                new string[] { "dayCreate", "dayLanding", "dayLandingPerson", "landingCount", "landingPersonCount" });

            GetMemberQRCodeDataAnalyseResult result = new GetMemberQRCodeDataAnalyseResult();

            result.DayCreate = dsResult.Tables["dayCreate"];
            result.DayLanding = dsResult.Tables["dayLanding"];
            result.DayLandingPerson = dsResult.Tables["dayLandingPerson"];

            if (dsResult.Tables["landingCount"].Rows.Count > 0)
            {
                string strCount = dsResult.Tables["landingCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strCount) == false)
                    result.LandingCount = int.Parse(strCount);
            }

            if (dsResult.Tables["landingPersonCount"].Rows.Count > 0)
            {
                string strCount = dsResult.Tables["landingPersonCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strCount) == false)
                    result.LandingPersonCount = int.Parse(strCount);
            }

            return result;
        }

        public string MemberQRCodeLanding(Guid campaignId, Guid qrCodeOwnMemberId, string visitorOpenId)
        {
            Campaign_MemberQRCodeBundle campaignBundle = GetMemberQRCodeBundle(campaignId);
            if (campaignBundle.Empty)
                return null;

            if (String.IsNullOrEmpty(campaignBundle.MemberQRCode.LandingUrl))
                return null;

            //判断活动状态
            EnumCampaignStatus? status = GetStatus(campaignId);
            if (status == null || status.Value != EnumCampaignStatus.Ongoing)
            {
                //允许落地，但是不奖励了
                return campaignBundle.MemberQRCode.LandingUrl;
            }

            //如果同一个OpenId扫描过，则不奖励，但是日志还是会记录，用于统计
            List<CommandParameter> logParameterList = new List<CommandParameter>();
            logParameterList.Add(new CommandParameter("@campaignId", campaignId));
            logParameterList.Add(new CommandParameter("@qrCodeOwnMember", qrCodeOwnMemberId));
            logParameterList.Add(new CommandParameter("@visitorOpenId", visitorOpenId));

            //同一个访问者扫了多少次
            int sameVisitorCount = 0;
            _dataBase.ExecuteScalar<int>(
                "SELECT Count(1) FROM [Campaign_MemberQRCodeLandingLog] WHERE [CampaignId] = @campaignId AND [QRCodeOwnMember] = @qrCodeOwnMember AND [VisitorOpenId] = @visitorOpenId",
                logParameterList, (scalarValue) => { sameVisitorCount = scalarValue; });


            #region 奖励积分

            if (campaignBundle.MemberQRCode.LandingPoint > 0 && sameVisitorCount == 0)
            {
                //奖励积分
                PointTrackArgs args = new PointTrackArgs();
                args.DomainId = campaignBundle.Campaign.Domain;
                args.MemberId = qrCodeOwnMemberId;
                args.Quantity = campaignBundle.MemberQRCode.LandingPoint;
                args.Type = MemberPointTrackType.Campaign;
                args.TagName = campaignBundle.Campaign.Name;
                args.TagId = campaignId;

                _memberManager.PointTrack(args);
            }

            #endregion

            #region 计数

            //LandingCount 直接+1 LandingPersonCount 要判断人员是否重复
            //查询已经有多少人扫过
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));
            parameterList.Add(new CommandParameter("@qrCodeOwnMember", qrCodeOwnMemberId));

            if (sameVisitorCount == 0)
            {
                _dataBase.ExecuteNonQuery(
                   "UPDATE [Campaign_MemberQRCodeItem] SET [LandingCount] = [LandingCount] + 1,[LandingPersonCount] = [LandingPersonCount] + 1 WHERE [CampaignId] = @campaignId AND [Member] = @qrCodeOwnMember ",
                   parameterList);
            }
            else
            {
                _dataBase.ExecuteNonQuery(
                    "UPDATE [Campaign_MemberQRCodeItem] SET [LandingCount] = [LandingCount] + 1 WHERE [CampaignId] = @campaignId AND [Member] = @qrCodeOwnMember ",
                    parameterList);
            }

            #endregion

            //记录落地日志
            Campaign_MemberQRCodeLandingLogEntity log = new Campaign_MemberQRCodeLandingLogEntity();
            log.CampaignId = campaignId;
            log.Domain = campaignBundle.Campaign.Domain;
            log.QRCodeOwnMember = qrCodeOwnMemberId;
            log.VisitorOpenId = visitorOpenId;
            log.LandingTime = DateTime.Now;

            _dataBase.Insert(log);

            return campaignBundle.MemberQRCode.LandingUrl;
        }

        public int GetMemberQRCodeItemCount(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));

            int intCount = 0;
            _dataBase.ExecuteScalar<int>(
                "SELECT Count(1) FROM [Campaign_MemberQRCodeItem] WHERE [CampaignId] = @campaignId",
                parameterList, (scalarValue) => { intCount = scalarValue; });

            return intCount;
        }

        /// <summary>
        /// 判断指定的指标活动是否达到最大允许参与人数
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        //public bool MemberQRCodeIsFullParticipant(Guid campaignId)
        //{
        //    CampaignEntity campaign = GetCampaign(campaignId);
        //    if (campaign.MaxParticipant > 0 && GetMemberQRCodeItemCount(campaignId) >= campaign.MaxParticipant)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public Campaign_MemberQRCodeDataReport GetMemberQRCodeDataReport(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));

            DataSet dsResult =
               _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_MemberQRCodeDataReport", parameterList,
               new string[] { "memberCount", "landingCount", "pageVisitCount" });

            Campaign_MemberQRCodeDataReport result = new Campaign_MemberQRCodeDataReport();

            if (dsResult.Tables["memberCount"].Rows.Count > 0)
            {
                string strMemberCount = dsResult.Tables["memberCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strMemberCount) == false)
                    result.MemberCount = int.Parse(strMemberCount);
            }

            if (dsResult.Tables["landingCount"].Rows.Count > 0)
            {
                string strLandingCount = dsResult.Tables["landingCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strLandingCount) == false)
                    result.LandingCount = int.Parse(strLandingCount);
            }

            if (dsResult.Tables["pageVisitCount"].Rows.Count > 0)
            {
                string strPageVisitCount = dsResult.Tables["pageVisitCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strPageVisitCount) == false)
                    result.PageVisitCount = int.Parse(strPageVisitCount);
            }

            return result;
        }



        #endregion

        #region Lottery

        public GetItemListResult GetCampaign_LotteryList(Guid domainId, string appId, GetCampaign_LotteryListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@status", args.Status));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LotteryList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetCampaign_LotteryList(domainId, appId, args);
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

        public void CreateLottery(CampaignEntity campaign, Campaign_LotteryEntity lottery)
        {
            if (campaign == null || lottery == null)
            {
                Debug.Assert(false, "campaign == null || lottery ==null");
                return;
            }

            lottery.CampaignId = campaign.Id;
            lottery.Domain = campaign.Domain;

            _dataBase.InsertList(campaign, lottery);
        }

        public void UpdateLottery(CampaignEntity campaign, Campaign_LotteryEntity lottery)
        {
            if (campaign == null || lottery == null)
            {
                Debug.Assert(false, "campaign == null || lottery ==null");
                return;
            }

            lottery.CampaignId = campaign.Id;
            lottery.Domain = campaign.Domain;

            _dataBase.UpdateList(campaign, lottery);
        }

        public void RemoveLottery(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", campaignId));

            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign] WHERE [Id] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_Lottery] WHERE [CampaignId] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_LotteryPeriod] WHERE [CampaignId] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_LotterySignLog] WHERE [CampaignId] = @id", parameterList);
        }

        public void CreateLotteryPeriod(Campaign_LotteryPeriodEntity lotteryPeriod)
        {
            if (lotteryPeriod == null)
            {
                Debug.Assert(false, "lotteryPeriod == null");
                return;
            }

            _dataBase.Insert(lotteryPeriod);
        }

        public NormalResult UpdateLotteryPeriod(Campaign_LotteryPeriodEntity lotteryPeriod)
        {
            if (lotteryPeriod == null)
            {
                Debug.Assert(false, "lotteryPeriod == null");
                return new NormalResult("参数错误。");
            }

            //所属活动如果已结束则不允许修改
            EnumCampaignStatus? campaignStatus = GetStatus(lotteryPeriod.CampaignId);
            if (campaignStatus == null)
                return new NormalResult("活动不存在。");

            if (campaignStatus.Value == EnumCampaignStatus.End)
                return new NormalResult("已结束活动的周期不允许修改。");

            Campaign_LotteryPeriodEntity current = GetLotteryPeriod(lotteryPeriod.Id);
            if (current == null)
                return new NormalResult("要修改的周期不存在。");

            if (current.Drawn)
                return new NormalResult("已完成抽奖的周期不允许修改。");

            //对于正在进行中的活动，已经结束或抽过奖的周期不允许修改
            if (campaignStatus.Value == EnumCampaignStatus.Ongoing)
            {
                if (current.EndTime <= DateTime.Now || current.Drawn)
                    return new NormalResult("已结束的周期不可修改。");
            }

            _dataBase.Update(lotteryPeriod);

            return new NormalResult();
        }

        public void RemoveLotteryPeriod(Guid periodId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", periodId));

            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_LotteryPeriod] WHERE [Id] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_LotterySignLog] WHERE [Period] = @id", parameterList);

        }

        public Campaign_LotteryEntity GetLottery(Guid campaignId)
        {
            Campaign_LotteryEntity campaign = new Campaign_LotteryEntity();
            campaign.CampaignId = campaignId;

            if (_dataBase.Fill<Campaign_LotteryEntity>(campaign))
                return campaign;
            else
                return null;
        }

        public Campaign_LotteryBundle GetLotteryBundle(Guid campaignId)
        {
            Campaign_LotteryBundle bundle = new Campaign_LotteryBundle();

            bundle.Campaign = GetCampaign(campaignId);
            bundle.Lottery = GetLottery(campaignId);

            return bundle;
        }

        /// <summary>
        /// 界于指定日期之前的所有活动
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetItemListResult GetLotteryPeriodList(GetLotteryPeriodListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@startTime", args.StartTime));
            parameterList.Add(new CommandParameter("@endTime", args.EndTime));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LotteryPeriodList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetLotteryPeriodList(args);
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
        /// 通过结束日期查找正在进行中的活动
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetItemListResult GetLotteryOngoingPeriodList(GetLotteryPeriodListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@endTime", args.EndTime));
            parameterList.Add(new CommandParameter("@type", 1));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LotteryPeriodListByEndTime", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetLotteryOngoingPeriodList(args);
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
        /// 通过结束日期查找正在进行中的活动
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetItemListResult GetLotteryEndedPeriodList(GetLotteryPeriodListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@endTime", args.EndTime));
            parameterList.Add(new CommandParameter("@type", 2));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LotteryPeriodListByEndTime", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetLotteryEndedPeriodList(args);
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

        public Campaign_LotteryPeriodEntity GetLotteryPeriod(Guid id)
        {
            Campaign_LotteryPeriodEntity campaign = new Campaign_LotteryPeriodEntity();
            campaign.Id = id;

            if (_dataBase.Fill<Campaign_LotteryPeriodEntity>(campaign))
                return campaign;
            else
                return null;
        }

        public Campaign_LotterySignLogEntity GetLotteryPeriodLog(Guid campaignId, Guid periodId, Guid memberId)
        {
            Campaign_LotterySignLogEntity campaign = new Campaign_LotterySignLogEntity();
            campaign.CampaignId = campaignId;
            campaign.Period = periodId;
            campaign.Member = memberId;

            if (_dataBase.Fill<Campaign_LotterySignLogEntity>(campaign))
                return campaign;
            else
                return null;
        }

        public GetItemListResult GetLotteryPeriodSignLogList(GetLotteryPeriodLogSignListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@periodId", args.PeriodId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@winner", args.Winner));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LotteryPeriodSignLogList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetLotteryPeriodSignLogList(args);
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
        /// 获取所有待开奖的抽奖周期
        /// </summary>
        /// <returns></returns>
        public List<Campaign_LotteryPeriodEntity> GetWaittingForDrawLotteryPeriod()
        {
            List<Campaign_LotteryPeriodEntity> list = _dataBase.Select<Campaign_LotteryPeriodEntity>(
               @"SELECT t.* FROM
                (
                    SELECT * FROM [Campaign_LotteryPeriod] WHERE [Drawn] = 0 AND [EndTime] <= GETDATE()
                ) t
                INNER JOIN [Campaign]
                ON t.[CampaignId] = [Campaign].[Id]
                WHERE [Campaign].[Status] = 1 OR [Campaign].[Status] = 2");

            return list;
        }

        public LotterySignResult LotterySign(LotterySignArgs args)
        {
            LotterySignResult result = new LotterySignResult();

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));
            parameterList.Add(new CommandParameter("@domain", args.DomainId));
            parameterList.Add(new CommandParameter("@period", args.PeriodId));
            parameterList.Add(new CommandParameter("@member", args.MemberId));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "Campaign_LotterySign", parameterList, new string[] { "result" });

            result.Result = (EnumLotterySignResult)int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());

            return result;
        }

        /// <summary>
        /// 返回中奖者列表（不分页）
        /// </summary>
        /// <param name="periodId"></param>
        /// <returns></returns>
        public GetLotteryWinnerListResult GetLotteryWinnerList(Guid periodId)
        {
            GetLotteryWinnerListResult result = new GetLotteryWinnerListResult();

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@period", periodId));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LotteryWinnerList", parameterList, new string[] { "result" });

            result.ItemList = dsResult.Tables[0];

            return result;
        }

        public bool IsLotteryWinner(Guid periodId, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@periodId", periodId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            int intStatus = 0;
            _dataBase.ExecuteScalar<int>(
                "SELECT Count(1) FROM [Campaign_LotterySignLog] WHERE [Period] = @periodId AND [Member] = @memberId AND [Win] = 1",
                parameterList, (scalarValue) => { intStatus = scalarValue; });

            return intStatus > 0;
        }

        public Campaign_LotteryDataReport GetLotteryDataReport(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));

            DataSet dsResult =
               _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LotteryDataReport", parameterList,
               new string[] { "memberCount", "winMemberCount", "pageVisitCount" });

            Campaign_LotteryDataReport result = new Campaign_LotteryDataReport();

            if (dsResult.Tables["memberCount"].Rows.Count > 0)
            {
                string strMemberCount = dsResult.Tables["memberCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strMemberCount) == false)
                    result.MemberCount = int.Parse(strMemberCount);
            }

            if (dsResult.Tables["winMemberCount"].Rows.Count > 0)
            {
                string strWinMemberCount = dsResult.Tables["winMemberCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strWinMemberCount) == false)
                    result.WinMemberCount = int.Parse(strWinMemberCount);
            }

            if (dsResult.Tables["pageVisitCount"].Rows.Count > 0)
            {
                string strPageVisitCount = dsResult.Tables["pageVisitCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strPageVisitCount) == false)
                    result.PageVisitCount = int.Parse(strPageVisitCount);
            }

            return result;
        }

        public void LotteryDraw()
        {
            List<Campaign_LotteryPeriodEntity> list = GetWaittingForDrawLotteryPeriod();
            if (list == null || list.Count == 0)
                return;

            foreach (Campaign_LotteryPeriodEntity period in list)
            {
                List<CommandParameter> parameterList = new List<CommandParameter>();
                parameterList.Add(new CommandParameter("@campaignId", period.CampaignId));
                parameterList.Add(new CommandParameter("@periodId", period.Id));

                _dataBase.ExecuteNonQuery(CommandType.StoredProcedure, "Campaign_LotteryDraw", parameterList);
            }
        }

        #endregion

        #region LuckyTicket

        public GetItemListResult GetCampaign_LuckyTicketList(GetCampaign_LuckyTicketListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@status", args.Status));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LuckyTicketList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetCampaign_LuckyTicketList(args);
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

        public void CreateLuckyTicket(CampaignEntity campaign, Campaign_LuckyTicketEntity luckyTicket)
        {
            if (campaign == null || luckyTicket == null)
            {
                Debug.Assert(false, "campaign == null || luckyTicket ==null");
                return;
            }

            luckyTicket.CampaignId = campaign.Id;
            luckyTicket.Domain = campaign.Domain;

            _dataBase.InsertList(campaign, luckyTicket);
        }

        public void UpdateLuckyTicket(CampaignEntity campaign, Campaign_LuckyTicketEntity luckyTicket)
        {
            if (campaign == null || luckyTicket == null)
            {
                Debug.Assert(false, "campaign == null || luckyTicket ==null");
                return;
            }

            luckyTicket.CampaignId = campaign.Id;
            luckyTicket.Domain = campaign.Domain;

            _dataBase.UpdateList(campaign, luckyTicket);
        }

        public void RemoveLuckyTicket(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", campaignId));

            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign] WHERE [Id] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_LuckyTicket] WHERE [CampaignId] = @id", parameterList);
            _dataBase.ExecuteNonQuery("DELETE FROM [Campaign_LuckyTicketLog] WHERE [CampaignId] = @id", parameterList);
        }

        public Campaign_LuckyTicketEntity GetLuckyTicket(Guid campaignId)
        {
            Campaign_LuckyTicketEntity campaign = new Campaign_LuckyTicketEntity();
            campaign.CampaignId = campaignId;

            if (_dataBase.Fill<Campaign_LuckyTicketEntity>(campaign))
                return campaign;
            else
                return null;
        }

        public Campaign_LuckyTicketBundle GetLuckyTicketBundle(Guid campaignId)
        {
            Campaign_LuckyTicketBundle bundle = new Campaign_LuckyTicketBundle();

            bundle.Campaign = GetCampaign(campaignId);
            bundle.LuckyTicket = GetLuckyTicket(campaignId);

            return bundle;
        }

        /// <summary>
        /// 创建一个抽奖号码
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool CreateLuckyTicketLog(Campaign_LuckyTicketLogEntity args)
        {
            if (args == null)
                return false;

            //先判断指定的的OpenId有没有帮MemberId生成过
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));
            parameterList.Add(new CommandParameter("@memberId", args.Member));
            parameterList.Add(new CommandParameter("@fromOpenId", args.FromOpenId));

            int intStatus = 0;
            _dataBase.ExecuteScalar<int>(
                "SELECT Count(1) FROM [Campaign_LuckyTicketLog] WHERE [CampaignId] = @campaignId AND [Member] = @memberId AND [FromOpenId] = @fromOpenId",
                parameterList, (scalarValue) => { intStatus = scalarValue; });

            if (intStatus > 0)
                return false;

            //生成一个抽奖号码
            args.CreateTime = DateTime.Now;
            args.TicketNumber = _domainManager.GetRandomSerialNumber("Campaign_LuckyTicket");

            if (String.IsNullOrEmpty(args.TicketNumber))
            {
                _log.Write("抽奖号码生成失败。", JsonHelper.Serializer(args), TraceEventType.Warning);
                return false;
            }

            _dataBase.Insert(args);

            return true;
        }

        public GetItemListResult GetLuckyTicketLogList(GetCampaign_LuckyTicketLogListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));
            parameterList.Add(new CommandParameter("@nickName", args.NickName));
            parameterList.Add(new CommandParameter("@mobilePhone", args.MobilePhone));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LuckyTicketLogList",
                parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetLuckyTicketLogList(args);
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

        /// <summary>
        /// 获取指定会员的号码列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetItemListResult GetMemberLuckyTicketLogList(GetCampaign_LuckyTicketLogListByMemberArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));
            parameterList.Add(new CommandParameter("@memberId", args.MemberId));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LuckyTicketLogListByMember",
                parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetMemberLuckyTicketLogList(args);
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

        public Campaign_LuckyTicketDataReport GetLuckyTicketDataReport(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));

            DataSet dsResult =
               _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LuckyTicketDataReport", parameterList,
               new string[] { "memberCount", "luckyTicketCount", "pageVisitCount" });

            Campaign_LuckyTicketDataReport result = new Campaign_LuckyTicketDataReport();

            if (dsResult.Tables["memberCount"].Rows.Count > 0)
            {
                string strMemberCount = dsResult.Tables["memberCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strMemberCount) == false)
                    result.MemberCount = int.Parse(strMemberCount);
            }

            if (dsResult.Tables["luckyTicketCount"].Rows.Count > 0)
            {
                string strLuckyTicketCount = dsResult.Tables["luckyTicketCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strLuckyTicketCount) == false)
                    result.LuckyTicketCount = int.Parse(strLuckyTicketCount);
            }

            if (dsResult.Tables["pageVisitCount"].Rows.Count > 0)
            {
                string strPageVisitCount = dsResult.Tables["pageVisitCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strPageVisitCount) == false)
                    result.PageVisitCount = int.Parse(strPageVisitCount);
            }

            return result;
        }

        public NormalResult LuckyTicketDraw(LuckyTicketDrawArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));
            parameterList.Add(new CommandParameter("@count", args.Count));
            parameterList.Add(new CommandParameter("@winRemark", args.WinRemark));

            _dataBase.ExecuteNonQuery(CommandType.StoredProcedure, "Campaign_LuckyTicketDraw", parameterList);

            return new NormalResult();
        }

        /// <summary>
        /// 获取所有中奖的号码
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetItemListResult GetLuckyTicketWinLogList(GetCampaign_LuckyTicketWinLogListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LuckyTicketWinLogList",
                parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetLuckyTicketWinLogList(args);
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

        public List<Campaign_LuckyTicketLogEntity> GetLuckyTicketWinLogListByMember(Guid campaignId, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            List<Campaign_LuckyTicketLogEntity> logList = _dataBase.Select<Campaign_LuckyTicketLogEntity>(
                "SELECT * FROM [Campaign_LuckyTicketLog] WHERE [Member] = @memberId AND [CampaignId] = @campaignId AND [Win] = 1",
                parameterList);

            return logList;
        }

        #endregion

        
    }
}
