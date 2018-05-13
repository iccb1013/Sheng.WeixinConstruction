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
    public class PageManager
    {
        private static readonly PageManager _instance = new PageManager();
        public static PageManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private static MemberManager _memberManager = MemberManager.Instance;
        private static ShareManager _shareManager = ShareManager.Instance;

        private PageManager()
        {

        }

        public void CreatePage(PageEntity page)
        {
            if (page == null)
            {
                Debug.Assert(false, "page 为空");
                return;
            }

            page.CreateTime = DateTime.Now;
            page.UpdateTime = DateTime.Now;
            _dataBase.Insert(page);
        }

        public void UpdatePage(PageEntity page)
        {
            if (page == null)
            {
                Debug.Assert(false, "page 为空");
                return;
            }

            _dataBase.Update(page);
        }

        public void RemovePage(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("DELETE FROM [Page] WHERE [Id] = @id", parameterList);
        }

        public PageEntity GetPage(Guid id)
        {
            PageEntity page = new PageEntity();
            page.Id = id;

            if (_dataBase.Fill<PageEntity>(page))
                return page;
            else
                return null;
        }

        public GetItemListResult GetPageList(GetItemListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.DomainId));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetPageList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetPageList(args);
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

        public void PageVisit(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("UPDATE [Page] SET [PageVisitCount] = [PageVisitCount] + 1 WHERE [Id] = @id",
                parameterList);

        }

        public ShareResult ShareTimeline(Guid pageId, Guid? memberId, string openId)
        {
            ShareResult result = new ShareResult();

            PageEntity page = GetPage(pageId);
            if (page == null)
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

            if (page.ShareTimelinePoint <= 0)
                return result;

            if (memberId.HasValue)
            {
                PointTrackArgs args = new PointTrackArgs();
                args.DomainId = page.Domain;
                args.MemberId = memberId.Value;
                args.Quantity = page.ShareTimelinePoint;
                args.Type = MemberPointTrackType.Share;
                args.TagName = page.Title;
                args.TagId = page.Id;

                PointTrackResult pointTrackResult = _memberManager.PointTrack(args);
                if (pointTrackResult.Success == false)
                    return result;

                result.Point = page.ShareTimelinePoint;
            }
            
            return result;
        }

        public ShareResult ShareAppMessage(Guid pageId, Guid? memberId, string openId)
        {
            ShareResult result = new ShareResult();

            PageEntity page = GetPage(pageId);
            if (page == null)
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

            if (page.ShareAppMessagePoint <= 0)
                return result;

            if (memberId.HasValue)
            {
                PointTrackArgs args = new PointTrackArgs();
                args.DomainId = page.Domain;
                args.MemberId = memberId.Value;
                args.Quantity = page.ShareAppMessagePoint;
                args.Type = MemberPointTrackType.Share;
                args.TagName = page.Title;
                args.TagId = page.Id;

                PointTrackResult pointTrackResult = _memberManager.PointTrack(args);
                if (pointTrackResult.Success == false)
                    return result;

                result.Point = page.ShareAppMessagePoint;
            }

            return result;
        }
    }
}
