using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Areas.Api.Controllers
{
    public class SystemController : BasalController
    {
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        public ActionResult GetOperatedLogList()
        {
            GetItemListArgs args = RequestArgs<GetItemListArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.DomainId = UserContext.User.Domain;
            args.AppId = DomainContext.AppId;

            GetItemListResult result = _operatedLogManager.GetOperatedLogList(args);
            return RespondDataResult(result);
        }


    }
}