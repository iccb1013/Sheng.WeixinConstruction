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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Areas.Api.Controllers
{
    public class SMSController : ApiBasalController
    {
        private static readonly SMSService _smsService = SMSService.Instance;
        private static readonly LimitedRequestHelper _limitedRequestHelper = LimitedRequestHelper.Instance;

        private TimeSpan _validateCodeExpiresIn = new TimeSpan(0, 5, 0);
        private Random _random = new Random(DateTime.Now.Millisecond);


        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <returns></returns>
        [AllowedAnonymous]
        public ActionResult SendMobilePhoneValidateCode()
        {
            if (_limitedRequestHelper.Shoot("SMS", Request.UserHostAddress) == false)
            {
                return RespondResult(false, "您的请求过频繁，请稍候再试。");
            }

            string phoneNumber = Request.QueryString["phoneNumber"];
            if (String.IsNullOrEmpty(phoneNumber))
            {
                return RespondResult(false, "参数无效。");
            }

            string validateCode = _random.Next(10000, 99999).ToString();

            NormalResult result = _smsService.Send(
                "升讯威", "SMS_13063249", phoneNumber, "{\"code\":\"" + validateCode + "\"}");

            if (result.Success)
            {
                _cachingService.Set(phoneNumber, validateCode, _validateCodeExpiresIn);
                return RespondResult();
            }
            else
            {
                return RespondResult(false, result.Message);
            }
        }

    }
}