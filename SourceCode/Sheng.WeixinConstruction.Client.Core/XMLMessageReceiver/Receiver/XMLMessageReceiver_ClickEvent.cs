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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Client.Core
{
    class XMLMessageReceiver_ClickEvent : XMLMessageReceiver<ReceivingXMLMessage_ClickEventMessage>
    {
        private static readonly CampaignManager _campaignManager = CampaignManager.Instance;

        public XMLMessageReceiver_ClickEvent()
        {
            MsgType = "event";
            Event = "CLICK";
        }

        protected override string Handle(ReceivingXMLMessage_ClickEventMessage message, ClientDomainContext domainContext)
        {
            if (String.IsNullOrEmpty(message.EventKey))
            {
                return String.Empty;
            }

            string[] keyArray = message.EventKey.Split(':');

            switch (keyArray[0])
            {
                case "MemberQRCode":
                    

                    Guid campaignId = Guid.Empty;
                    if (Guid.TryParse(keyArray[1], out campaignId) == false)
                    {
                        string replyMessage = GetReplyTextMessage(domainContext, message.FromUserName, "campaignId 参数无效：" + keyArray[1]);
                        return replyMessage;
                    }

                    //  Campaign_MemberQRCodeEntity campaign_MemberQRCodeEntity = _campaignManager.GetMemberQRCode();
                    MemberEntity memberEntity = _memberManager.GetMemberByOpenId(domainContext.Domain.Id, domainContext.AppId, message.FromUserName);
                    Campaign_MemberQRCodeItemEntity memberQRCodeItem =
                        _campaignManager.MemberQRCode.GetMemberQRCodeItem(Guid.Parse(keyArray[0]), memberEntity.Id);
                    if (memberQRCodeItem == null)
                    {

                    }
                    break;
                default:
                    break;
            }

            return String.Empty;
        }


    }
}
