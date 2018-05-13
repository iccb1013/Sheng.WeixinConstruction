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
    class AutoReplyHelper
    {
        private static readonly LogService _log = LogService.Instance;
        private static readonly MaterialManager _materialManager = MaterialManager.Instance;

        //TODO:这里应该用 ResponsiveXMLMessage
        /// <summary>
        /// 获取XML版的回复消息
        /// </summary>
        /// <param name="domainContext"></param>
        /// <param name="toUserName"></param>
        /// <param name="autoReply"></param>
        /// <returns></returns>
        public static ResponsiveXMLMessageBase GetXMLMessage(ClientDomainContext domainContext, string toUserName, 
            IAutoReply autoReply)
        {
            if (autoReply == null)
                return null;

            ResponsiveXMLMessageBase replyMessage = null;

            switch (autoReply.Type)
            {
                case EnumAutoReplyType.Text:
                    //不能返回空白 Content 的XML给微信 API
                    //否则它会在客户端提示该公众号暂时无法提供服务
                    if (String.IsNullOrEmpty(autoReply.Content) == false)
                    {
                        ResponsiveXMLMessage_TextMessage textMessage = new ResponsiveXMLMessage_TextMessage();
                        textMessage.Content = autoReply.Content;
                        replyMessage = textMessage;
                    }
                    break;
                case EnumAutoReplyType.Image:
                    if (String.IsNullOrEmpty(autoReply.MediaId) == false)
                    {
                        ResponsiveXMLMessage_ImageMessage imageMessage = new ResponsiveXMLMessage_ImageMessage();
                        imageMessage.Image.MediaId = autoReply.MediaId;
                        replyMessage = imageMessage;
                    }
                    break;
                default:
                    Debug.Assert(false, "GetXMLMessage 不支持的 AutoReplyOnKeyWordsRule.Type：" + autoReply.Type.ToString());
                    _log.Write("GetXMLMessage 不支持的 AutoReplyOnKeyWordsRule.Type：" + autoReply.Type.ToString(),
                        domainContext.AutoReplyOnSubscribe.Type.ToString(), TraceEventType.Error);
                    break;
            }

            if (replyMessage != null)
            {
                replyMessage.ToUserName = toUserName;
                //这几个字段还是要的，因为当直接以HTTP返回的形式返回XML格式的数据时
                //是要求这几个字段的
                replyMessage.FromUserName = domainContext.UserName;
                replyMessage.CreateTime = WeixinApiHelper.ConvertDateTimeToInt(DateTime.Now);
            }
            return replyMessage;
        }

        /// <summary>
        /// 获取XML版的回复消息
        /// </summary>
        /// <param name="domainContext"></param>
        /// <param name="toUserName"></param>
        /// <param name="autoReply"></param>
        /// <returns></returns>
        public static KFMessageBase GetKFMessage(ClientDomainContext domainContext, string toUserName,
            IAutoReply autoReply)
        {
            if (autoReply == null)
                return null;

            KFMessageBase replyMessage = null;

            switch (autoReply.Type)
            {
                case EnumAutoReplyType.Text:
                    //不能返回空白 Content 的XML给微信 API
                    //否则它会在客户端提示该公众号暂时无法提供服务
                    if (String.IsNullOrEmpty(autoReply.Content) == false)
                    {
                        KFTextMessage textMessage = new KFTextMessage();
                        textMessage.Text.Content = autoReply.Content;
                        replyMessage = textMessage;
                    }
                    break;
                case EnumAutoReplyType.Image:
                    KFImageMessage imageMessage = new KFImageMessage();
                    imageMessage.Image.MediaId = autoReply.MediaId;
                    replyMessage = imageMessage;
                    break;
                case EnumAutoReplyType.Article:
                    //微信的图文消息有几种不同的格式，此处专用于自动回复
                    //所以只考虑客服接口的内部文章页面格式
                    //图文素材可能会被删除
                    string mediaId = _materialManager.GetArticleMaterialMediaId(autoReply.ArticleId);
                    if (String.IsNullOrEmpty(mediaId) == false)
                    {
                        KFMpArticleMessage articleMessage = new KFMpArticleMessage();
                        articleMessage.Mpnews.MediaId = mediaId;
                        replyMessage = articleMessage;
                    }
                    break;
                default:
                    Debug.Assert(false, "GetKFMessage 不支持的 AutoReplyOnKeyWordsRule.Type：" + autoReply.Type.ToString());
                    _log.Write("GetKFMessage 不支持的 AutoReplyOnKeyWordsRule.Type：" + autoReply.Type.ToString(),
                        domainContext.AutoReplyOnSubscribe.Type.ToString(), TraceEventType.Error);
                    break;
            }

            if (replyMessage != null)
            {
                replyMessage.ToUserName = toUserName;
            }
            return replyMessage;
        }
    }
}
