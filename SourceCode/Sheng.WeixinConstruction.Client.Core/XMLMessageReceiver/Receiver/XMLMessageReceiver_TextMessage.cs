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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Sheng.WeixinConstruction.Client.Core
{
    class XMLMessageReceiver_TextMessage : XMLMessageReceiver<ReceivingXMLMessage_TextMessage>
    {
        public XMLMessageReceiver_TextMessage()
        {
            MsgType = "text";
        }

        protected override string Handle(ReceivingXMLMessage_TextMessage message, ClientDomainContext domainContext)
        {
            //存储消息记录
            MessageEntity messageEntity = new MessageEntity();
            messageEntity.Receive = true;
            messageEntity.ReceivingMessageType = EnumReceivingMessageType.Text;

            messageEntity.OfficalWeixinId = message.ToUserName;
            messageEntity.MemberOpenId = message.FromUserName;
            messageEntity.CreateTime = WeixinApiHelper.ConvertIntToDateTime(message.CreateTime);
            messageEntity.MsgId = message.MsgId;

            messageEntity.Content = message.Content;
            _messageManager.AddMessage(domainContext.Domain.Id, domainContext.AppId, messageEntity);
            
            bool handled = false;
            string replyMessageString = null;

            //判断是否存在关键词自动回复
            if (domainContext.AutoReplyOnKeyWords != null && domainContext.AutoReplyOnKeyWords.RuleList != null
                && domainContext.AutoReplyOnKeyWords.RuleList.Count > 0)
            {
                //这里无法直接返回一个string去响应微信服务发出的http请求
                //因为那样的话只能回复一条消息，要回复多条消息必须得调用客服接口去发
                List<KFMessageBase> replyMessageList = domainContext.AutoReplyOnKeyWords.GetReplyMessage(domainContext, message);
                if (replyMessageList != null && replyMessageList.Count > 0)
                {
                    foreach (var item in replyMessageList)
                    {
                        KFApiWrapper.Send(domainContext, item);
                    }
                    handled = true;
                }
            }

            //如果没有命中关键字回复，则看有没有消息自动回复
            if (handled == false && domainContext.AutoReplyOnMessage != null)
            {
                ResponsiveXMLMessageBase replyMessage =
                    AutoReplyHelper.GetXMLMessage(domainContext, message.FromUserName, domainContext.AutoReplyOnMessage);

                if (replyMessage != null)
                {
                    handled = true;
                    replyMessageString = XMLMessageHelper.XmlSerialize(replyMessage);
                }
            }

            return replyMessageString;
        }
    }
}
