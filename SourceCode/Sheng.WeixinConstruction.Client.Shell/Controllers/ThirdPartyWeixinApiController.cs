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
using Sheng.WeixinConstruction.Client.Core;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Controllers
{
    /// <summary>
    /// 作为第三方平台运营时
    /// 接收消息推送
    /// </summary>
    public class ThirdPartyWeixinApiController : Controller
    {
        private static readonly LogService _log = LogService.Instance;
        private static readonly ClientDomainPool _domainPool = ClientDomainPool.Instance;
        private static readonly HttpService _httpService = HttpService.Instance;
        private static readonly WxConfigurationSection _configuration = ConfigService.Instance.Configuration;
        private static readonly CachingService _cachingService = CachingService.Instance;

        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly MemberManager _memberManager = MemberManager.Instance;
        private static readonly UserManager _userManager = UserManager.Instance;

        private static WXBizMsgCrypt _msgCrypt;

        static ThirdPartyWeixinApiController()
        {
            _msgCrypt = new WXBizMsgCrypt(_configuration.ThirdParty.Token,
                _configuration.ThirdParty.EncodingAESKey, _configuration.ThirdParty.AppId);
        }

        /// <summary>
        /// 公众号消息与事件接收URL
        /// ​当普通微信用户向公众账号发消息时，微信服务器将POST消息的XML数据包到开发者填写的URL上。
        /// ​在微信用户和公众号产生交互的过程中，用户的某些操作会使得微信服务器通过事件推送的形式通知到
        /// 开发者在开发者中心处设置的服务器地址，从而开发者可以获取到该信息。其中，某些事件推送在发生后，
        /// 是允许开发者回复用户的，某些则不允许，详细说明请见本页末尾的微信推送消息与事件说明。
        /// 作为第三方平台运营时，AppId 会作为URL的一部分带过来
        /// http://wxc.shengxunwei.com/ThirdPartyWeixinApi/Handler/$APPID$
        /// 其中$APPID$在实际推送时会替换成所属的已授权公众号的appid。
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Handler(string domainId, string signature, string timestamp, string nonce, string encrypt_type, string msg_signature)
        {
            string appId = domainId;

            //请求的URL如下格式：
            //http://wxctest.shengxunwei.com/WeixinApi/Handler/F6AAD430-CA1F-4AFD-B2B0-6E0D2FABB622
            //?signature=84001ea92e2f369642e861d557b9f4c6781db1ca
            //&timestamp=1446393828
            //&nonce=1291578710
            //&encrypt_type=aes
            //&msg_signature=3ed4a96dbc50d491664ec3f425eb7fc1f088ac9b

            if (String.IsNullOrEmpty(appId))
            {
                return new HttpStatusCodeResult(404);
            }

            XMLMessageUrlParameter parameter = new XMLMessageUrlParameter();
            parameter.Signature = signature;
            parameter.Timestamp = timestamp;
            parameter.Nonce = nonce;

            //消息加解密
            //http://mp.weixin.qq.com/wiki/0/61c3a8b9d50ac74f18bdf2e54ddfc4e0.html
            //url上无encrypt_type参数或者其值为raw时表示为不加密；encrypt_type为aes时，表示aes加密（暂时只有raw和aes两种值)。
            //msg_signature:表示对消息体的签名。
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

                //resultContent = domainContext.Handle(parameter, postString);
            }

            _log.Write("收到消息推送（密文）", postString, TraceEventType.Verbose);

            string message = String.Empty;

            int decryptResult =
                _msgCrypt.DecryptMsg(parameter.Msg_signature, parameter.Timestamp, parameter.Nonce,
                postString, ref message);

            _log.Write("收到消息推送（明文）", message, TraceEventType.Verbose);

            string resultMessage = null;
            //全网发布时的自动化测试的专用测试公众号
            if (appId == TestApp.AppId)
            {
                _log.Write("收到接入检测消息", message, TraceEventType.Verbose);

                resultMessage = TestApp.Handle(message);

                _log.Write("返回接入检测消息", resultMessage, TraceEventType.Verbose);
            }
            else
            {
                Guid? guidDomainId = _domainPool.GetDomainId(appId);
                if (guidDomainId == null)
                {
                    return new HttpStatusCodeResult(404);
                }

                ClientDomainContext domainContext = _domainPool.GetDomainContext(guidDomainId.Value);
                if (domainContext == null)
                {
                    return new HttpStatusCodeResult(404);
                }

                resultMessage = domainContext.Handle(message);
            }

            _log.Write("返回消息（明文）", resultMessage, TraceEventType.Verbose);

            if (String.IsNullOrEmpty(resultMessage) == false)
            {
                _msgCrypt.EncryptMsg(resultMessage,
                    parameter.Timestamp, parameter.Nonce, ref resultContent);
            }

            _log.Write("返回消息（密文）", resultContent, TraceEventType.Verbose);


            return new ContentResult() { Content = resultContent };
        }

        /// <summary>
        ///微信网页授权回调
        ///如果用户同意授权，页面将跳转至 redirect_uri/?code=CODE&state=STATE。
        ///若用户禁止授权，则重定向后不会带上code参数，仅会带上state参数redirect_uri?state=STATE
        ///http://mp.weixin.qq.com/wiki/9/01f711493b5a02f24b04365ac5d8fd95.html
        /// </summary>
        /// <returns></returns>
        public ActionResult OAuthCallback()
        {
            _log.Write("微信网页授权接口发起回调", HttpContext.Request.Url.ToString(), TraceEventType.Verbose);

            string code = HttpContext.Request.QueryString["code"];
            string state = HttpContext.Request.QueryString["state"];

            //第为第三方平台运营时，会返回 appId
            string appId = HttpContext.Request.QueryString["appid"];

            //完成网页鉴权后要转回的页面地址
            string redirectUrl = null;
            if (String.IsNullOrEmpty(state) == false)
            {
                redirectUrl = _cachingService.Get(state);
            }
            else
            {
                _log.Write("微信网页授权接口发起回调",
                   "没有指定完成网页鉴权后要转回的页面地址： state" + HttpContext.Request.Url.ToString(),
                   TraceEventType.Warning);
                return new HttpStatusCodeResult(404);
            }

            string domainId = HttpContext.Request.QueryString["domainId"];

            if (String.IsNullOrEmpty(domainId))
            {
                _log.Write("微信网页授权接口发起回调", "没有指定 domainId ", TraceEventType.Warning);
                return new HttpStatusCodeResult(404);
            }

            DomainContext domainContext = _domainPool.GetDomainContext(Guid.Parse(domainId));

            if (domainContext == null)
            {
                _log.Write("微信网页授权接口发起回调", "指定的 domainId 不存在", TraceEventType.Warning);
                return new HttpStatusCodeResult(404);
            }

            if (domainContext.Authorizer == null)
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                            "~/Home/ErrorView/{0}?message={1}", domainContext.Domain.Id, "td5"));
            }

            //只有微信认证服务号具备此权限
            //用户管理-网页授权获取用户openid/用户基本信息
            if (domainContext.Authorizer.AuthorizationType != EnumAuthorizationType.AuthorizedService)
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                            "~/Home/ErrorView/{0}?message={1}", domainContext.Domain.Id, "td4"));
            }

            if (String.IsNullOrEmpty(code))
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                            "~/Home/ErrorView/{0}?message={1}", domainContext.Domain.Id, "td1"));
            }

            /*
             * 此处偶发 40029 错误，不合法的oauth_code
             * 40029-invalid code, hints: [ req_id: Ft6quA0644ns67 ]
             * https://segmentfault.com/q/1010000002739502?foxhandler=RssReadRenderProcessHandler
             * http://mp.weixin.qq.com/qa/11/3c20059cc944d6edf4a1124c2fd09253.html
             * redirect_uri后面加个随机数没用
             * 通过尝试发起两次请求的方式解决
             */
            //通过code换取网页access_token
            RequestApiResult<WeixinWebAccessTokenResult> getWebAccessToken =
                TokenApi.GetThirdPartyWebAccessToken(domainContext.AppId, code, _configuration.ThirdParty.AppId,
                ThirdPartyAccessTokenGetter.Get());

            if (getWebAccessToken.Success == false)
            {
                //再来一次，防止死循环，只重试一次，在URL后面加个参数以标记
                if (redirectUrl.Contains("RetryGetWebAccessToken"))
                {
                    _log.Write("请求网页AccessToken失败。", getWebAccessToken.Message, TraceEventType.Warning);
                    //重定向到错误页面
                    return new RedirectResult(String.Format(
                                 "~/Home/ErrorView/{0}?message={1}", domainContext.Domain.Id, "td2"));
                }
                else
                {
                    if (redirectUrl.IndexOf('?') >= 0)
                    {
                        redirectUrl += "&RetryGetWebAccessToken=1";
                    }
                    else
                    {
                        redirectUrl += "?RetryGetWebAccessToken=1";
                    }

                    return new RedirectResult(redirectUrl);
                }
            }

            //将OpenId保存到Session
            SessionContainer.SetOpenId(HttpContext, getWebAccessToken.ApiResult.OpenId);

            //先判断本地数据库中是否已经有了此用户信息
            MemberContext memberContext = null;
            MemberEntity member = _memberManager.GetMemberByOpenId(domainContext.Domain.Id, domainContext.AppId, getWebAccessToken.ApiResult.OpenId);
            if (member != null)
            {
                if (member.Attention)
                {
                    //为提高鉴权性能，此处不更新用户信息，只作标记，用windows服务后台更新
                    _memberManager.NeedUpdate(member.Id, true);

                    memberContext = new MemberContext(member);
                    SessionContainer.SetMemberContext(HttpContext, memberContext);

                    //转回初始业务页面
                    return new RedirectResult(redirectUrl);
                }
                else
                {
                    //如果用户已经取消关注，此处就不需要再调用微信的 GetUserInfo 接口了
                    //直接判断能不能匿名浏览即可
                    return RedirectUrlOnlyOpenId(redirectUrl, domainId);
                }
            }

            //在本地没有用户信息的情况下，调用weixinApi去取
            //此处拿到OpenId了，接下来判断该用户是否是已关注用户
            RequestApiResult<WeixinUser> getUserInfoResult =
                UserApiWrapper.GetUserInfo(domainContext, getWebAccessToken.ApiResult.OpenId);
            if (getUserInfoResult.Success == false)
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                    "~/Home/ErrorView/{0}?message={1}", domainContext.Domain.Id, "td3"));
            }

            //值为0时，代表此用户没有关注该公众号，拉取不到其余信息。
            //跳转到引导关注页面
            if (getUserInfoResult.ApiResult.Subscribe == 0)
            {
                //用户取消关注有消息推送
                //在那时设置member中是否在关注为false

                return RedirectUrlOnlyOpenId(redirectUrl, domainId);
            }
            else
            {
                //根据OpenId获取用户信息
                //添加新用户
                AddMemberArgs args = new AddMemberArgs();
                args.WeixinUser = getUserInfoResult.ApiResult;
                member = _memberManager.AddMember(domainContext, args);

                memberContext = new MemberContext(member);
                SessionContainer.SetMemberContext(HttpContext, memberContext);

                //转回初始业务页面
                return new RedirectResult(redirectUrl);
            }
        }

        /// <summary>
        /// 在只有 OpenId 的情况下
        /// 判断是否允许转到目标 URL ，如果不能则转到引导关注画面
        /// </summary>
        /// <param name="redirectUrl"></param>
        /// <param name="domainId"></param>
        /// <returns></returns>
        private RedirectResult RedirectUrlOnlyOpenId(string redirectUrl, string domainId)
        {
            //先判断页面是否允许未关注用户浏览
            //URL后面有没有 ?AllowedOnlyOpenId=1
            if (redirectUrl.Contains("AllowedOnlyOpenId"))
            {
                return new RedirectResult(redirectUrl);
            }
            else
            {
                #region 重定向到提示关注页面

                //重定向到提示关注页面
                //return new RedirectResult(domainContext.GuideSubscribeUrl);

                //此处需要先判断是不是活动页面，如果是则跳转到活动引导关注页
                if (redirectUrl.Contains("campaignId"))
                {
                    string strCampaignId = null;
                    Uri uri = new Uri(redirectUrl);
                    string[] queryArray = uri.Query.TrimStart('?').Split('&');
                    foreach (string query in queryArray)
                    {
                        if (query.Contains("campaignId"))
                        {
                            string[] campaignIdQuery = query.Split('=');
                            strCampaignId = campaignIdQuery[1];
                        }
                    }

                    Guid campaignId;
                    if (String.IsNullOrEmpty(strCampaignId) || Guid.TryParse(strCampaignId, out campaignId) == false)
                    {
                        return new RedirectResult(String.Format("~/Home/GuideSubscribe/{0}", domainId));
                    }
                    else
                    {
                        return new RedirectResult(String.Format(
                            "~/Home/CampaignGuideSubscribe/{0}?campaignId={1}", domainId, strCampaignId));
                    }
                }
                else
                {
                    return new RedirectResult(String.Format("~/Home/GuideSubscribe/{0}", domainId));
                }

                #endregion
            }
        }
    }
}