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
    public class CampaignManager_MemberQRCode
    {
        private CampaignManager _campaignManager;

        public CampaignManager_MemberQRCode(CampaignManager campaignManager)
        {
            _campaignManager = campaignManager;
        }

        public GetItemListResult GetCampaign_MemberQRCodeList(Guid domainId, string appId, GetCampaign_MemberQRCodeListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@status", args.Status));

            DataSet dsResult =
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_MemberQRCodeList", parameterList, new string[] { "result" });

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

            _campaignManager.DataBase.InsertList(campaign, memberQRCode);
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

            _campaignManager.DataBase.UpdateList(campaign, memberQRCode);
        }

        /// <summary>
        /// 删除整个活动所有数据
        /// </summary>
        /// <param name="campaignId"></param>
        public void RemoveMemberQRCode(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", campaignId));

            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign] WHERE [Id] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_MemberQRCode] WHERE [CampaignId] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_MemberQRCodeItem] WHERE [CampaignId] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_MemberQRCodeLandingLog] WHERE [CampaignId] = @id", parameterList);
        }

        public Campaign_MemberQRCodeBundle GetMemberQRCodeBundle(Guid id)
        {
            Campaign_MemberQRCodeBundle bundle = new Campaign_MemberQRCodeBundle();

            bundle.Campaign = _campaignManager.GetCampaign(id);
            bundle.MemberQRCode = GetMemberQRCode(id);

            return bundle;
        }

        public Campaign_MemberQRCodeEntity GetMemberQRCode(Guid campaignId)
        {
            Campaign_MemberQRCodeEntity campaign = new Campaign_MemberQRCodeEntity();
            campaign.CampaignId = campaignId;

            if (_campaignManager.DataBase.Fill<Campaign_MemberQRCodeEntity>(campaign))
                return campaign;
            else
                return null;
        }

        public Campaign_MemberQRCodeItemEntity GetMemberQRCodeItem(Guid campaignId, Guid memberId)
        {
            Campaign_MemberQRCodeItemEntity campaign = new Campaign_MemberQRCodeItemEntity();
            campaign.CampaignId = campaignId;
            campaign.Member = memberId;

            if (_campaignManager.DataBase.Fill<Campaign_MemberQRCodeItemEntity>(campaign))
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

            string landingUrl = String.Format(_campaignManager.SettingsManager.GetClientAddress(domainContext) +
                "Campaign/MemberQRCodeLanding/{0}?campaignId={1}&memberId={2}",
                campaignMemberQRCode.Domain, campaignId, memberId);

            GetCampaign_MemberQRCodeImageArgs args = new GetCampaign_MemberQRCodeImageArgs();
            args.BackgroundImageId = campaignMemberQRCode.BackgroundImageId;
            args.LandingUrl = landingUrl;
            args.Domain = campaignMemberQRCode.Domain;
            args.MemberId = memberId;

            GetMemberQRCodeImageResult getMemberQRCodeImageResult = _campaignManager.FileService.GetMemberQRCodeImage(args);
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
            item.QRCodeUrl = _campaignManager.FileService.FileServiceUri + getMemberQRCodeImageResult.FileName;

            _campaignManager.DataBase.Insert(item);

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
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_MemberQRCodeItemList", parameterList, new string[] { "result" });

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
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_MemberQRCodeDataAnalyse", parameterList,
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
            EnumCampaignStatus? status = _campaignManager.GetStatus(campaignId);
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
            _campaignManager.DataBase.ExecuteScalar<int>(
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

                _campaignManager.MemberManager.PointTrack(args);
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
                _campaignManager.DataBase.ExecuteNonQuery(
                   "UPDATE [Campaign_MemberQRCodeItem] SET [LandingCount] = [LandingCount] + 1,[LandingPersonCount] = [LandingPersonCount] + 1 WHERE [CampaignId] = @campaignId AND [Member] = @qrCodeOwnMember ",
                   parameterList);
            }
            else
            {
                _campaignManager.DataBase.ExecuteNonQuery(
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

            _campaignManager.DataBase.Insert(log);

            return campaignBundle.MemberQRCode.LandingUrl;
        }

        public int GetMemberQRCodeItemCount(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));

            int intCount = 0;
            _campaignManager.DataBase.ExecuteScalar<int>(
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
               _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_MemberQRCodeDataReport", parameterList,
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


    }
}
