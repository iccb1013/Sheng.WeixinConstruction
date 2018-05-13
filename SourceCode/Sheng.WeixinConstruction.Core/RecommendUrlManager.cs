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
    public class RecommendUrlManager
    {
        private static readonly RecommendUrlManager _instance = new RecommendUrlManager();
        public static RecommendUrlManager Instance
        {
            get { return _instance; }
        }

        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly SettingsManager _settingsManager = SettingsManager.Instance;

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private RecommendUrlManager()
        {

        }

        public RecommendUrlSettingsEntity GetSettings(Guid domainId, string appId)
        {
            if (String.IsNullOrEmpty(appId))
                return null;

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));

            List<RecommendUrlSettingsEntity> list = _dataBase.Select<RecommendUrlSettingsEntity>(
                "SELECT * FROM [RecommendUrlSettings] WHERE [Domain] = @domainId AND [AppId] = @appId", parameterList);

            if (list.Count > 0)
            {
                return list[0];
            }

            _dataBase.ExecuteNonQuery(CommandType.StoredProcedure, "CreateRecommendUrlSettings", parameterList);

            RecommendUrlSettingsEntity settings = new RecommendUrlSettingsEntity()
            {
                Domain = domainId,
                AppId = appId
            };

            return settings;
        }

        public void SaveSettings(RecommendUrlSettingsEntity args)
        {
            _dataBase.Update(args);

            _domainManager.UpdateLastUpdateTime(args.Domain);
        }

        public RecommendUrlEntity Get(DomainContext domainContext, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainContext.Domain.Id));
            parameterList.Add(new CommandParameter("@appId", domainContext.AppId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            List<RecommendUrlEntity> list = _dataBase.Select<RecommendUrlEntity>(
                "SELECT * FROM [RecommendUrl] WHERE [Domain] = @domainId AND [AppId] = @appId AND [Member] = @memberId", parameterList);

            if (list.Count > 0)
            {
                return list[0];
            }

            Guid id = Guid.NewGuid();

            string url = "{0}Function/RecommendUrl/{1}?id={2}";
            url = String.Format(url, _settingsManager.GetClientAddress(domainContext.AppId), domainContext.Domain.Id, id);
            string shortUrl = String.Empty;
            RequestApiResult<WeixinCreateShortUrlResult> getShortUrlResult = ShortUrlApiWrapper.GetShortUrl(domainContext, url);
            if (getShortUrlResult.Success)
            {
                shortUrl = getShortUrlResult.ApiResult.ShortUrl;
            }

            parameterList.Add(new CommandParameter("@id", id));
            parameterList.Add(new CommandParameter("@url", url));
            parameterList.Add(new CommandParameter("@shortUrl", shortUrl));

            _dataBase.ExecuteNonQuery(CommandType.StoredProcedure, "CreateRecommendUrl", parameterList);

            RecommendUrlEntity recommendUrl = new RecommendUrlEntity()
            {
                Id = id,
                Domain = domainContext.Domain.Id,
                AppId = domainContext.AppId,
                Member = memberId,
                Url = url,
                ShortUrl = shortUrl
            };

            return recommendUrl;

        }

        public RecommendUrlEntity Get(Guid id)
        {
            RecommendUrlEntity recommendUrl = new RecommendUrlEntity();
            recommendUrl.Id = id;

            if (_dataBase.Fill<RecommendUrlEntity>(recommendUrl))
            {
                return recommendUrl;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 判断指定的OpenId是否存在浏览记录
        /// 返回最新的一条Log记录
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public RecommendUrlLogEntity GetLogByOpenId(string openId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@visitOpenId", openId));

            List<RecommendUrlLogEntity> list = _dataBase.Select<RecommendUrlLogEntity>(
                "SELECT TOP 1 * FROM [RecommendUrlLog] WHERE [VisitOpenId] = @visitOpenId ORDER BY [Time] DESC", parameterList);

            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 记录一次访问
        /// </summary>
        /// <param name="recommendUrlId"></param>
        /// <param name="openId"></param>
        public void Log(Guid recommendUrlId, string openId)
        {
            RecommendUrlEntity url = Get(recommendUrlId);
            if (url == null)
                return;

            RecommendUrlLogEntity log = new RecommendUrlLogEntity();
            log.Domain = url.Domain;
            log.AppId = url.AppId;
            log.UrlOwnMember = url.Member;
            log.VisitOpenId = openId;
            log.Time = DateTime.Now;
            _dataBase.Insert(log);

            //递增访问量计数
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", recommendUrlId));

            _dataBase.ExecuteNonQuery(
                "UPDATE [RecommendUrl] SET [LandingCount] = [LandingCount] + 1 WHERE [Id] = @id",
                parameterList);

        }

        /// <summary>
        /// 获取指定会员的1级下线人数
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public int GetLevel1DownlineCount(Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@refereeMemberId", memberId));

            int count = 0;
            _dataBase.ExecuteScalar<int>("SELECT COUNT(1) FROM [Member] WHERE [RefereeMemberId] = @refereeMemberId AND [Attention] = 1",
                parameterList, (scalarValue) => { count = scalarValue; });
            return count;
        }

        /// <summary>
        /// 获取指定会员的2级下线人数（不包括1级）
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public int GetLevel2DownlineCount(Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@memberId", memberId));
            parameterList.Add(new CommandParameter("@level", 2));

            int count = 0;
            _dataBase.ExecuteScalar<int>(CommandType.StoredProcedure, "GetRecommendSubLevelDownlineCount",
                parameterList, (scalarValue) => { count = scalarValue; });
            return count;
        }

        public GetItemListResult GetLevel1DownlineList(GetRecommendDownlineListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@memberId", args.MemberId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetRecommendLevel1DownlineList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetLevel1DownlineList(args);
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

        public GetItemListResult GetLevel2DownlineList(GetRecommendDownlineListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@memberId", args.MemberId));
            parameterList.Add(new CommandParameter("@level", 2));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetRecommendSubLevelDownlineList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetLevel2DownlineList(args);
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
        /// 获取上线列表
        /// Id,[RefereeMemberId],[Attention],Level
        /// 会员ID，上级会员ID，是否正在关注，层级
        /// Level 0 表示指定的会员，每加1表示向上一级
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="appId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public RefereeUplineWrapper GetRefereeUplineList(Guid domainId, string appId, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetRecommendRefereeUplineList", parameterList, new string[] { "result" });

            DataTable dt = dsResult.Tables[0];

            DataRow[] drArray = dt.Select("Level = 0");
            if (drArray.Length == 0)
                return null;

            RefereeUplineWrapper wrapper = RelationalMappingUnity.Select<RefereeUplineWrapper>(drArray[0]);
            RefereeUplineWrapper refereeWrapper = wrapper;
            int i = 1;
            while (true)
            {
                drArray = dt.Select("Level = " + i);
                if (drArray.Length == 0)
                    break;

                RefereeUplineWrapper newRefereeWrapper = RelationalMappingUnity.Select<RefereeUplineWrapper>(drArray[0]);
                refereeWrapper.Upline = newRefereeWrapper;
                refereeWrapper = newRefereeWrapper;
            }

            return wrapper;
        }
    }
}

//TODO:Referrer