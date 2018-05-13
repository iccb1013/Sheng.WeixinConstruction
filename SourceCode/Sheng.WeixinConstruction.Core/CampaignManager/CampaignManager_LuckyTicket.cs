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
    public class CampaignManager_LuckyTicket
    {
        private CampaignManager _campaignManager;

        public CampaignManager_LuckyTicket(CampaignManager campaignManager)
        {
            _campaignManager = campaignManager;
        }

        public GetItemListResult GetCampaign_LuckyTicketList(GetCampaign_LuckyTicketListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@status", args.Status));

            DataSet dsResult =
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LuckyTicketList", parameterList, new string[] { "result" });

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

            _campaignManager.DataBase.InsertList(campaign, luckyTicket);
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

            _campaignManager.DataBase.UpdateList(campaign, luckyTicket);
        }

        public void RemoveLuckyTicket(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", campaignId));

            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign] WHERE [Id] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_LuckyTicket] WHERE [CampaignId] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_LuckyTicketLog] WHERE [CampaignId] = @id", parameterList);
        }

        public Campaign_LuckyTicketEntity GetLuckyTicket(Guid campaignId)
        {
            Campaign_LuckyTicketEntity campaign = new Campaign_LuckyTicketEntity();
            campaign.CampaignId = campaignId;

            if (_campaignManager.DataBase.Fill<Campaign_LuckyTicketEntity>(campaign))
                return campaign;
            else
                return null;
        }

        public Campaign_LuckyTicketBundle GetLuckyTicketBundle(Guid campaignId)
        {
            Campaign_LuckyTicketBundle bundle = new Campaign_LuckyTicketBundle();

            bundle.Campaign = _campaignManager.GetCampaign(campaignId);
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
            _campaignManager.DataBase.ExecuteScalar<int>(
                "SELECT Count(1) FROM [Campaign_LuckyTicketLog] WHERE [CampaignId] = @campaignId AND [Member] = @memberId AND [FromOpenId] = @fromOpenId",
                parameterList, (scalarValue) => { intStatus = scalarValue; });

            if (intStatus > 0)
                return false;

            //生成一个抽奖号码
            args.CreateTime = DateTime.Now;
            args.TicketNumber = _campaignManager.DomainManager.GetRandomSerialNumber("Campaign_LuckyTicket");

            if (String.IsNullOrEmpty(args.TicketNumber))
            {
                _campaignManager.Log.Write("抽奖号码生成失败。", JsonHelper.Serializer(args), TraceEventType.Warning);
                return false;
            }

            _campaignManager.DataBase.Insert(args);

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
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LuckyTicketLogList",
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
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LuckyTicketLogListByMember",
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
               _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LuckyTicketDataReport", parameterList,
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

            _campaignManager.DataBase.ExecuteNonQuery(CommandType.StoredProcedure, "Campaign_LuckyTicketDraw", parameterList);

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
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_LuckyTicketWinLogList",
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

            List<Campaign_LuckyTicketLogEntity> logList = _campaignManager.DataBase.Select<Campaign_LuckyTicketLogEntity>(
                "SELECT * FROM [Campaign_LuckyTicketLog] WHERE [Member] = @memberId AND [CampaignId] = @campaignId AND [Win] = 1",
                parameterList);

            return logList;
        }

    }
}
