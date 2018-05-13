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


using Linkup.Common;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sheng.WeixinConstruction.Client.Core
{
    interface IXMLMessageReceiver
    {
        string MsgType { get; }

        //对于普通消息 MsgType 就要以区分消息的类型了
        //但是对于事件消息，需要通过另外一个字段来区分事件的类型
        string Event { get; }

        string Handle(string message, ClientDomainContext domainContext);
    }

    abstract class XMLMessageReceiver<T> : IXMLMessageReceiver where T : class
    {
        private static readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(T));

        protected static readonly LogService _log = LogService.Instance;
        protected static readonly MemberManager _memberManager = MemberManager.Instance;
        protected static readonly MessageManager _messageManager = MessageManager.Instance;
        protected static readonly RecommendUrlManager _recommendUrlManager = RecommendUrlManager.Instance;
        protected static readonly FileService _fileService = FileService.Instance;

        protected static string _fileDownloadCallbackUrl;
        protected static string _thumbFileDownloadCallbackUrl;

        public string MsgType { get; protected set; }

        public string Event { get; protected set; }

        static XMLMessageReceiver()
        {
            _fileDownloadCallbackUrl = String.Format("http://{0}/FileServerCallback/MessageFile", AppSettings.IntranetIp);
            _thumbFileDownloadCallbackUrl = String.Format("http://{0}/FileServerCallback/MessageThumbFile", AppSettings.IntranetIp);
        }

        public XMLMessageReceiver()
        {
        }

        public string Handle(string message, ClientDomainContext domainContext)
        {
            if (String.IsNullOrEmpty(message))
                return null;

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(message));

            return Handle(_xmlSerializer.Deserialize(stream) as T, domainContext);
        }

        protected abstract string Handle(T message, ClientDomainContext domainContext);

        protected string GetReplyTextMessage(DomainContext domainContext, string toUserName, string content)
        {
            ResponsiveXMLMessage_TextMessage replyMessage = new ResponsiveXMLMessage_TextMessage();

            replyMessage.Content = content;

            replyMessage.ToUserName = toUserName;
            //这几个字段还是要的，因为当直接以HTTP返回的形式返回XML格式的数据时
            //是要求这几个字段的
            replyMessage.FromUserName = domainContext.UserName;
            replyMessage.CreateTime = WeixinApiHelper.ConvertDateTimeToInt(DateTime.Now);

            return XMLMessageHelper.XmlSerialize(replyMessage);
        }

    }
}
