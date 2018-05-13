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
    public class CampaignManager
    {
        private static readonly CampaignManager _instance = new CampaignManager();
        public static CampaignManager Instance
        {
            get { return _instance; }
        }

        internal readonly DomainManager DomainManager = DomainManager.Instance;
        internal readonly UserManager UserManager = UserManager.Instance;
        internal readonly MemberManager MemberManager = MemberManager.Instance;
        internal readonly ShareManager ShareManager = ShareManager.Instance;
        internal readonly FileService FileService = FileService.Instance;
        internal readonly SettingsManager SettingsManager = SettingsManager.Instance;
        internal readonly LogService Log = LogService.Instance;

        internal readonly DatabaseWrapper DataBase = ServiceUnity.Instance.Database;

        private CampaignManager()
        {
            _shakingLottery = new CampaignManager_ShakingLottery(this);
            _pictureVote = new CampaignManager_PictureVote(this);
            _memberQRCode = new CampaignManager_MemberQRCode(this);
            _lottery = new CampaignManager_Lottery(this);
            _luckyTicket = new CampaignManager_LuckyTicket(this);
            _donation = new CampaignManager_Donation(this);
        }

        private CampaignManager_ShakingLottery _shakingLottery;
        public CampaignManager_ShakingLottery ShakingLottery
        {
            get { return _shakingLottery; }
        }

        private CampaignManager_PictureVote _pictureVote;
        public CampaignManager_PictureVote PictureVote
        {
            get { return _pictureVote; }
        }

        private CampaignManager_MemberQRCode _memberQRCode;
        public CampaignManager_MemberQRCode MemberQRCode
        {
            get { return _memberQRCode; }
        }

        private CampaignManager_Lottery _lottery;
        public CampaignManager_Lottery Lottery
        {
            get { return _lottery; }
        }

        private CampaignManager_LuckyTicket _luckyTicket;
        public CampaignManager_LuckyTicket LuckyTicket
        {
            get { return _luckyTicket; }
        }

        private CampaignManager_Donation _donation;
        public CampaignManager_Donation Donation
        {
            get { return _donation; }
        }

        #region Campaign

        public void PageVisit(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));

            DataBase.ExecuteNonQuery("UPDATE [Campaign] SET [PageVisitCount] = [PageVisitCount] + 1 WHERE [Id] = @campaignId",
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
                DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaignList", parameterList, new string[] { "result" });

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
                DataBase.ExecuteDataSet(CommandType.StoredProcedure, "StartCampaign", parameterList, new string[] { "result" });

            int reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());

            EnumCampaignStartResult result = (EnumCampaignStartResult)reason;

            return result;
        }

        public EnumCampaignEndResult EndCampaign(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            DataSet dsResult =
                DataBase.ExecuteDataSet(CommandType.StoredProcedure, "EndCampaign", parameterList, new string[] { "result" });

            int reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());

            EnumCampaignEndResult result = (EnumCampaignEndResult)reason;

            return result;
        }

        public CampaignEntity GetCampaign(Guid id)
        {
            CampaignEntity campaign = new CampaignEntity();
            campaign.Id = id;

            if (DataBase.Fill<CampaignEntity>(campaign))
            {
                UserEntity userEntity = UserManager.GetUser(campaign.CreateUser);
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
                DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaignCount", parameterList, new string[] { "result" });

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

        internal EnumCampaignStatus? GetStatus(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            int intStatus = 0;
            if (DataBase.ExecuteScalar<int>("SELECT [Status] FROM [Campaign] WHERE [Id] = @id", parameterList,
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

            object objStr = DataBase.ExecuteScalar(
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
        internal PointTrackResult ShareCampaignToTimeline(CampaignEntity campaign, Guid? memberId, string openId)
        {
            PointTrackResult result = new PointTrackResult();

            if (campaign == null || campaign.Status != EnumCampaignStatus.Ongoing)
                return result;

            if (campaign.ShareTimelinePoint <= 0)
                return result;

            #region 判断有没有分享过

            ShareLogEntity log = ShareManager.GetShareLog(campaign.Id, openId);
            if (log == null)
            {
                log = new ShareLogEntity();
                log.Member = memberId;
                log.OpenId = openId;
                log.PageId = campaign.Id;
                log.ShareTimeline = true;
                ShareManager.Create(log);
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
                ShareManager.Update(log);
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
                result = MemberManager.PointTrack(args);
            }

            return result;
        }

        internal PointTrackResult ShareCampaignToAppMessage(CampaignEntity campaign, Guid? memberId, string openId)
        {
            PointTrackResult result = new PointTrackResult();

            if (campaign == null || campaign.Status != EnumCampaignStatus.Ongoing)
                return result;

            if (campaign.ShareAppMessagePoint <= 0)
                return result;

            #region 判断有没有分享过

            ShareLogEntity log = ShareManager.GetShareLog(campaign.Id, openId);
            if (log == null)
            {
                log = new ShareLogEntity();
                log.Member = memberId;
                log.OpenId = openId;
                log.PageId = campaign.Id;
                log.ShareAppMessage = true;
                ShareManager.Create(log);
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
                ShareManager.Update(log);
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
                result = MemberManager.PointTrack(args);
            }

            return result;
        }

        #endregion

        

        

        

        
    }
}
