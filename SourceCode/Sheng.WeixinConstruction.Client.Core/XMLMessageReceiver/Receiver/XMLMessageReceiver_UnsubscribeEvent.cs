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


using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Client.Core
{
    class XMLMessageReceiver_UnsubscribeEvent : XMLMessageReceiver<ReceivingXMLMessage_UnsubscribeEventMessage>
    {
        public XMLMessageReceiver_UnsubscribeEvent()
        {
            MsgType = "event";
            Event = "unsubscribe";
        }

        protected override string Handle(ReceivingXMLMessage_UnsubscribeEventMessage message, ClientDomainContext domainContext)
        {
            _memberManager.UnsubscribeMember(domainContext.Domain.Id, domainContext.AppId, message.FromUserName);

            //清除该用户当前Session
            SessionPool.Instance.Abandon(message.FromUserName);

            return String.Empty;
        }
    }
}
