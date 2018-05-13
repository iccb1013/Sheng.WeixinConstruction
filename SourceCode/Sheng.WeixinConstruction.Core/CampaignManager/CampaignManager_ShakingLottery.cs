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
    public class CampaignManager_ShakingLottery
    {
        private Dictionary<Guid, Campaign_ShakingLotteryGiftPool> _giftPool =
            new Dictionary<Guid, Campaign_ShakingLotteryGiftPool>();
        private object _giftPoolLockObj = new object();

        private CampaignManager _campaignManager;

        private Campaign_ShakingLotteryGiftPool GetGiftPool(Guid campaignId, Guid? periodId)
        {
            Campaign_ShakingLotteryGiftPool pool = null;

            if (_giftPool.ContainsKey(campaignId) == false)
            {
                lock (_giftPoolLockObj)
                {
                    if (_giftPool.ContainsKey(campaignId) == false)
                    {
                        pool = new Campaign_ShakingLotteryGiftPool(campaignId, periodId);
                        _giftPool.Add(campaignId, pool);
                    }
                }
            }

            if (pool == null)
                pool = _giftPool[campaignId];

            if (pool.PeriodId != periodId)
            {
                lock (_giftPoolLockObj)
                {
                    if (pool.PeriodId != periodId)
                    {
                        pool = new Campaign_ShakingLotteryGiftPool(campaignId, periodId);
                        _giftPool[campaignId] = pool;
                    }
                }
            }

            if ((DateTime.Now - pool.CreatedTime).TotalMinutes > 5)
            {
                lock (pool)
                {
                    if ((DateTime.Now - pool.CreatedTime).TotalMinutes > 5)
                    {
                        pool.LoadGiftList();
                    }
                }
            }

            return pool;
        }


        public CampaignManager_ShakingLottery(CampaignManager campaignManager)
        {
            _campaignManager = campaignManager;
        }

        public GetItemListResult GetList(GetCampaignListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            if (args.Status.HasValue)
                parameterList.Add(new CommandParameter("@status", args.Status));
            else
                parameterList.Add(new CommandParameter("@status", DBNull.Value));

            DataSet dsResult =
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_ShakingLotteryList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetList(args);
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

        public void Create(CampaignEntity campaign, Campaign_ShakingLotteryEntity shakingLottery)
        {
            if (campaign == null || shakingLottery == null)
            {
                Debug.Assert(false, "campaign == null || shakingLottery ==null");
                return;
            }

            shakingLottery.CampaignId = campaign.Id;
            shakingLottery.Domain = campaign.Domain;

            _campaignManager.DataBase.InsertList(campaign, shakingLottery);
        }

        public void Update(CampaignEntity campaign, Campaign_ShakingLotteryEntity shakingLottery)
        {
            if (campaign == null || shakingLottery == null)
            {
                Debug.Assert(false, "campaign == null || shakingLottery ==null");
                return;
            }

            shakingLottery.CampaignId = campaign.Id;
            shakingLottery.Domain = campaign.Domain;

            _campaignManager.DataBase.UpdateList(campaign, shakingLottery);
        }

        public void Remove(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", campaignId));

            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign] WHERE [Id] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_ShakingLottery] WHERE [CampaignId] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_ShakingLotteryGift] WHERE [CampaignId] = @id", parameterList);
            _campaignManager.DataBase.ExecuteNonQuery("DELETE FROM [Campaign_ShakingLotteryLog] WHERE [CampaignId] = @id", parameterList);
        }

        public Campaign_ShakingLotteryEntity Get(Guid campaignId)
        {
            Campaign_ShakingLotteryEntity campaign = new Campaign_ShakingLotteryEntity();
            campaign.CampaignId = campaignId;

            if (_campaignManager.DataBase.Fill<Campaign_ShakingLotteryEntity>(campaign))
                return campaign;
            else
                return null;
        }

        public Campaign_ShakingLotteryBundle GetBundle(Guid campaignId)
        {
            Campaign_ShakingLotteryBundle bundle = new Campaign_ShakingLotteryBundle();

            bundle.Campaign = _campaignManager.GetCampaign(campaignId);
            bundle.ShakingLottery = Get(campaignId);

            return bundle;
        }

        #region Gift

        public DataTable GetGiftDataTable(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));

            DataSet dsResult =
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure,
                "GetCampaign_ShakingLotteryGiftList", parameterList, new string[] { "result" });

            return dsResult.Tables[0];
        }

        public List<Campaign_ShakingLotteryGiftEntity> GetGiftList(Guid campaignId)
        {
            List<AttachedWhereItem> parameterList = new List<AttachedWhereItem>();
            parameterList.Add(new AttachedWhereItem("CampaignId", campaignId));

            List<Campaign_ShakingLotteryGiftEntity> result =
                _campaignManager.DataBase.Select<Campaign_ShakingLotteryGiftEntity>(parameterList);

            return result;
        }

        public List<Campaign_ShakingLotteryGiftEntity> GetGiftList(Guid campaignId, Guid periodId)
        {
            List<AttachedWhereItem> parameterList = new List<AttachedWhereItem>();
            parameterList.Add(new AttachedWhereItem("CampaignId", campaignId));
            parameterList.Add(new AttachedWhereItem("Period", periodId));

            List<Campaign_ShakingLotteryGiftEntity> result =
                _campaignManager.DataBase.Select<Campaign_ShakingLotteryGiftEntity>(parameterList);

            return result;
        }

        public void CreateGift(Campaign_ShakingLotteryGiftEntity gift)
        {
            if (gift == null)
            {
                Debug.Assert(false, "gift == null");
                return;
            }

            _campaignManager.DataBase.Insert(gift);
        }

        public void UpdateGift(Campaign_ShakingLotteryGiftEntity gift)
        {
            if (gift == null)
            {
                Debug.Assert(false, "gift == null");
                return;
            }

            _campaignManager.DataBase.Update(gift);
        }

        public void RemoveGift(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", campaignId));

            _campaignManager.DataBase.ExecuteNonQuery(
                "DELETE FROM [Campaign_ShakingLotteryGift] WHERE [Id] = @id", parameterList);
        }

        public Campaign_ShakingLotteryGiftEntity GetGift(Guid id)
        {
            Campaign_ShakingLotteryGiftEntity gift = new Campaign_ShakingLotteryGiftEntity();
            gift.Id = id;

            if (_campaignManager.DataBase.Fill<Campaign_ShakingLotteryGiftEntity>(gift))
                return gift;
            else
                return null;
        }

        #endregion

        #region Period

        public List<Campaign_ShakingLotteryPeriodEntity> GetPeriodList(Guid campaignId)
        {
            List<AttachedWhereItem> parameterList = new List<AttachedWhereItem>();
            parameterList.Add(new AttachedWhereItem("CampaignId", campaignId));

            List<Campaign_ShakingLotteryPeriodEntity> result =
                _campaignManager.DataBase.Select<Campaign_ShakingLotteryPeriodEntity>(parameterList);

            return result;
        }

        public void CreatePeriod(Campaign_ShakingLotteryPeriodEntity period)
        {
            if (period == null)
            {
                Debug.Assert(false, "period == null");
                return;
            }

            _campaignManager.DataBase.Insert(period);
        }

        public void UpdatePeriod(Campaign_ShakingLotteryPeriodEntity period)
        {
            if (period == null)
            {
                Debug.Assert(false, "period == null");
                return;
            }

            _campaignManager.DataBase.Update(period);
        }

        public void RemovePeriod(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", campaignId));

            _campaignManager.DataBase.ExecuteNonQuery(
                "DELETE FROM [Campaign_ShakingLotteryPeriod] WHERE [Id] = @id", parameterList);
        }

        public Campaign_ShakingLotteryPeriodEntity GetPeriod(Guid id)
        {
            Campaign_ShakingLotteryPeriodEntity period = new Campaign_ShakingLotteryPeriodEntity();
            period.Id = id;

            if (_campaignManager.DataBase.Fill<Campaign_ShakingLotteryPeriodEntity>(period))
                return period;
            else
                return null;
        }

        public void SetPeriod(Guid campaignId, Guid? periodId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));
            if (periodId.HasValue)
            {
                parameterList.Add(new CommandParameter("@periodId", periodId.Value));
            }
            else
            {
                parameterList.Add(new CommandParameter("@periodId", DBNull.Value));
            }

            _campaignManager.DataBase.ExecuteNonQuery(
                "UPDATE [Campaign_ShakingLottery] SET [Period] = @periodId WHERE [CampaignId] = @campaignId",
                parameterList);
        }

        #endregion

        public Campaign_ShakingLotteryDataReport GetDataReport(Guid campaignId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));

            DataSet dsResult =
               _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetCampaign_ShakingLotteryDataReport", parameterList,
               new string[] { "memberCount", "luckyMemberCount", "pageVisitCount" });

            Campaign_ShakingLotteryDataReport result = new Campaign_ShakingLotteryDataReport();

            if (dsResult.Tables["memberCount"].Rows.Count > 0)
            {
                string strMemberCount = dsResult.Tables["memberCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strMemberCount) == false)
                    result.MemberCount = int.Parse(strMemberCount);
            }

            if (dsResult.Tables["luckyMemberCount"].Rows.Count > 0)
            {
                string strLuckyMemberCount = dsResult.Tables["luckyMemberCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strLuckyMemberCount) == false)
                    result.LuckyMemberCount = int.Parse(strLuckyMemberCount);
            }

            if (dsResult.Tables["pageVisitCount"].Rows.Count > 0)
            {
                string strPageVisitCount = dsResult.Tables["pageVisitCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strPageVisitCount) == false)
                    result.PageVisitCount = int.Parse(strPageVisitCount);
            }

            return result;
        }

        public DataTable GetLuckyMemberList(Guid campaignId)
        {
            return new DataTable();
        }

        /// <summary>
        /// 摇奖
        /// </summary>
        /// <param name="campaignId"></param>
        /// <param name="memberId"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public NormalResult<Campaign_ShakingLotteryGiftEntity> Shake(Guid campaignId, Guid? periodId, Guid memberId, DomainContext domain)
        {
            NormalResult<Campaign_ShakingLotteryGiftEntity> result = new NormalResult<Campaign_ShakingLotteryGiftEntity>();

            CampaignEntity campaign = _campaignManager.GetCampaign(campaignId);
            if (campaign == null)
            {
                result.Message = "指定的活动不存在。";
                result.Success = false;
                return result;
            }

            //状态是否进行中
            switch (campaign.Status)
            {
                case EnumCampaignStatus.Preparatory:
                    result.Message = "活动尚未开始。";
                    result.Success = false;
                    return result;
                case EnumCampaignStatus.End:
                    result.Message = "活动已结束。";
                    result.Success = false;
                    return result;
            }

            Campaign_ShakingLotteryEntity shakingLottery = Get(campaignId);

            if (shakingLottery == null)
            {
                result.Message = "指定的活动不存在。";
                result.Success = false;
                return result;
            }

            if (shakingLottery.Mode == EnumCampaign_ShakingLotteryMode.Period)
            {
                if (shakingLottery.Period.HasValue == false)
                {
                    result.Message = "摇奖尚未开始~~";
                    result.Success = false;
                    return result;
                }

                //直接参与最新一期
                periodId = shakingLottery.Period;
            }
            else
            {
                if (shakingLottery.Started == false)
                {
                    result.Message = "摇奖尚未开始~~";
                    result.Success = false;
                    return result;
                }
            }

            //判断是否摇过了
            int playedTimes = GetMemberPlayedTimes(campaignId, periodId, memberId);
            if (playedTimes >= shakingLottery.ChanceTimes)
            {
                if (shakingLottery.Mode == EnumCampaign_ShakingLotteryMode.Period)
                {
                    result.Message = "您已经用本轮完全部参与机会~";
                }
                else
                {
                    result.Message = "您已经用完全部参与机会~";
                }

                result.Success = false;
                return result;
            }
            //////

            Campaign_ShakingLotteryLogEntity log = new Campaign_ShakingLotteryLogEntity();
            log.CampaignId = campaignId;
            log.Period = periodId;
            log.Domain = domain.Domain.Id;
            log.AppId = domain.AppId;
            log.Member = memberId;
            log.Time = DateTime.Now;

            //判断是否已经中奖了
            if (GetMemberObtainedGiftList(campaignId, memberId).Count > 0)
            {
                //同一个摇奖活动，只允许中一个奖，但是还有次数或新的周期开始时
                //还是允许他继续摇，假装还能参与
                //result.Message = "您已中奖~";
                //扣掉摇奖机会
                log.Win = false;
                _campaignManager.DataBase.Insert(log);

                result.Message = "遗憾您没有摇中，请再接再厉~";
                result.Success = false;
                return result;
            }

            Campaign_ShakingLotteryGiftPool pool = GetGiftPool(campaignId, periodId);

            if (pool == null)
            {
                _campaignManager.Log.Write("没有取到 GiftPool",
                    String.Format("campaignId:{0}，periodId:{1}", campaignId, periodId), TraceEventType.Error);
                result.Message = "遗憾您没有摇中，请再接再厉~~";
                result.Success = false;
                return result;
            }

            Campaign_ShakingLotteryGiftEntity gift = pool.GetGift();

            if (gift == null)
            {
                result.Message = "遗憾您没有摇中，请再接再厉！~";
                result.Success = false;
                return result;
            }

            log.Gift = gift.Id;

            if (gift.IsGift == false || gift.Stock <= 0)
            {
                log.Win = false;
            }
            else
            {
                //尝试扣减库存
                List<CommandParameter> parameterList = new List<CommandParameter>();
                parameterList.Add(new CommandParameter("@giftId", gift.Id));

                DataSet dsResult =
                    _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure, "DecrementShakingLotteryGiftStock",
                    parameterList, new string[] { "result" });

                result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());
                if (result.Reason == 0)
                {
                    log.Win = true;
                }
                else
                {
                    log.Win = false;
                }

                if (result.Reason != 2)
                {
                    gift.Stock = int.Parse(dsResult.Tables[0].Rows[0]["Stock"].ToString());
                }
            }

            _campaignManager.DataBase.Insert(log);

            if (log.Win)
            {
                result.Data = gift;
                result.Success = true;
                return result;
            }
            else
            {
                result.Message = "遗憾您没有摇中，请再接再厉~";
                result.Success = false;
                return result;
            }
        }

        public int GetMemberPlayedTimes(Guid campaignId, Guid? periodId, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));
            parameterList.Add(new CommandParameter("@memberId", memberId));
            if (periodId.HasValue)
            {
                parameterList.Add(new CommandParameter("@periodId", periodId.Value));
            }
            else
            {
                parameterList.Add(new CommandParameter("@periodId", DBNull.Value));
            }

            int intCount = 0;
            _campaignManager.DataBase.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM [Campaign_ShakingLotteryLog] WHERE [CampaignId] = @campaignId AND [Period] = @periodId AND [Member] = @memberId",
                parameterList, (scalarValue) => { intCount = scalarValue; });

            return intCount;
        }

        public int GetMemberPlayedTimes(Guid campaignId, Guid periodId, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));
            parameterList.Add(new CommandParameter("@periodId", periodId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            int intCount = 0;
            _campaignManager.DataBase.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM [Campaign_ShakingLotteryLog] WHERE [CampaignId] = @campaignId AND [Period] = @periodId AND [Member] = @memberId",
                parameterList, (scalarValue) => { intCount = scalarValue; });

            return intCount;
        }

        /// <summary>
        /// 获取指定会员获得奖品列表
        /// </summary>
        /// <param name="campaignId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public List<Campaign_ShakingLotteryGiftEntity> GetMemberObtainedGiftList(Guid campaignId, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            DataSet dsResult =
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure,
                "GetCampaign_ShakingLottery_MemberObtainedGiftList", parameterList, new string[] { "result" });

            List<Campaign_ShakingLotteryGiftEntity> list =
                RelationalMappingUnity.Select<Campaign_ShakingLotteryGiftEntity>(dsResult.Tables[0]);

            return list;
        }

        public GetItemListResult GetGiftWinningList(GetCampaign_ShakingLotteryGiftWinningListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            parameterList.Add(new CommandParameter("@campaignId", args.CampaignId));

            if (args.PeriodId.HasValue)
            {
                parameterList.Add(new CommandParameter("@periodId", args.PeriodId.Value));
            }
            else
            {
                parameterList.Add(new CommandParameter("@periodId", DBNull.Value));
            }

            DataSet dsResult =
                _campaignManager.DataBase.ExecuteDataSet(CommandType.StoredProcedure,
                "GetCampaign_ShakingLotteryGiftWinningList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetGiftWinningList(args);
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

        public void StartShaking(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _campaignManager.DataBase.ExecuteNonQuery(
                "UPDATE [Campaign_ShakingLottery] SET [Started] = 1 WHERE [CampaignId] = @id",
                parameterList);
        }

        public void EndShaking(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _campaignManager.DataBase.ExecuteNonQuery(
                "UPDATE [Campaign_ShakingLottery] SET [Started] = 0 WHERE [CampaignId] = @id",
                parameterList);
        }

        public void ClearWinning(Guid campaignId, Guid domainId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@campaignId", campaignId));
            parameterList.Add(new CommandParameter("@domainId", domainId));

            _campaignManager.DataBase.ExecuteNonQuery(
                "DELETE FROM [Campaign_ShakingLotteryLog] WHERE [CampaignId] = @campaignId AND [Domain] = @domainId",
                parameterList);
        }

        public ShakingLotteryGiftProjectiveData GetProjectiveData(GetCampaign_ShakingLotteryGiftWinningListArgs args)
        {
            ShakingLotteryGiftProjectiveData data = new ShakingLotteryGiftProjectiveData();

            Campaign_ShakingLotteryEntity campaign = Get(args.CampaignId);

            if (campaign.Mode == EnumCampaign_ShakingLotteryMode.Period)
            {
                args.PeriodId = campaign.Period;
                if (campaign.Period.HasValue)
                {
                    data.CurrentPeriod = GetPeriod(campaign.Period.Value);
                }
            }

            GetItemListResult winningList = GetGiftWinningList(args);

            data.WinningList = winningList;

            return data;

        }
    }
}
