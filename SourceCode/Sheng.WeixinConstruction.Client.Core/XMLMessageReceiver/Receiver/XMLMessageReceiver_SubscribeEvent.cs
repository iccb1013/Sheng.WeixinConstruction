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


using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Client.Core
{
    class XMLMessageReceiver_SubscribeEvent : XMLMessageReceiver<ReceivingXMLMessage_SubscribeEventMessage>
    {
        private static readonly ScenicQRCodeManager _scenicQRCodeManager = ScenicQRCodeManager.Instance;

        public XMLMessageReceiver_SubscribeEvent()
        {
            MsgType = "event";
            Event = "subscribe";
        }

        protected override string Handle(ReceivingXMLMessage_SubscribeEventMessage message, ClientDomainContext domainContext)
        {
            //用户管理-获取用户基本信息
            //未认证订阅号 未认证服务号 没有此权限

            RequestApiResult<WeixinUser> getUserInfoResult =
                UserApiWrapper.GetUserInfo(domainContext, message.FromUserName);
            if (getUserInfoResult.Success == false)
            {
                return String.Empty;
            }

            if (getUserInfoResult.ApiResult.Subscribe == 0)
            {
                return String.Empty;
            }
          
            #region 判断是否是扫描带参数二维码事件

            //扫描带参数二维码事件
            string strScenicQRCodeId = String.Empty;
            if (String.IsNullOrEmpty(message.EventKey) == false)
            {
                //用户未关注时，进行关注后的事件推送
                if (message.EventKey.StartsWith("qrscene_"))
                {
                    strScenicQRCodeId = message.EventKey.Remove(0, "qrscene_".Length);
                }
            }

            //获取场景二维码Id
            Guid? scenicQRCodeId = null;
            ScenicQRCodeLandingLogEntity scenicQRCodeLandingLog = null;
            if (String.IsNullOrEmpty(strScenicQRCodeId) == false)
            {
                Guid scenicQRCodeId2;
                if (Guid.TryParse(strScenicQRCodeId, out scenicQRCodeId2))
                {
                    scenicQRCodeId = scenicQRCodeId2;

                    scenicQRCodeLandingLog = new ScenicQRCodeLandingLogEntity();
                    scenicQRCodeLandingLog.Domain = domainContext.Domain.Id;
                    scenicQRCodeLandingLog.QRCodeId = scenicQRCodeId.Value;
                    scenicQRCodeLandingLog.VisitorOpenId = message.FromUserName;
                    scenicQRCodeLandingLog.LandingTime = DateTime.Now;
                }
            }

            #endregion

            #region 判断是否是通过RecomendUrl引流关注的

            Guid? refereeMemberId = null;
            //如果有场景二维码ID就不用判断RecomendUrl了
            if (String.IsNullOrEmpty(strScenicQRCodeId))
            {
                RecommendUrlLogEntity recomendUrlLog = _recommendUrlManager.GetLogByOpenId(getUserInfoResult.ApiResult.OpenId);
                if (recomendUrlLog != null)
                    refereeMemberId = recomendUrlLog.UrlOwnMember;
            }

            #endregion

            //根据OpenId获取用户信息
            MemberEntity member = _memberManager.GetMemberByOpenId(domainContext.Domain.Id, domainContext.AppId, getUserInfoResult.ApiResult.OpenId);

            AddMemberArgs args = new AddMemberArgs();
            args.WeixinUser = getUserInfoResult.ApiResult;
            args.ScenicQRCodeId = scenicQRCodeId;
            args.RefereeMemberId = refereeMemberId;

            if (member == null)
            {
                //添加新用户
                member = _memberManager.AddMember(domainContext, args);

                //更新场景二维码计数
                if (scenicQRCodeId.HasValue)
                {
                    _scenicQRCodeManager.IncrementAttentionPerson(scenicQRCodeId.Value, scenicQRCodeLandingLog);
                }

                //如果通过RecomendUrl引流关注的，奖励积分
                if (refereeMemberId.HasValue && domainContext.RecommendUrlSettings != null 
                    && domainContext.RecommendUrlSettings.AttentionPoint > 0)
                {
                    PointTrackArgs pointTrackArgs = new PointTrackArgs();
                    pointTrackArgs.DomainId = domainContext.Domain.Id;
                    pointTrackArgs.MemberId = refereeMemberId.Value;
                    pointTrackArgs.Quantity = domainContext.RecommendUrlSettings.AttentionPoint;
                    pointTrackArgs.Type = MemberPointTrackType.RecommendUrl;
                    pointTrackArgs.TagName = member.NickName;
                    pointTrackArgs.TagId = member.Id;
                    _memberManager.PointTrack(pointTrackArgs);
                }

                //判断是否有二级上线，对其奖励积分
                if (refereeMemberId.HasValue && domainContext.RecommendUrlSettings != null
                   && domainContext.RecommendUrlSettings.Level2AttentionPoint > 0)
                {
                    RefereeUplineWrapper refereeUplineWrapper =
                       _recommendUrlManager.GetRefereeUplineList(domainContext.Domain.Id, domainContext.AppId, refereeMemberId.Value);
                    if (refereeUplineWrapper.Upline != null)
                    {
                        PointTrackArgs pointTrackArgs = new PointTrackArgs();
                        pointTrackArgs.DomainId = domainContext.Domain.Id;
                        pointTrackArgs.MemberId = refereeUplineWrapper.Upline.MemberId;
                        pointTrackArgs.Quantity = domainContext.RecommendUrlSettings.Level2AttentionPoint;
                        pointTrackArgs.Type = MemberPointTrackType.RecommendUrl;
                        pointTrackArgs.TagName = member.NickName;
                        pointTrackArgs.TagId = member.Id;
                        _memberManager.PointTrack(pointTrackArgs);
                    }
                }
            }
            else
            {
                //更新当前用户信息
                _memberManager.UpdateMember(member, args);

                //更新场景二维码计数
                if (scenicQRCodeId.HasValue)
                {
                    _scenicQRCodeManager.IncrementLanding(scenicQRCodeId.Value, scenicQRCodeLandingLog);
                }
            }

            #region 关注时的自动回复

            if (domainContext.AutoReplyOnSubscribe != null
                && String.IsNullOrEmpty(domainContext.AutoReplyOnSubscribe.Content) == false)
            {
                ResponsiveXMLMessageBase replyMessage =
                    AutoReplyHelper.GetXMLMessage(domainContext, message.FromUserName, domainContext.AutoReplyOnSubscribe);

                if (replyMessage != null)
                    return XMLMessageHelper.XmlSerialize(replyMessage);
                else
                    return String.Empty;
            }
            else
            {
                return String.Empty;
            }

            #endregion

        }
    }
}
