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
    public class WeixinApiController : Controller
    {
        private static readonly LogService _log = LogService.Instance;
        private static readonly ClientDomainPool _domainPool = ClientDomainPool.Instance;
        private static readonly HttpService _httpService = HttpService.Instance;

        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly MemberManager _memberManager = MemberManager.Instance;

        /// <summary>
        /// 开发者提交信息后，微信服务器将发送GET请求到填写的服务器地址URL上
        /// 开发者通过检验signature对请求进行校验（下面有校验方式）。
        /// 若确认此次GET请求来自微信服务器，请原样返回echostr参数内容，则接入生效，成为开发者成功，否则接入失败。
        /// </summary>
        /// <param name="domainId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Handler(string domainId)
        {
            Guid domainGuid = Guid.Empty;
            if (Guid.TryParse(domainId, out domainGuid) == false)
            {
                return new HttpStatusCodeResult(404);
            }

            // 强制刷新缓存，用于微信对接
            // 用户在后台填写信息后，微信端立即提交的话，相关信息可能还没有同步到缓存
            _domainPool.Refresh(domainGuid);
            DomainEntity domain = _domainPool.Get(domainGuid);
            if (domain == null)
            {
                return new HttpStatusCodeResult(404);
            }

            string resultContent = ConnectAuthorize(domain); //微信接入的测试

            if (String.IsNullOrEmpty(resultContent) == false)
            {
                _domainManager.UpdateDockingTime(domain.Id);
            }

            return new ContentResult() { Content = resultContent };
        }

        /// <summary>
        /// ​当普通微信用户向公众账号发消息时，微信服务器将POST消息的XML数据包到开发者填写的URL上。
        /// ​在微信用户和公众号产生交互的过程中，用户的某些操作会使得微信服务器通过事件推送的形式通知到
        /// 开发者在开发者中心处设置的服务器地址，从而开发者可以获取到该信息。其中，某些事件推送在发生后，
        /// 是允许开发者回复用户的，某些则不允许，详细说明请见本页末尾的微信推送消息与事件说明。
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Handler(string domainId, string signature, string timestamp, string nonce)
        {
            //请求的URL如下格式：
            //http://wxctest.zkebao.com/WeixinApi/Handler/F6AAD430-CA1F-4AFD-B2B0-6E0D2FABB622
            //?signature=84001ea92e2f369642e861d557b9f4c6781db1ca
            //&timestamp=1446393828
            //&nonce=1291578710
            //&encrypt_type=aes
            //&msg_signature=3ed4a96dbc50d491664ec3f425eb7fc1f088ac9b

            Guid domainGuid = Guid.Empty;
            if (Guid.TryParse(domainId, out domainGuid) == false)
            {
                return new HttpStatusCodeResult(404);
            }

            ClientDomainContext domainContext = _domainPool.GetDomainContext(domainGuid);
            if (domainContext == null)
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
            parameter.Encrypt_type = Request.QueryString["encrypt_type"];
            parameter.Msg_signature = Request.QueryString["msg_signature"];

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

                resultContent = domainContext.Handle(parameter, postString);

            }

            return new ContentResult() { Content = resultContent };
        }

        #region 配置服务器URL时的验证

        /// <summary>
        /// 成为开发者的第一步，验证并相应服务器的数据
        /// </summary>
        private string ConnectAuthorize(DomainEntity domain)
        {
            ContentResult result = new ContentResult();

            string token = domain.Token;
            string echoString = HttpContext.Request.QueryString["echoStr"];
            string signature = HttpContext.Request.QueryString["signature"];
            string timestamp = HttpContext.Request.QueryString["timestamp"];
            string nonce = HttpContext.Request.QueryString["nonce"];

            if (CheckSignature(token, signature, timestamp, nonce))
            {
                //对接成功
                return echoString;
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 验证微信签名
        /// </summary>
        public bool CheckSignature(string token, string signature, string timestamp, string nonce)
        {
            string[] ArrTmp = { token, timestamp, nonce };

            Array.Sort(ArrTmp);
            string tmpStr = string.Join("", ArrTmp);

            byte[] cleanBytes = Encoding.UTF8.GetBytes(tmpStr);
            byte[] hashedBytes = System.Security.Cryptography.SHA1.Create().ComputeHash(cleanBytes);
            tmpStr = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            if (tmpStr == signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

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

            //完成网页鉴权后要转回的页面地址
            string redirectUrl = null;
            if (String.IsNullOrEmpty(state) == false)
                redirectUrl = Server.UrlDecode(state);
            else
            {
                _log.Write("没有指定完成网页鉴权后要转回的页面地址： state", TraceEventType.Warning);
                return new HttpStatusCodeResult(404);
            }

            string domainId = HttpContext.Request.QueryString["domainId"];

            if (String.IsNullOrEmpty(domainId))
            {
                _log.Write("没有指定 domainId ", TraceEventType.Warning);
                return new HttpStatusCodeResult(404);
            }

            DomainContext domainContext = _domainPool.GetDomainContext(Guid.Parse(domainId));

            if (domainContext == null)
            {
                _log.Write("指定的 domainId 不存在", TraceEventType.Warning);
                return new HttpStatusCodeResult(404);
            }

            if (String.IsNullOrEmpty(code))
            {
                //重定向到错误页面
                return new RedirectResult(String.Format(
                            "~/Home/ErrorView/{0}?message={1}", domainContext.Domain.Id, "td1"));
            }

            //domainContext.AppSecret
            RequestApiResult<WeixinWebAccessTokenResult> getWebAccessToken =
                TokenApi.GetWebAccessToken(domainContext.AppId, "domainContext.AppSecret", code);

            if (getWebAccessToken.Success == false)
            {
                _log.Write("请求网页AccessToken失败。", getWebAccessToken.Message, TraceEventType.Warning);
                //重定向到错误页面
                return new RedirectResult(String.Format(
                             "~/Home/ErrorView/{0}?message={1}", domainContext.Domain.Id, "td2"));
            }

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
                //重定向到提示关注页面
                return new RedirectResult("~/Home/GuideSubscribe/" + domainContext.Domain.Id);
            }

            //根据OpenId获取用户信息
            MemberEntity member = _memberManager.GetMemberByOpenId(domainContext.Domain.Id, getUserInfoResult.ApiResult.OpenId);
            if (member == null)
            {
                //添加新用户
                member = _memberManager.AddMember(domainContext, getUserInfoResult.ApiResult);
            }
            else
            {
                //更新当前用户信息
                _memberManager.UpdateMember(member, getUserInfoResult.ApiResult);
            }

            MemberContext memberContext = new MemberContext(member);
            SessionContainer.SetMemberContext(HttpContext, memberContext);

            //转回初始业务页面
            return new RedirectResult(redirectUrl);
        }

        #region 测试用

        public ActionResult HandlerTest()
        {
            return View();
        }

        #endregion
    }
}