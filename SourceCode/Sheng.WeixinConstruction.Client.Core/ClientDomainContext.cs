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
using Newtonsoft.Json.Linq;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Core
{
    public class ClientDomainContext : DomainContext
    {
        private static readonly XMLMessageReceiverFactory _messageReceiver = XMLMessageReceiverFactory.Instance;

        private static readonly SettingsManager _settingsManager = SettingsManager.Instance;

        private static readonly LogService _log = LogService.Instance;

        /// <summary>
        /// 关注自动回复
        /// </summary>
        public AutoReplyOnSubscribeEntity AutoReplyOnSubscribe
        {
            get;
            set;
        }

        /// <summary>
        /// 消息自动回复
        /// </summary>
        public AutoReplyOnMessageEntity AutoReplyOnMessage
        {
            get;
            set;
        }

        /// <summary>
        /// 关键字自动回复
        /// </summary>
        public AutoReplyOnKeyWords AutoReplyOnKeyWords
        {
            get;
            set;
        }

        public ClientDomainContext(DomainEntity domain)
            : base(domain)
        {
            //_msgCrypt = new WXBizMsgCrypt(domain.Token, domain.EncodingAESKey, domain.AppId);

            Refresh();
        }

        //public string Handle(XMLMessageUrlParameter parameter, string postString)
        //{
        //    //注意不能返回空白 Content 的XML给微信 API
        //    //否则它会在客户端提示该公众号暂时无法提供服务

        //    _log.Write("收到消息推送（密文）", postString, TraceEventType.Verbose);

        //    string message = String.Empty;

        //    int decryptResult =
        //        _msgCrypt.DecryptMsg(parameter.Msg_signature, parameter.Timestamp, parameter.Nonce,
        //        postString, ref message);

        //    _log.Write("收到消息推送（明文）", message, TraceEventType.Verbose);

        //    string returnMessage = _messageReceiver.Handle(message, this);

        //    _log.Write("返回消息（明文）", returnMessage, TraceEventType.Verbose);

        //    string encryptMessage = null;

        //    if (String.IsNullOrEmpty(returnMessage) == false)
        //    {
        //        _msgCrypt.EncryptMsg(returnMessage,
        //            parameter.Timestamp, parameter.Nonce, ref encryptMessage);
        //    }

        //    _log.Write("返回消息（密文）", encryptMessage, TraceEventType.Verbose);

        //    return encryptMessage;
        //}

        /// <summary>
        /// 作为第三方平台运营时，消息的解密是统一进行的，这里直接处理明文
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string Handle(string message)
        {
            return _messageReceiver.Handle(message, this);
        }

        /// <summary>
        /// 刷新基本信息以外的相关设置
        /// </summary>
        protected override void OnRefresh()
        {
            //刷新自动回复相关内容
            this.AutoReplyOnSubscribe =
                _settingsManager.GetAutoReplyOnSubscribe(this.Domain.Id);

            this.AutoReplyOnMessage =
                _settingsManager.GetAutoReplyOnMessage(this.Domain.Id);

            AutoReplyOnKeyWords autoReplyOnKeyWords = new AutoReplyOnKeyWords();
            autoReplyOnKeyWords.RuleList = _settingsManager.GetAutoReplyOnKeyWordsRuleList(this.Domain.Id);
            this.AutoReplyOnKeyWords = autoReplyOnKeyWords;

        }

        public WeixinJsApiConfig GetJsApiConfig(string url)
        {
            WeixinJsApiConfig config = WeixinJsApi.GetConfig(AccessTokenGetter.GetJsApiTicket(this.AppId), url);
            config.AppId = this.AppId;
            return config;
        }
    }
}
