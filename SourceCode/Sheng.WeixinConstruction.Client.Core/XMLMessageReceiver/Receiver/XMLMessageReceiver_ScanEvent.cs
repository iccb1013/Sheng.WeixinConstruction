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
    class XMLMessageReceiver_ScanEvent : XMLMessageReceiver<ReceivingXMLMessage_ScanEventMessage>
    {
        private static readonly ScenicQRCodeManager _scenicQRCodeManager = ScenicQRCodeManager.Instance;

        public XMLMessageReceiver_ScanEvent()
        {
            MsgType = "event";
            Event = "SCAN";
        }

        protected override string Handle(ReceivingXMLMessage_ScanEventMessage message, ClientDomainContext domainContext)
        {
            //扫描带参数二维码事件
            if (String.IsNullOrEmpty(message.EventKey))
            {
                return String.Empty;
            }

            string strScenicQRCodeId = message.EventKey;

            //获取场景二维码Id
            Guid scenicQRCodeId;
            if (Guid.TryParse(strScenicQRCodeId, out scenicQRCodeId))
            {
                ScenicQRCodeLandingLogEntity log = new ScenicQRCodeLandingLogEntity();
                log.Domain = domainContext.Domain.Id;
                log.QRCodeId = scenicQRCodeId;
                log.VisitorOpenId = message.FromUserName;
                log.LandingTime = DateTime.Now;
                _scenicQRCodeManager.IncrementLanding(scenicQRCodeId, log);
            }

            return String.Empty;
        }
    }
}
