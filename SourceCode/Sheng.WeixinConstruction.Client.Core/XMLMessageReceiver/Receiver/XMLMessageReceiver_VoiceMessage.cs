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
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Client.Core.XMLMessageReceiver
{
    class XMLMessageReceiver_VoiceMessage : XMLMessageReceiver<ReceivingXMLMessage_VoiceMessage>
    {
        public XMLMessageReceiver_VoiceMessage()
        {
            MsgType = "voice";
        }

        protected override string Handle(ReceivingXMLMessage_VoiceMessage message, ClientDomainContext domainContext)
        {
            //存储消息记录
            MessageEntity messageEntity = new MessageEntity();
            messageEntity.Receive = true;
            messageEntity.ReceivingMessageType = EnumReceivingMessageType.Voice;

            messageEntity.OfficalWeixinId = message.ToUserName;
            messageEntity.MemberOpenId = message.FromUserName;
            messageEntity.CreateTime = WeixinApiHelper.ConvertIntToDateTime(message.CreateTime);
            messageEntity.MsgId = message.MsgId;

            messageEntity.Voice_Format = message.Format;
            messageEntity.MediaId = message.MediaId;

            _messageManager.AddMessage(domainContext.Domain.Id, domainContext.AppId, messageEntity);

            //向文件服务器发起文件异步下载请求
            FileDownloadQueueWithMediaIdArgs args = new FileDownloadQueueWithMediaIdArgs();
            args.Domain = domainContext.Domain.Id;
            args.AppId = domainContext.AppId;
            args.MediaId = message.MediaId;
            args.CallbackUrl = _fileDownloadCallbackUrl;
            args.Tag = messageEntity.Id.ToString();

            ApiResult apiResult = _fileService.DownloadQueueWithMediaId(args);
            if (apiResult.Success == false)
            {
                _log.Write("队列下载请求失败", apiResult.Message, TraceEventType.Warning);
            }

            return null;
        }

    }
}
