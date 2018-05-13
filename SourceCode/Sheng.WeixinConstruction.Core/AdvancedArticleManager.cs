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
    public class AdvancedArticleManager
    {
        private static readonly AdvancedArticleManager _instance = new AdvancedArticleManager();
        public static AdvancedArticleManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private static MemberManager _memberManager = MemberManager.Instance;
        private static ShareManager _shareManager = ShareManager.Instance;

        private AdvancedArticleManager()
        {

        }

        public void CreateAdvancedArticle(AdvancedArticleEntity advancedArticle)
        {
            if (advancedArticle == null)
            {
                Debug.Assert(false, "advancedArticle 为空");
                return;
            }

            advancedArticle.CreateTime = DateTime.Now;
            advancedArticle.UpdateTime = DateTime.Now;
            _dataBase.Insert(advancedArticle);
        }

        public void UpdateAdvancedArticle(AdvancedArticleEntity advancedArticle)
        {
            if (advancedArticle == null)
            {
                Debug.Assert(false, "advancedArticle 为空");
                return;
            }

            _dataBase.Update(advancedArticle);
        }

        public void RemoveAdvancedArticle(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("DELETE FROM [AdvancedArticle] WHERE [Id] = @id", parameterList);
        }

        public AdvancedArticleEntity GetAdvancedArticle(Guid id)
        {
            AdvancedArticleEntity advancedArticle = new AdvancedArticleEntity();
            advancedArticle.Id = id;

            if (_dataBase.Fill<AdvancedArticleEntity>(advancedArticle))
                return advancedArticle;
            else
                return null;
        }

        public GetItemListResult GetAdvancedArticleList(GetItemListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetAdvancedArticleList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetAdvancedArticleList(args);
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

        public void AdvancedArticleVisit(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("UPDATE [AdvancedArticle] SET [PageVisitCount] = [PageVisitCount] + 1 WHERE [Id] = @id",
                parameterList);

        }

        public ShareResult ShareTimeline(Guid pageId, Guid? memberId, string openId)
        {
            ShareResult result = new ShareResult();

            AdvancedArticleEntity advancedArticle = GetAdvancedArticle(pageId);
            if (advancedArticle == null)
                return result;

            #region 判断有没有分享过

            ShareLogEntity log = _shareManager.GetShareLog(pageId, openId);
            if (log == null)
            {
                log = new ShareLogEntity();
                log.Member = memberId;
                log.OpenId = openId;
                log.PageId = pageId;
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

            if (advancedArticle.ShareTimelinePoint <= 0)
                return result;

            if (memberId.HasValue)
            {
                PointTrackArgs args = new PointTrackArgs();
                args.DomainId = advancedArticle.Domain;
                args.MemberId = memberId.Value;
                args.Quantity = advancedArticle.ShareTimelinePoint;
                args.Type = MemberPointTrackType.Share;
                args.TagName = advancedArticle.Title;
                args.TagId = advancedArticle.Id;

                PointTrackResult pointTrackResult = _memberManager.PointTrack(args);
                if (pointTrackResult.Success == false)
                    return result;

                result.Point = advancedArticle.ShareTimelinePoint;
            }
            
            return result;
        }

        public ShareResult ShareAppMessage(Guid pageId, Guid? memberId, string openId)
        {
            ShareResult result = new ShareResult();

            AdvancedArticleEntity advancedArticle = GetAdvancedArticle(pageId);
            if (advancedArticle == null)
                return result;

            #region 判断有没有分享过

            ShareLogEntity log = _shareManager.GetShareLog(pageId, openId);
            if (log == null)
            {
                log = new ShareLogEntity();
                log.Member = memberId;
                log.OpenId = openId;
                log.PageId = pageId;
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

            if (advancedArticle.ShareAppMessagePoint <= 0)
                return result;

            if (memberId.HasValue)
            {
                PointTrackArgs args = new PointTrackArgs();
                args.DomainId = advancedArticle.Domain;
                args.MemberId = memberId.Value;
                args.Quantity = advancedArticle.ShareAppMessagePoint;
                args.Type = MemberPointTrackType.Share;
                args.TagName = advancedArticle.Title;
                args.TagId = advancedArticle.Id;

                PointTrackResult pointTrackResult = _memberManager.PointTrack(args);
                if (pointTrackResult.Success == false)
                    return result;

                result.Point = advancedArticle.ShareAppMessagePoint;
            }

            return result;
        }
    }
}
