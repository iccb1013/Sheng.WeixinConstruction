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
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Client.Core
{
    /// <summary>
    /// 关键字自动回复
    /// </summary>
    public class AutoReplyOnKeyWords
    {
        private static readonly LogService _log = LogService.Instance;

        public List<AutoReplyOnKeyWordsRuleEntity> RuleList
        {
            get;
            set;
        }

        public List<KFMessageBase> GetReplyMessage(ClientDomainContext domainContext, ReceivingXMLMessage_TextMessage userMessage)
        {
            if (userMessage == null || String.IsNullOrEmpty(userMessage.Content))
                return null;

            if (RuleList == null || RuleList.Count == 0)
                return null;

            List<KFMessageBase> messageList = new List<KFMessageBase>();

            foreach (AutoReplyOnKeyWordsRuleEntity rule in RuleList)
            {
                if (rule.IsMatch(userMessage.Content))
                {
                    //判断是随机回复一条还是回复全部
                    if (rule.ReplyAll)
                    {
                        foreach (var content in rule.ContentList)
                        {
                            if (content == null)
                                continue;

                            KFMessageBase message = 
                                AutoReplyHelper.GetKFMessage(domainContext, userMessage.FromUserName, content);

                            if (message != null)
                                messageList.Add(message);
                        }
                    }
                    else
                    {
                        Random random = new Random(DateTime.Now.Millisecond);
                        AutoReplyOnKeyWordsContentEntity content =
                            rule.ContentList[random.Next(0, rule.ContentList.Count)];

                        KFMessageBase message =
                               AutoReplyHelper.GetKFMessage(domainContext, userMessage.FromUserName, content);

                        if (message != null)
                            messageList.Add(message);
                    }
                    break;
                }
            }

            return messageList;
        }
    }
}
