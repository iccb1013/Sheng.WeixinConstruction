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
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Client.Core
{
    /*
     * 由于群发任务提交后，群发任务可能在一定时间后才完成，因此，
     * 群发接口调用时，仅会给出群发任务是否提交成功的提示，若群发任务提交成功，则在群发任务结束时，
     * 会向开发者在公众平台填写的开发者URL（callback URL）推送事件。
     * 
     * http://mp.weixin.qq.com/wiki/15/40b6865b893947b764e2de8e4a1fb55f.html
     */
    class XMLMessageReceiver_GroupMessageFinishEvent : XMLMessageReceiver<ReceivingXMLMessage_GroupMessageFinishEvent>
    {
        protected static readonly GroupMessageManager _groupMessageManager = GroupMessageManager.Instance;

        public XMLMessageReceiver_GroupMessageFinishEvent()
        {
            MsgType = "event";
            Event = "MASSSENDJOBFINISH";
        }

        protected override string Handle(ReceivingXMLMessage_GroupMessageFinishEvent message, ClientDomainContext domainContext)
        {
            _groupMessageManager.Finish(domainContext.Domain.Id, domainContext.AppId, message);
            return null;
        }

    }
}
