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


using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Client.Core
{
    class XMLMessageReceiver_LinkMessage : XMLMessageReceiver<ReceivingXMLMessage_LinkMessage>
    {
        public XMLMessageReceiver_LinkMessage()
        {
            MsgType = "link";
        }

        protected override string Handle(ReceivingXMLMessage_LinkMessage message, ClientDomainContext domainContext)
        {
            //存储消息记录
            MessageEntity messageEntity = new MessageEntity();
            messageEntity.Receive = true;
            messageEntity.ReceivingMessageType = EnumReceivingMessageType.Link;

            messageEntity.OfficalWeixinId = message.ToUserName;
            messageEntity.MemberOpenId = message.FromUserName;
            messageEntity.CreateTime = WeixinApiHelper.ConvertIntToDateTime(message.CreateTime);
            messageEntity.MsgId = message.MsgId;

            messageEntity.Link_Title = message.Title;
            messageEntity.Link_Description = message.Description;
            messageEntity.Link_Url = message.Url;

            _messageManager.AddMessage(domainContext.Domain.Id, domainContext.AppId, messageEntity);

            return null;
        }
    }
}
