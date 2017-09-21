using Linkup.Common;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Container.Models;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using Sheng.WeixinConstruction.WeixinContract.ThirdParty;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Container.Controllers
{
    /// <summary>
    /// 作为第三方平台运行时
    /// 平台授权事件接收
    /// </summary>
    public class ThirdPartyAuthController : BasalController
    {
        private static readonly LogService _log = LogService.Instance;
        private static readonly WxConfigurationSection _configuration = ConfigService.Instance.Configuration;
        private static readonly ThirdPartyAuthHandler _handler = ThirdPartyAuthHandler.Instance;
        private static readonly ThirdPartyAccessToken _thirdPartyAccessToken = ThirdPartyAccessToken.Instance;
        private static readonly ThirdPartyManager _thirdPartyManager = ThirdPartyManager.Instance;

        private static readonly AuthorizerAccessTokenPool _accessTokenPool = AuthorizerAccessTokenPool.Instance;
        private static readonly AuthorizerJsApiTicketPool _jsApiTicketPool = AuthorizerJsApiTicketPool.Instance;

        private static WXBizMsgCrypt _msgCrypt;

        static ThirdPartyAuthController()
        {
            _msgCrypt = new WXBizMsgCrypt(_configuration.ThirdParty.Token,
                _configuration.ThirdParty.EncodingAESKey, _configuration.ThirdParty.AppId);
        }

        /// <summary>
        /// 推送component_verify_ticket协议
        /// 推送取消授权通知
        /// https://open.weixin.qq.com/cgi-bin/showdocument?action=dir_list&t=resource/res_list&verify=1&id=open1419318587&lang=zh_CN
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="encrypt_type"></param>
        /// <param name="msg_signature"></param>
        /// <returns></returns>
        [HttpPost]
        [PublishAction]
        public ActionResult Handler(string signature, string timestamp, string nonce, string encrypt_type, string msg_signature)
        {
            _log.Write("Handler", HttpContext.Request.Url.ToString(), TraceEventType.Verbose);

            XMLMessageUrlParameter parameter = new XMLMessageUrlParameter();
            parameter.Signature = signature;
            parameter.Timestamp = timestamp;
            parameter.Nonce = nonce;

            //消息加解密
            //http://mp.weixin.qq.com/wiki/0/61c3a8b9d50ac74f18bdf2e54ddfc4e0.html
            //url上无encrypt_type参数或者其值为raw时表示为不加密；encrypt_type为aes时，表示aes加密（暂时只有raw和aes两种值)。
            //msg_signature:表示对消息体的签名。
            //作为第三方平台，要求必须加密
            parameter.Encrypt_type = encrypt_type;
            parameter.Msg_signature = msg_signature;

            string resultContent = null;

            string postString;
            using (Stream stream = HttpContext.Request.InputStream)
            {
                Byte[] postBytes = new Byte[stream.Length];
                stream.Read(postBytes, 0, (int)stream.Length);
                postString = Encoding.UTF8.GetString(postBytes);

                //测试用，接收微信过来的消息不能UrlDecode，会改变内容导致解密失败
                //postString = Server.UrlDecode(postString);
                //_log.Write("Handler:\r\n" + postString);
            }

            resultContent = ProcessMessage(parameter, postString);

            return new ContentResult() { Content = resultContent };
        }

        /// <summary>
        /// 解决并处理消息，返回处理结果
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="postString"></param>
        /// <returns></returns>
        private string ProcessMessage(XMLMessageUrlParameter parameter, string postString)
        {
            //解密消息
            //注意不能返回空白 Content 的XML给微信 API
            //否则它会在客户端提示该公众号暂时无法提供服务

            _log.Write("收到消息推送（密文）", postString, TraceEventType.Verbose);

            string message = String.Empty;

            int decryptResult =
                _msgCrypt.DecryptMsg(parameter.Msg_signature, parameter.Timestamp, parameter.Nonce,
                postString, ref message);

            _log.Write("收到消息推送（明文）", message, TraceEventType.Verbose);

            _handler.Handle(message);

            //接收到后之后只需直接返回字符串success
            return "success";

            //string encryptMessage = null;

            //if (String.IsNullOrEmpty(returnMessage) == false)
            //{
            //    _msgCrypt.EncryptMsg(returnMessage,
            //        parameter.Timestamp, parameter.Nonce, ref encryptMessage);
            //}

            //_log.Write("返回消息（密文）", encryptMessage, TraceEventType.Verbose);

            //return encryptMessage;
        }

        /// <summary>
        /// 作为第三方平台运营时，获取自己的 AccessToken
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAccessToken()
        {
            return new ContentResult()
            {
                ContentEncoding = Encoding.ASCII,
                Content = _thirdPartyAccessToken.Get()
            };
        }

        /// <summary>
        /// 作为第三方平台运营时，强制刷新自己的 AccessToken
        /// </summary>
        /// <returns></returns>
        public ActionResult RefreshAccessToken(string accessToken)
        {
            return new ContentResult()
            {
                ContentEncoding = Encoding.ASCII,
                Content = _thirdPartyAccessToken.Refresh(accessToken)
            };
        }

        /// <summary>
        /// 根据授权码开始维护一个新的公众号Token
        /// 如果指定的公众号之前授权给其它domain了，将解除之前的授权关联
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateAuthorizer(string domainId, string authCode)
        {
            RequestApiResult<WeixinThirdPartyGetAuthorizationInfoResult> createResult =
                _thirdPartyManager.CreateAuthorizer(Guid.Parse(domainId), authCode);

            ApiResult<CreateAuthorizerResult> result = new ApiResult<CreateAuthorizerResult>();
            result.Success = createResult.Success;
            result.Message = createResult.Message;

            if (createResult.Success)
            {
                WeixinThirdPartyAuthorizationInfo info = createResult.ApiResult.AuthorizationInfo;

                //开始维持accessToken
                _accessTokenPool.Add(info.AppId, info.AccessToken, info.ExpiresIn, info.RefreshToken);

                result.Data = new CreateAuthorizerResult();
                result.Data.AppId = info.AppId;
            }

            return RespondResult(result);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAuthorizerAccessToken(string appId)
        {
            return new ContentResult()
            {
                ContentEncoding = Encoding.ASCII,
                Content = _accessTokenPool.Get(appId)
            };
        }

        /// <summary>
        /// 强制刷新
        /// </summary>
        /// <returns></returns>
        public ActionResult RefreshAuthorizerAccessToken(string appId, string accessToken)
        {
            return new ContentResult()
            {
                ContentEncoding = Encoding.ASCII,
                Content = _accessTokenPool.Refresh(appId, accessToken)
            };
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAuthorizerJsApiTicket(string appId)
        {
            return new ContentResult()
            {
                ContentEncoding = Encoding.ASCII,
                Content = _jsApiTicketPool.Get(appId)
            };
        }

        public ActionResult RefreshAuthorizerJsApiTicket(string appId, string jsApiTicket)
        {
            return new ContentResult()
            {
                ContentEncoding = Encoding.ASCII,
                Content = _jsApiTicketPool.Refresh(appId, jsApiTicket)
            };
        }

        public ActionResult Monitor()
        {
            ThirdPartyAuthMonitorViewModel model = new ThirdPartyAuthMonitorViewModel();
            model.AccessToken = _thirdPartyAccessToken.Get();
            model.AccessTokenExpiryTime = _thirdPartyAccessToken.GetExpiryTime();
            model.AuthorizerAccessTokenList = _accessTokenPool.GetAuthorizerAccessTokenList();
            model.AuthorizerJsApiTicketList = _jsApiTicketPool.GetAuthorizerJsApiTicketList();
            return View(model);
        }
    }
}