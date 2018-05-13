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
    public class CampaignManager_Lottery
    {
        private CampaignManager _campaignManager;

        public CampaignManager_Lottery(CampaignManager campaignManager)
        {
            _campaignManager = campaignManager;
        }

        public GetItemListResult GetCampaign_LotteryList(Guid domainId, string appId, GetCampaign_LotteryListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@status", args.Status));

            DataSet dsResult =
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LotteryList", parameterList, new string[] { "result" });

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

            _campaignManager.DataBase.InsertList(campaign, lottery);
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

            _campaignManager.DataBase.UpdateList(campaign, lottery);
        }

        public void RemoveLottery(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", campaignId));

            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign] WHERE [Id] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_Lottery] WHERE [CampaignId] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_LotteryPeriod] WHERE [CampaignId] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_LotterySignLog] WHERE [CampaignId] = @id", parameterList);
        }

        public void CreateLotteryPeriod(Campaign_LotteryPeriodEntity lotteryPeriod)
        {
            if (lotteryPeriod == null)
            {
                Debug.Assert(false, "lotteryPeriod == null");
                return;
            }

            _campaignManager.DataBase.Insert(lotteryPeriod);
        }

        public NormalResult UpdateLotteryPeriod(Campaign_LotteryPeriodEntity lotteryPeriod)
        {
            if (lotteryPeriod == null)
            {
                Debug.Assert(false, "lotteryPeriod == null");
                return new NormalResult("参数错误。");
            }

            //所属活动如果已结束则不允许修改
            EnumCampaignStatus? campaignStatus = _campaignManager.GetStatus(lotteryPeriod.CampaignId);
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

            _campaignManager.DataBase.Update(lotteryPeriod);

            return new NormalResult();
        }

        public void RemoveLotteryPeriod(Guid periodId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", periodId));

            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_LotteryPeriod] WHERE [Id] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_LotterySignLog] WHERE [Period] = @id", parameterList);

        }

        public Campaign_LotteryEntity GetLottery(Guid campaignId)
        {
            Campaign_LotteryEntity campaign = new Campaign_LotteryEntity();
            campaign.CampaignId = campaignId;

            if (_campaignManager.DataBase.Fill<Campaign_LotteryEntity>(campaign))
                return campaign;
            else
                return null;
        }

        public Campaign_LotteryBundle GetLotteryBundle(Guid campaignId)
        {
            Campaign_LotteryBundle bundle = new Campaign_LotteryBundle();

            bundle.Campaign = _campaignManager.GetCampaign(campaignId);
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
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LotteryPeriodList", parameterList, new string[] { "result" });

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
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LotteryPeriodListByEndTime", parameterList, new string[] { "result" });

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
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LotteryPeriodListByEndTime", parameterList, new string[] { "result" });

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

            if (_campaignManager.DataBase.Fill<Campaign_LotteryPeriodEntity>(campaign))
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

            if (_campaignManager.DataBase.Fill<Campaign_LotterySignLogEntity>(campaign))
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
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LotteryPeriodSignLogList", parameterList, new string[] { "result" });

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
            List<Campaign_LotteryPeriodEntity> list = _campaignManager.DataBase.Select<Campaign_LotteryPeriodEntity>(
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
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "Campaign_LotterySign", parameterList, new string[] { "result" });

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
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LotteryWinnerList", parameterList, new string[] { "result" });

            result.ItemList = dsResult.Tables[0];

            return result;
        }

        public bool IsLotteryWinner(Guid periodId, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@periodId", periodId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            int intStatus = 0;
            _campaignManager.DataBase.ExecuteScalar<int>(
                "SELECT Count(1) FROM [Campaign_LotterySignLog] WHERE [Period] = @periodId AND [Member] = @memberId AND [Win] = 1",
                parameterList, (scalarValue) => { intStatus = scalarValue; });

            return intStatus > 0;
        }

        public Campaign_LotteryDataReport GetLotteryDataReport(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));

            DataSet dsResult =
               _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LotteryDataReport", parameterList,
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

                _campaignManager.DataBase.ExecuteNonQuery(CommandType.StoredProcedure, "Campaign_LotteryDraw", parameterList);
            }
        }


    }
}
