using Newtonsoft.Json;
using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.FileService.Controllers
{
    /// <summary>
    /// 粉丝海报
    /// </summary>
    public class MemberQRCodeController : BasalController
    {
        private static readonly MemberQRCodeManager _memberQRCodeManager = MemberQRCodeManager.Instance;

        public ActionResult GetMemberQRCodeImage()
        {
            GetCampaign_MemberQRCodeImageArgs args = RequestArgs<GetCampaign_MemberQRCodeImageArgs>();
            GetMemberQRCodeImageResult result = new GetMemberQRCodeImageResult();
            if (args == null)
            {
                result.Message = "参数无效。";
                return new ContentResult()
                {
                    Content = JsonConvert.SerializeObject(result)
                };
            }

            string serverRootDir = Server.MapPath("/");
            result = _memberQRCodeManager.GetMemberQRCodeImage(serverRootDir, args);
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(result)
            };

        }
    }
}