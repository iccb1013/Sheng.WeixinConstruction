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
    public class MemberManager
    {
        private static readonly MemberManager _instance = new MemberManager();
        public static MemberManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private object _lockObj = new object();

        private static readonly DomainManager _domainManager = DomainManager.Instance;

        private MemberManager()
        {

        }

        public MemberStatisticData GetMemberStatisticData(Guid domainId, string appId, DateTime startDate, DateTime endDate)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@startDate", startDate));
            parameterList.Add(new CommandParameter("@endDate", endDate));

            DataSet dsResult =
               _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetMemberStatisticData", parameterList,
               new string[] { "subscribeCount", "unsubscribeCount", "newAttentionCount", "totalAttentionCount","todaySigninCount" });

            MemberStatisticData result = new MemberStatisticData();

            if (dsResult.Tables["subscribeCount"].Rows.Count > 0)
            {
                string strCount = dsResult.Tables["subscribeCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strCount) == false)
                    result.SubscribeCount = int.Parse(strCount);
            }

            if (dsResult.Tables["unsubscribeCount"].Rows.Count > 0)
            {
                string strCount = dsResult.Tables["unsubscribeCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strCount) == false)
                    result.UnsubscribeCount = int.Parse(strCount);
            }

            if (dsResult.Tables["newAttentionCount"].Rows.Count > 0)
            {
                string strCount = dsResult.Tables["newAttentionCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strCount) == false)
                    result.NewAttentionCount = int.Parse(strCount);
            }

            if (dsResult.Tables["totalAttentionCount"].Rows.Count > 0)
            {
                string strCount = dsResult.Tables["totalAttentionCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strCount) == false)
                    result.TotalAttentionCount = int.Parse(strCount);
            }

            if (dsResult.Tables["todaySigninCount"].Rows.Count > 0)
            {
                string strCount = dsResult.Tables["todaySigninCount"].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(strCount) == false)
                    result.TodaySigninCount = int.Parse(strCount);
            }

            return result;
        }


        /// <summary>
        /// 生成一个会员卡号
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public string GetMemberCardNumber(Guid domainId, string appId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));

            string cardNumber = null;
            _dataBase.ExecuteScalar<string>(CommandType.StoredProcedure, "GetMemberCardNumber",
                parameterList, (scalarString) => { cardNumber = scalarString; });

            return cardNumber;
        }

        public GetItemListResult GetMemberList(Guid domainId, string appId, GetMemberListArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));
            //GroupId:-1 全部，-2 已取消关注
            if (args.GroupId != -1)
            {
                parameterList.Add(new CommandParameter("@groupId", args.GroupId));
            }
            else
            {
                parameterList.Add(new CommandParameter("@groupId", DBNull.Value));
            }
            if (String.IsNullOrEmpty(args.NickName) == false)
            {
                parameterList.Add(new CommandParameter("@nickName", args.NickName));
            }
            else
            {
                parameterList.Add(new CommandParameter("@nickName", DBNull.Value));
            }
            if (String.IsNullOrEmpty(args.MobilePhone) == false)
            {
                parameterList.Add(new CommandParameter("@mobilePhone", args.MobilePhone));
            }
            else
            {
                parameterList.Add(new CommandParameter("@mobilePhone", DBNull.Value));
            }
            if (String.IsNullOrEmpty(args.CardNumber) == false)
            {
                parameterList.Add(new CommandParameter("@cardNumber", args.CardNumber));
            }
            else
            {
                parameterList.Add(new CommandParameter("@cardNumber", DBNull.Value));
            }

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetMemberList", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetMemberList(domainId, appId, args);
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

        //TODO:增加domainId和appId参数，不是一个域的不给取，以增加安全性
        public MemberEntity GetMember(Guid id)
        {
            MemberEntity member = new MemberEntity();
            member.Id = id;

            if (_dataBase.Fill<MemberEntity>(member))
                return member;
            else
                return null;
        }

        public MemberEntity GetMemberByCardNumber(Guid domainId, string appId, string cardNumber)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@cardNumber", cardNumber));

            List<MemberEntity> memberList = _dataBase.Select<MemberEntity>(
                "SELECT * FROM [Member] WHERE [Domain] = @domainId AND [AppId] = @appId AND [CardNumber] = @cardNumber",
                parameterList);

            if (memberList.Count == 0)
                return null;
            else
                return memberList[0];
        }

        public MemberEntity GetMemberByOpenId(Guid domainId, string appId, string openId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@openId", openId));

            List<MemberEntity> memberList = _dataBase.Select<MemberEntity>(
                "SELECT * FROM [Member] WHERE [Domain] = @domainId  AND [AppId] = @appId  AND [OpenId] = @openId",
                parameterList);

            if (memberList.Count != 1)
                return null;
            else
                return memberList[0];
        }

        /// <summary>
        /// 预约更新用户信息的标记，由Windows服务更新
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="appId"></param>
        /// <param name="memberId"></param>
        public void NeedUpdate(Guid memberId, bool needUpdate)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            //parameterList.Add(new CommandParameter("@domainId", domainId));
            //parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@id", memberId));
            parameterList.Add(new CommandParameter("@needUpdate", needUpdate));

            _dataBase.ExecuteNonQuery(
                "UPDATE [Member] SET [NeedUpdate] = @needUpdate WHERE [Id] = @id",
                parameterList);
        }

        /// <summary>
        /// 获取需要更新的会员列表
        /// </summary>
        /// <returns></returns>
        public List<MemberEntity> GetNeedUpdateList()
        {
            List<MemberEntity> memberList = _dataBase.Select<MemberEntity>(
                "SELECT * FROM [Member] WHERE [NeedUpdate] = 1");

            return memberList;
        }

        public MemberPersonalInfo GetMemberPersonalInfo(Guid id)
        {
            MemberPersonalInfo memberPersonalInfo = new MemberPersonalInfo();
            memberPersonalInfo.Id = id;

            if (_dataBase.Fill<MemberPersonalInfo>(memberPersonalInfo))
                return memberPersonalInfo;
            else
                return null;
        }

        public int GetMemberPoint(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            object objPoint = _dataBase.ExecuteScalar("SELECT [Point] FROM [Member] WHERE [Id] = @id",
                parameterList);

            if (objPoint == null)
                return 0;
            else
                return int.Parse(objPoint.ToString());
        }

        /// <summary>
        /// 单位，分
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetMemberCashAccountBalances(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            object objCashAccountBalances = _dataBase.ExecuteScalar("SELECT [CashAccount] FROM [Member] WHERE [Id] = @id",
                parameterList);

            if (objCashAccountBalances == null)
                return 0;
            else
                return int.Parse(objCashAccountBalances.ToString());
        }

        public MemberEntity AddMember(DomainContext domainContext, AddMemberArgs args)
        {
            if (args == null)
            {
                Debug.Assert(false, "AddMember args == null");
                return null;
            }

            MemberEntity member = new MemberEntity();
            member.Domain = domainContext.Domain.Id;
            member.AppId = domainContext.AppId;
            member.OpenId = args.WeixinUser.OpenId;
            member.NickName = args.WeixinUser.Nickname;
            member.Sex = args.WeixinUser.Sex;
            member.City = args.WeixinUser.City;
            member.Country = args.WeixinUser.Country;
            member.Province = args.WeixinUser.Province;
            member.Language = args.WeixinUser.Language;
            member.Headimgurl = args.WeixinUser.Headimgurl;
            member.SubscribeTime = args.WeixinUser.SubscribeTime;
            member.Remark = args.WeixinUser.Remark;
            member.GroupId = args.WeixinUser.GroupId;
            member.TagList = args.WeixinUser.GetTagListString();
            member.ScenicQRCodeId = args.ScenicQRCodeId;
            member.RefereeMemberId = args.RefereeMemberId;
            member.Attention = true;
            member.Point = domainContext.InitialMemberPoint;
            member.CardNumber = GetMemberCardNumber(domainContext.Domain.Id, domainContext.AppId);
            member.SyncFlag = args.SyncFlag;

            //TODO:初始积分要写记录表

            _dataBase.Insert(member);

            return member;
        }

        public void UpdateMemberCenterQRCodeImageUrl(Guid memberId, string url)
        {
            SqlStructureBuild sqlBuild = new SqlStructureBuild();
            sqlBuild.Table = "Member";
            sqlBuild.Type = SqlExpressionType.Update;
            sqlBuild.AddParameter("Id", memberId, true);
            sqlBuild.AddParameter("MemberCenterQRCodeImageUrl", url);
            _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());
        }

        public void UpdateMember(MemberEntity member, AddMemberArgs args)
        {
            if (member == null || args == null)
            {
                Debug.Assert(false, "UpdateMember member== null || args == null");
                return;
            }

            member.NickName = args.WeixinUser.Nickname;
            member.Sex = args.WeixinUser.Sex;
            member.City = args.WeixinUser.City;
            member.Country = args.WeixinUser.Country;
            member.Province = args.WeixinUser.Province;
            member.Language = args.WeixinUser.Language;
            member.Headimgurl = args.WeixinUser.Headimgurl;
            member.SubscribeTime = args.WeixinUser.SubscribeTime;
            member.Remark = args.WeixinUser.Remark;
            member.GroupId = args.WeixinUser.GroupId;
            member.TagList = args.WeixinUser.GetTagListString();
            member.Attention = true;
            member.SyncFlag = args.SyncFlag;

            if (args.ScenicQRCodeId.HasValue)
                member.ScenicQRCodeId = args.ScenicQRCodeId;

            if (args.RefereeMemberId.HasValue)
                member.RefereeMemberId = args.RefereeMemberId;

            _dataBase.Update(member);
        }

        /// <summary>
        /// 剔出取消了关注的用户，将其关注状态置为取消关注
        /// </summary>
        /// <param name="syncFlag"></param>
        public void UpdateUnsubscribeMemberList(Guid domainId, string appId, DateTime syncFlag)
        {
            //这里不能简单的用_syncFlag反向查询，因为在同步过程中会产生新的关注者
            //查询时还需要带入关注时间，关注时间在_syncFlag之前的，且SyncFlag字段不等于_syncFlag的
            //全是取消了关注的
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@syncFlag", syncFlag));

            _dataBase.ExecuteNonQuery(
                "UPDATE [Member] SET [Attention] = 0 WHERE [Domain] = @domainId AND [AppId] = @appId  AND ( [SyncFlag] IS NULL OR ([SyncFlag] <> CONVERT(datetime,@syncFlag) AND [SubscribeTime] <= CONVERT(datetime,@syncFlag)))",
                parameterList);
        }

        /// <summary>
        /// 指定的用户取消了关注
        /// </summary>
        /// <param name="openId"></param>
        public void UnsubscribeMember(Guid domainId, string appId, string openId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@openId", openId));

            //GroupId:-2 表示已取消关注这一虚拟分组
            _dataBase.ExecuteNonQuery(
                "UPDATE [Member] SET [Attention] = 0,[GroupId]=-2,[UnsubscribeTime] = GETDATE() WHERE [Domain] = @domainId AND [AppId] = @appId  AND [OpenId] = @openId",
                parameterList);

        }

        /// <summary>
        /// 每日签到
        /// </summary>
        /// <param name="id"></param>
        public SignInResult SignIn(Guid domainId, string appId, Guid id)
        {
            SignInResult result = new SignInResult();

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@domainId", domainId));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "MemberSignIn", parameterList, new string[] { "result" });

            result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());

            if (result.Success)
            {
                //增加积分
                result.SignInPoint = int.Parse(dsResult.Tables[0].Rows[0]["SignInPoint"].ToString());
                result.Point = int.Parse(dsResult.Tables[0].Rows[0]["Point"].ToString());
            }

            return result;
        }

        /// <summary>
        /// 获取帐户信息（积分变化列化）
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetItemListResult GetPointAccount(GetPointAccountArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@memberId", args.MemberId));
            parameterList.Add(new CommandParameter("@page", args.Page));
            parameterList.Add(new CommandParameter("@pageSize", args.PageSize));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetPointAccount", parameterList, new string[] { "result" });

            if (dsResult.Tables[0].Rows.Count == 0 && args.Page > 1)
            {
                args.Page--;
                return GetPointAccount(args);
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

        public void UpdatePersonalInfo(UpdatePersonalInfoArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", args.MemberId));
            parameterList.Add(new CommandParameter("@name", args.Name));
            parameterList.Add(new CommandParameter("@birthday", args.Birthday));
            parameterList.Add(new CommandParameter("@mobilePhone", args.MobilePhone));
            parameterList.Add(new CommandParameter("@email", args.Email));

            _dataBase.ExecuteNonQuery(
                "UPDATE [Member] SET [Name] = @name,[Birthday] = @birthday,[MobilePhone] = @mobilePhone,[Email]=@email WHERE [Id] = @id",
                parameterList);
        }

        //TODO:积分变更记录中没有appid?
        /// <summary>
        /// 修改积分
        /// </summary>
        /// <param name="args"></param>
        public PointTrackResult PointTrack(PointTrackArgs args)
        {
            PointTrackResult result = new PointTrackResult();

            if (args == null)
            {
                return result;
            }

            if (args.Quantity == 0)
            {
                return result;
            }

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domain", args.DomainId));
            parameterList.Add(new CommandParameter("@memberId", args.MemberId));
            parameterList.Add(new CommandParameter("@quantity", args.Quantity));
            parameterList.Add(new CommandParameter("@type", (int)args.Type));
            parameterList.Add(new CommandParameter("@tagName", args.TagName));
            parameterList.Add(new CommandParameter("@tagId", args.TagId));
            parameterList.Add(new CommandParameter("@operatorUser", args.OperatorUser));
            parameterList.Add(new CommandParameter("@remark", args.Remark));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "TrackMemberPoint", parameterList, new string[] { "result" });

            //result.Reason = int.Parse(dsResult.Tables[0].Rows[0]["Result"].ToString());

            result.Success = true;
            result.LeftPoint = int.Parse(dsResult.Tables[0].Rows[0]["Point"].ToString());

            return result;
        }

        #region 会员卡

        public List<MemberCardLevelEntity> GetMemberCardList(Guid domainId, string appId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));

            List<MemberCardLevelEntity> list =
                _dataBase.Select<MemberCardLevelEntity>(
                "SELECT * FROM [MemberCardLevel] WHERE [Domain] = @domainId AND [AppId] = @appId",
                parameterList);

            return list;
        }

        public MemberCardLevelEntity GetMemberCard(Guid id)
        {
            MemberCardLevelEntity memberCard = new MemberCardLevelEntity();
            memberCard.Id = id;

            if (_dataBase.Fill<MemberCardLevelEntity>(memberCard))
                return memberCard;
            else
                return null;
        }

        public void CreateMemberCard(MemberCardLevelEntity memberCard)
        {
            if (memberCard == null)
            {
                Debug.Assert(false, "memberCard 为空");
                return;
            }

            _dataBase.Insert(memberCard);
        }

        public void UpdateMemberCard(MemberCardLevelEntity memberCard)
        {
            if (memberCard == null)
            {
                Debug.Assert(false, "memberCard 为空");
                return;
            }

            _dataBase.Update(memberCard);
        }

        public void RemoveMemberCard(Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("DELETE FROM [MemberCardLevel] WHERE [Id] = @id", parameterList);
        }

        public void SetMemberLevel(SetMemberLevelArgs args)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", args.Domain));
            parameterList.Add(new CommandParameter("@appId", args.AppId));
            parameterList.Add(new CommandParameter("@memberId", args.MemberId));
            parameterList.Add(new CommandParameter("@memberCardLevelId", args.MemberCardId));

            _dataBase.ExecuteNonQuery(
                "UPDATE [Member] SET [CardLevel] = @memberCardLevelId WHERE [Id] = @memberId AND [Domain] = @domainId AND [AppId] = @appId",
                parameterList);
        }



        #endregion

        //TODO:临时发红包用的方法
        public bool Redpack(Guid domainId, Guid memberId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domain", domainId));
            parameterList.Add(new CommandParameter("@memberId", memberId));

            object obj = _dataBase.ExecuteScalar(
                "SELECT Count(1) FROM [Redpack] WHERE [Domain] = @domain AND [Member] = @memberId", parameterList);

            if (obj.ToString() == "0")
            {
                _dataBase.ExecuteNonQuery("INSERT INTO [Redpack] ([Domain],[Member]) VALUES(@domain,@memberId)",
                    parameterList);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
