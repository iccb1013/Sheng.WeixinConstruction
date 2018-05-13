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


using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Areas.Api.Controllers
{
    public class MemberController : BasalController
    {
        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly MemberManager _memberManager = MemberManager.Instance;
        private static readonly MemberGroupManager _memberGroupManager = MemberGroupManager.Instance;
        private static readonly MemberTagManager _memberTagManager = MemberTagManager.Instance;
        private static readonly GroupMessageManager _groupMessageManager = GroupMessageManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        #region Member

        public ActionResult GetMemberList()
        {
            GetMemberListArgs args = RequestArgs<GetMemberListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult result =
                _memberManager.GetMemberList(UserContext.User.Domain, DomainContext.AppId, args);
            return RespondDataResult(result);
        }

        public ActionResult GetMember()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            MemberEntity member = _memberManager.GetMember(id);

            return RespondDataResult(member);
        }

        public ActionResult GetMemberByCardNumber()
        {
            string cardNumber = Request.QueryString["cardNumber"];

            if (String.IsNullOrEmpty(cardNumber))
            {
                return RespondResult(false, "参数无效。");
            }

            MemberEntity member =
                _memberManager.GetMemberByCardNumber(DomainContext.Domain.Id, DomainContext.AppId, cardNumber);

            if (member == null)
            {
                return RespondResult(false, "会员不存在。");
            }
            else
            {
                return RespondDataResult(member);
            }
        }

        public ActionResult PointIncrement()
        {
            PointTrackArgs args = RequestArgs<PointTrackArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.Type = MemberPointTrackType.UserOperate;
            args.OperatorUser = UserContext.User.Id;

            PointTrackResult pointTrackResult = _memberManager.PointTrack(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "增加用户积分"
            });

            #endregion

            return RespondDataResult(pointTrackResult);
        }

        public ActionResult PointDecrement()
        {
            PointTrackArgs args = RequestArgs<PointTrackArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = DomainContext.Domain.Id;
            args.Type = MemberPointTrackType.UserOperate;
            args.OperatorUser = UserContext.User.Id;

            PointTrackResult pointTrackResult = _memberManager.PointTrack(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "消减用户积分"
            });

            #endregion

            return RespondDataResult(pointTrackResult);
        }

        #endregion

        #region MemberGroup

        public ActionResult GetMemberGroupList()
        {
            List<MemberGroupEntity> list = _memberGroupManager.GetMemberGroupList(DomainContext.Domain.Id, DomainContext.AppId);
            return RespondDataResult(list);
        }

        public ActionResult GetMemberGroup()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            MemberGroupEntity memberGroup = _memberGroupManager.GetMemberGroup(id);

            return RespondDataResult(memberGroup);
        }

        public ActionResult CreateMemberGroup()
        {
            MemberGroupEntity memberGroup = RequestArgs<MemberGroupEntity>();
            if (memberGroup == null)
            {
                return RespondResult(false, "参数无效。");
            }

            memberGroup.Id = Guid.NewGuid();
            memberGroup.Domain = UserContext.User.Domain;
            memberGroup.AppId = DomainContext.AppId;
            string result = _memberGroupManager.CreateMemberGroup(DomainContext, memberGroup);

            ApiResult apiResult = new ApiResult();
            if (String.IsNullOrEmpty(result))
            {
                apiResult.Success = true;
            }
            else
            {
                apiResult.Message = result;
            }

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "添加用户分组"
            });

            #endregion

            return RespondResult(apiResult);
        }

        public ActionResult UpdateMemberGroup()
        {
            MemberGroupEntity memberGroup = RequestArgs<MemberGroupEntity>();
            if (memberGroup == null)
            {
                return RespondResult(false, "参数无效。");
            }

            memberGroup.Domain = UserContext.User.Domain;
            memberGroup.AppId = DomainContext.AppId;
            string result = _memberGroupManager.UpdateMemberGroup(DomainContext, memberGroup);

            ApiResult apiResult = new ApiResult();
            if (String.IsNullOrEmpty(result))
            {
                apiResult.Success = true;
            }
            else
            {
                apiResult.Message = result;
            }

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "更新用户分组"
            });

            #endregion

            return RespondResult(apiResult);
        }

        public ActionResult RemoveMemberGroup()
        {
            string id = Request.QueryString["id"];

            if (String.IsNullOrEmpty(id))
            {
                return RespondResult(false, "参数无效。");
            }

            string result = _memberGroupManager.RemoveMemberGroup(DomainContext, Guid.Parse(id));

            ApiResult apiResult = new ApiResult();
            if (String.IsNullOrEmpty(result))
            {
                apiResult.Success = true;
            }
            else
            {
                apiResult.Message = result;
            }

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "删除用户分组"
            });

            #endregion

            return RespondResult(apiResult);
        }

        public ActionResult MoveMemberListToGroup()
        {

            MoveMemberListToGroupArgs args = RequestArgs<MoveMemberListToGroupArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            string result = _memberGroupManager.MoveMemberListToGroup(DomainContext, args);

            ApiResult apiResult = new ApiResult();
            if (String.IsNullOrEmpty(result))
            {
                apiResult.Success = true;
            }
            else
            {
                apiResult.Message = result;
            }

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "设置用户所属分组"
            });

            #endregion

            return RespondResult(apiResult);
        }

        #endregion

        #region MemberTag

        public ActionResult GetMemberTagList()
        {
            List<MemberTagEntity> list = _memberTagManager.GetMemberTagList(DomainContext.Domain.Id, DomainContext.AppId);
            return RespondDataResult(list);
        }

        public ActionResult GetMemberTag()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            MemberTagEntity memberTag = _memberTagManager.GetMemberTag(id);

            return RespondDataResult(memberTag);
        }

        public ActionResult CreateMemberTag()
        {
            MemberTagEntity memberTag = RequestArgs<MemberTagEntity>();
            if (memberTag == null)
            {
                return RespondResult(false, "参数无效。");
            }

            memberTag.Id = Guid.NewGuid();
            memberTag.Domain = UserContext.User.Domain;
            memberTag.AppId = DomainContext.AppId;
            NormalResult result = _memberTagManager.CreateMemberTag(DomainContext, memberTag);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "添加用户标签"
            });

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult UpdateMemberTag()
        {
            MemberTagEntity memberTag = RequestArgs<MemberTagEntity>();
            if (memberTag == null)
            {
                return RespondResult(false, "参数无效。");
            }

            memberTag.Domain = UserContext.User.Domain;
            memberTag.AppId = DomainContext.AppId;
            NormalResult result = _memberTagManager.UpdateMemberTag(DomainContext, memberTag);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "更新用户标签"
            });

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult RemoveMemberTag()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            NormalResult result = _memberTagManager.RemoveMemberTag(DomainContext, id);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "删除用户分组"
            });

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult BatchTagging()
        {
            MemberBatchTaggingArgs args = RequestArgs<MemberBatchTaggingArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            NormalResult result = _memberTagManager.BatchTagging(DomainContext, args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "设置用户的标签"
            });

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        public ActionResult BatchUntagging()
        {
            MemberBatchTaggingArgs args = RequestArgs<MemberBatchTaggingArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            NormalResult result = _memberTagManager.BatchUntagging(DomainContext, args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "设置用户的标签"
            });

            #endregion

            return RespondResult(result.Success, result.Message);
        }

        #endregion

        #region GroupMessage

        public ActionResult SendGroupMessage()
        {
            SendGroupMessageArgs args = RequestArgs<SendGroupMessageArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            string result = _groupMessageManager.SendGroupMessage(DomainContext, args);

            ApiResult apiResult = new ApiResult();
            if (String.IsNullOrEmpty(result))
            {
                apiResult.Success = true;
            }
            else
            {
                apiResult.Message = result;
            }

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "群发消息"
            });

            #endregion

            return RespondResult(apiResult);
        }

        public ActionResult GetSentGroupMessageList()
        {
            GetItemListArgs args = RequestArgs<GetItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            GetItemListResult result =
                _groupMessageManager.GetSentGroupMessageList(UserContext.User.Domain, DomainContext.AppId, args);
            return RespondDataResult(result);
        }

        #endregion

        #region MemberCard

        public ActionResult GetMemberCardList()
        {
            List<MemberCardLevelEntity> result =
                _memberManager.GetMemberCardList(DomainContext.Domain.Id, DomainContext.AppId);
            return RespondDataResult(result);
        }

        public ActionResult GetMemberCard()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            MemberCardLevelEntity memberGroup = _memberManager.GetMemberCard(id);

            return RespondDataResult(memberGroup);
        }

        public ActionResult CreateMemberCard()
        {
            MemberCardLevelEntity memberCard = RequestArgs<MemberCardLevelEntity>();
            if (memberCard == null)
            {
                return RespondResult(false, "参数无效。");
            }

            memberCard.Domain = UserContext.User.Domain;
            memberCard.AppId = DomainContext.AppId;
            _memberManager.CreateMemberCard(memberCard);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "创建会员卡"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult UpdateMemberCard()
        {
            MemberCardLevelEntity memberCard = RequestArgs<MemberCardLevelEntity>();
            if (memberCard == null)
            {
                return RespondResult(false, "参数无效。");
            }

            memberCard.Domain = UserContext.User.Domain;
            memberCard.AppId = DomainContext.AppId;
            _memberManager.UpdateMemberCard(memberCard);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "更新会员卡"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult RemoveMemberCard()
        {
            string strId = Request.QueryString["id"];
            Guid id = Guid.Empty;
            if (String.IsNullOrEmpty(strId) || Guid.TryParse(strId, out id) == false)
            {
                return RespondResult(false, "参数无效。");
            }

            _memberManager.RemoveMemberCard(id);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "删除会员卡"
            });

            #endregion

            return RespondResult();
        }

        public ActionResult SetMemberLevel()
        {
            SetMemberLevelArgs args = RequestArgs<SetMemberLevelArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;

            _memberManager.SetMemberLevel(args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Member,
                Description = "设置会员的会员卡级别"
            });

            #endregion

            return RespondResult();
        }

        #endregion
    }
}