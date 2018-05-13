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
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Sheng.WeixinConstruction.Client.Core
{
    public class XMLMessageReceiverFactory
    {
        private static XMLMessageReceiverFactory _instance;
        public static XMLMessageReceiverFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new XMLMessageReceiverFactory();

                return _instance;
            }
        }

        private List<IXMLMessageReceiver> _receiverList = new List<IXMLMessageReceiver>();

        private LogService _log = LogService.Instance;

        private XMLMessageReceiverFactory()
        {
            List<Type> receiverTypeList = ReflectionHelper.GetTypeListBaseOn<IXMLMessageReceiver>();
            foreach (var receiverType in receiverTypeList)
            {
                _receiverList.Add((IXMLMessageReceiver)Activator.CreateInstance(receiverType));
            }
        }

        public string Handle(string message, ClientDomainContext domainContext)
        {
            //对于普通消息 MsgType 就要以区分消息的类型了
            //但是对于事件消息，需要通过另外一个字段来区分事件的类型

            XElement xml = XElement.Parse(message);

            string messageType = xml.XPathSelectElement("MsgType").Value;
            if (String.IsNullOrEmpty(messageType))
                return String.Empty;

            //事件消息
            if (messageType == "event")
            {
                string eventType = xml.XPathSelectElement("Event").Value;
                if (String.IsNullOrEmpty(eventType))
                    return String.Empty;

                foreach (var receiver in _receiverList)
                {
                    if (receiver.Event == eventType)
                    {
                        return receiver.Handle(message, domainContext);
                    }
                }
            }
            //普通消息
            else
            {
                foreach (var receiver in _receiverList)
                {
                    if (receiver.MsgType == messageType)
                    {
                        return receiver.Handle(message, domainContext);
                    }
                }
            }

            return String.Empty;
        }
    }
}