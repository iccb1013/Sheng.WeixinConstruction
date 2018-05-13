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
using Newtonsoft.Json;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using Sheng.WeixinConstruction.WeixinContract.ThirdParty;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;


namespace Sheng.WeixinConstruction.Client.Shell
{
    /*
     * 全网发布接入检测
     * https://open.weixin.qq.com/cgi-bin/showdocument?action=dir_list&t=resource/res_list&verify=1&id=open1419318611&token=&lang=zh_CN
     */
    /// <summary>
    /// 全网发布接入检测测试公众号
    /// </summary>
    public static class TestApp
    {
        private static LogService _log = LogService.Instance;

        public const string AppId = "wx570bc396a51b8ff8";
        public const string UserName = "gh_3c884a361561";

        private static string _query_auth_code;
        private static string _fromUserName;
        private static string _accessToken;

        private static Timer _timer;

        public static string Handle(string message)
        {
            XElement xml = XElement.Parse(message);

            string messageType = xml.XPathSelectElement("MsgType").Value;
            if (String.IsNullOrEmpty(messageType))
                return String.Empty;

            _fromUserName = xml.XPathSelectElement("FromUserName").Value;

            //事件消息
            if (messageType == "event")
            {
                string eventType = xml.XPathSelectElement("Event").Value;
                if (String.IsNullOrEmpty(eventType))
                    return String.Empty;

                ResponsiveXMLMessage_TextMessage textMessage = new ResponsiveXMLMessage_TextMessage();
                textMessage.Content = eventType + "from_callback";
                textMessage.ToUserName = _fromUserName;
                textMessage.FromUserName = UserName;
                textMessage.CreateTime = WeixinApiHelper.ConvertDateTimeToInt(DateTime.Now);

                return XMLMessageHelper.XmlSerialize(textMessage);
            }
            //普通消息
            else
            {
                string content = xml.XPathSelectElement("Content").Value;
                if (content == "TESTCOMPONENT_MSG_TYPE_TEXT")
                {
                    ResponsiveXMLMessage_TextMessage textMessage = new ResponsiveXMLMessage_TextMessage();
                    textMessage.Content = "TESTCOMPONENT_MSG_TYPE_TEXT_callback";
                    textMessage.ToUserName = _fromUserName;
                    textMessage.FromUserName = UserName;
                    textMessage.CreateTime = WeixinApiHelper.ConvertDateTimeToInt(DateTime.Now);

                    return XMLMessageHelper.XmlSerialize(textMessage);
                }
                else
                {
                    _query_auth_code = content.Remove(0, "QUERY_AUTH_CODE:".Length);

                    RequestApiResult<WeixinThirdPartyGetAuthorizationInfoResult> getAuthorizationInfo =
                        ThirdPartyApiWrapper.GetAuthorizationInfo(_query_auth_code);

                    _accessToken = getAuthorizationInfo.ApiResult.AuthorizationInfo.AccessToken;

                    _timer = new System.Threading.Timer(SendKFMessage, null, 2 * 1000, 1 * 60 * 1000);

                    return null;
                }
            }
        }


        private static void SendKFMessage(object state)
        {
            _timer.Change(-1, -1);

            KFTextMessage textMessage = new KFTextMessage();
            textMessage.Text.Content = _query_auth_code + "_from_api";
            textMessage.ToUserName = _fromUserName;
            KFApi.Send(_accessToken, textMessage);

            _log.Write("调用客服接口回复测试消息", JsonConvert.SerializeObject(textMessage), TraceEventType.Verbose);

        }
    }
}