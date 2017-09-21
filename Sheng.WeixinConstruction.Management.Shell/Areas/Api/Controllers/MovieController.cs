using Linkup.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Management.Core;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using Sheng.WeixinConstruction.WeixinContract.ThirdParty;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Management.Shell.Areas.Api.Controllers
{
    public class MovieController : BasalController
    {
        private static readonly MovieManager _movieManager = MovieManager.Instance;
        private static readonly OperatedLogManager _operatedLogManager = OperatedLogManager.Instance;

        public ActionResult SaveSettings()
        {
            MovieSettingsEntity args = RequestArgs<MovieSettingsEntity>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            args.Domain = DomainContext.Domain.Id;
            args.AppId = DomainContext.AppId;
            _movieManager.SaveSettings(DomainContext.Domain.Id, args);

            #region 操作日志

            _operatedLogManager.Create(new OperatedLogEntity()
            {
                Domain = DomainContext.Domain.Id,
                AppId = DomainContext.AppId,
                User = UserContext.User.Id,
                IP = Request.UserHostAddress,
                Module = EnumModule.Settings,
                Description = "更新电影设置"
            });

            #endregion

            return RespondResult();
        }
    }
}