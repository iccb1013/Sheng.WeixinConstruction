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
    class XMLMessageReceiver_LocationMessage : XMLMessageReceiver<ReceivingXMLMessage_LocationMessage>
    {
        public XMLMessageReceiver_LocationMessage()
        {
            MsgType = "location";
        }

        protected override string Handle(ReceivingXMLMessage_LocationMessage message, ClientDomainContext domainContext)
        {
            //存储消息记录
            MessageEntity messageEntity = new MessageEntity();
            messageEntity.Receive = true;
            messageEntity.ReceivingMessageType = EnumReceivingMessageType.Location;

            messageEntity.OfficalWeixinId = message.ToUserName;
            messageEntity.MemberOpenId = message.FromUserName;
            messageEntity.CreateTime = WeixinApiHelper.ConvertIntToDateTime(message.CreateTime);
            messageEntity.MsgId = message.MsgId;

            messageEntity.Location_X = message.Location_X;
            messageEntity.Location_Y = message.Location_Y;
            messageEntity.Location_Scale = message.Scale;
            messageEntity.Location_Label = message.Label;

            _messageManager.AddMessage(domainContext.Domain.Id, domainContext.AppId, messageEntity);

            return null;
        }
    }
}
