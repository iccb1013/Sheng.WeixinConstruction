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
    public class CampaignManager_Donation
    {
        private static PayManager _payManager = PayManager.Instance;

        private CampaignManager _campaignManager;

        public CampaignManager_Donation(CampaignManager campaignManager)
        {
            _campaignManager = campaignManager;
        }

        public GetItemListResult GetCampaign_DonationList(GetCampaign_DonationListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@status", args.Status));

            DataSet dsResult =
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_DonationList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetCampaign_DonationList(args);
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

        public void CreateDonation(CampaignEntity campaign, Campaign_DonationEntity donation)
        {
            if (campaign == null || donation == null)
            {
                Debug.Assert(false, "campaign == null || donation ==null");
                return;
            }

            donation.CampaignId = campaign.Id;
            donation.Domain = campaign.Domain;

            _campaignManager.DataBase.InsertList(campaign, donation);
        }

        public void UpdateDonation(CampaignEntity campaign, Campaign_DonationEntity donation)
        {
            if (campaign == null || donation == null)
            {
                Debug.Assert(false, "campaign == null || donation ==null");
                return;
            }

            donation.CampaignId = campaign.Id;
            donation.Domain = campaign.Domain;

            _campaignManager.DataBase.UpdateList(campaign, donation);
        }

        public void RemoveDonation(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", campaignId));

            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign] WHERE [Id] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_Donation] WHERE [CampaignId] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_DonationLog] WHERE [CampaignId] = @id", parameterList);
        }

        public Campaign_DonationEntity GetDonation(Guid campaignId)
        {
            Campaign_DonationEntity campaign = new Campaign_DonationEntity();
            campaign.CampaignId = campaignId;

            if (_campaignManager.DataBase.Fill<Campaign_DonationEntity>(campaign))
                return campaign;
            else
                return null;
        }

        public Campaign_DonationBundle GetDonationBundle(Guid campaignId)
        {
            Campaign_DonationBundle bundle = new Campaign_DonationBundle();

            bundle.Campaign = _campaignManager.GetCampaign(campaignId);
            bundle.Donation = GetDonation(campaignId);

            return bundle;
        }

        //public bool CreateDonationLog(Campaign_DonationLogEntity args)
        //{
        //    if (args == null)
        //        return false;

        //    //先判断指定的的OpenId有没有帮MemberId生成过
        //    List<CommandParameter> parameterList = new List<CommandParameter>();
        //    parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));
        //    parameterList.Add(new CommandParameter("@memberId", args.Member));
        //    parameterList.Add(new CommandParameter("@fromOpenId", args.FromOpenId));

        //    int intStatus = 0;
        //    _campaignManager.DataBase.ExecuteScalar<int>(
        //        "SELECT Count(1) FROM [Campaign_DonationLog] WHERE [CampaignId] = @campaignId AND [Member] = @memberId AND [FromOpenId] = @fromOpenId",
        //        parameterList, (scalarValue) => { intStatus = scalarValue; });

        //    if (intStatus > 0)
        //        return false;

        //    //生成一个抽奖号码
        //    args.CreateTime = DateTime.Now;
        //    args.TicketNumber = _campaignManager.DomainManager.GetRandomSerialNumber("Campaign_Donation");

        //    if (String.IsNullOrEmpty(args.TicketNumber))
        //    {
        //        _campaignManager.Log.Write("抽奖号码生成失败。", JsonHelper.Serializer(args), TraceEventType.Warning);
        //        return false;
        //    }

        //    _campaignManager.DataBase.Insert(args);

        //    return true;
        //}

        public GetItemListResult GetDonationLogList(GetCampaign_DonationLogListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));

            if(args.Member.HasValue)
                parameterList.Add(new CommandParameter("@member", args.Member.Value));
            else
                parameterList.Add(new CommandParameter("@member", DBNull.Value));
            parameterList.Add(new CommandParameter("@finished", args.Finished));

            DataSet dsResult =
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_DonationLogList",
                parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetDonationLogList(args);
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

        public NormalResult<CreatePayOrderResult> DonationPay(DonationPayArgs args, AuthorizerPayConfig config)
        {
            //创建支付信息
            CreatePayOrderArgs createPayOrderArgs = new CreatePayOrderArgs();

            createPayOrderArgs.MemberId = args.MemberId;
            createPayOrderArgs.OpenId = args.OpenId;
            createPayOrderArgs.Fee = args.Fee;
            createPayOrderArgs.SpbillCreateIp = args.SpbillCreateIp;

            createPayOrderArgs.OrderType = EnumPayOrderType.Donation;
            createPayOrderArgs.Body = "在线捐款";
            createPayOrderArgs.OutTradeNo = Guid.NewGuid().ToString().Replace("-", "");
            NormalResult<CreatePayOrderResult> result = _payManager.CreatePayOrder(createPayOrderArgs, config);

            //捐款记录
            Campaign_DonationLogEntity log = new Campaign_DonationLogEntity();
            log.CampaignId = args.CampaignId;
            log.Domain = args.DomainId;
            log.PayOrder = result.Data.PayOrderId;
            log.Member = args.MemberId;
            log.CreateTime = DateTime.Now;
            log.Name = args.Name;
            log.Contact = args.Contact;
            log.Description = args.Description;
            _campaignManager.DataBase.Insert(log);

            return result;
        }


    }
}
