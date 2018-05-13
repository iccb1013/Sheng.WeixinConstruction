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
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using Sheng.WeixinConstruction.WeixinContract.ThirdParty;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class ThirdPartyApiWrapper
    {
        private static readonly LogService _log = LogService.Instance;
        private static readonly WxConfigurationSection _configuration = ConfigService.Instance.Configuration;

        /// <summary>
        /// 使用授权码换取公众号的授权信息
        /// </summary>
        public static RequestApiResult<WeixinThirdPartyGetAuthorizationInfoResult> GetAuthorizationInfo(string authorizationCode)
        {
            WeixinThirdPartyGetAuthorizationInfoArgs args = new WeixinThirdPartyGetAuthorizationInfoArgs();
            args.ComponentAppId = _configuration.ThirdParty.AppId;
            args.AuthorizationCode = authorizationCode;

            string accessToken = ThirdPartyAccessTokenGetter.Get();

            RequestApiResult<WeixinThirdPartyGetAuthorizationInfoResult> result =
                ThirdPartyApi.GetAuthorizationInfo(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = ThirdPartyAccessTokenGetter.Refresh(accessToken);
                    }

                    result = ThirdPartyApi.GetAuthorizationInfo(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("ThirdPartyApi.GetAuthorizationInfo 失败",
                            result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("ThirdPartyApi.GetAuthorizationInfo 失败",
                           result.GetDetail(), TraceEventType.Warning);
                }
            }

            //此时数据库中可能还没有建立记录，保存refreshtoken的工作在调用处做

            return result;
        }

        public static RequestApiResult<WeixinThirdPartyGetAuthorizerAccessTokenResult> GetAuthorizerAccessToken(string appId, string refreshToken)
        {
            WeixinThirdPartyGetAuthorizerAccessTokenArgs args = new WeixinThirdPartyGetAuthorizerAccessTokenArgs();
            args.ComponentAppId = _configuration.ThirdParty.AppId;
            args.AuthorizerAppId = appId;
            args.AuthorizerRefreshToken = refreshToken;

            string accessToken = ThirdPartyAccessTokenGetter.Get();
            RequestApiResult<WeixinThirdPartyGetAuthorizerAccessTokenResult> result =
                ThirdPartyApi.GetAuthorizerAccessToken(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = ThirdPartyAccessTokenGetter.Refresh(accessToken);
                    }

                    result = ThirdPartyApi.GetAuthorizerAccessToken(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("ThirdPartyApi.GetAuthorizerAccessToken 失败",
                             result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("ThirdPartyApi.GetAuthorizerAccessToken 失败",
                             result.GetDetail(), TraceEventType.Warning);
                }
            }
            else
            {
                //保存新的RefreshToken到数据库
                //非常重要，一旦丢失则需要公众号重新授权
                WeixinThirdPartyGetAuthorizerAccessTokenResult token = result.ApiResult;
                ThirdPartyManager.Instance.UpdateAuthorizerRefreshToken(
                    appId, token.AccessToken, DateTime.Now.AddSeconds(token.ExpiresIn), token.RefreshToken);
            }

            return result;
        }

        public static RequestApiResult<WeixinThirdPartyGetAuthorizerAccountInfoResult> GetAuthorizerAccountInfo(string appId)
        {
            WeixinThirdPartyGetAuthorizerAccountInfoArgs args = new WeixinThirdPartyGetAuthorizerAccountInfoArgs();
            args.ComponentAppId = _configuration.ThirdParty.AppId;
            args.AuthorizerAppId = appId;

            string accessToken = ThirdPartyAccessTokenGetter.Get();
            RequestApiResult<WeixinThirdPartyGetAuthorizerAccountInfoResult> result =
                ThirdPartyApi.GetAuthorizerAccountInfo(accessToken, args);

            if (result.Success == false)
            {
                if (result.Retry)
                {
                    if (result.ApiError.ErrorCode == 40001)
                    {
                        accessToken = ThirdPartyAccessTokenGetter.Refresh(accessToken);
                    }

                    result = ThirdPartyApi.GetAuthorizerAccountInfo(accessToken, args);
                    if (result.Success == false)
                    {
                        _log.Write("ThirdPartyApi.GetAuthorizerAccountInfo 失败",
                             result.GetDetail(), TraceEventType.Warning);
                    }
                }
                else
                {
                    _log.Write("ThirdPartyApi.GetAuthorizerAccountInfo 失败",
                             result.GetDetail(), TraceEventType.Warning);
                }
            }

            return result;
        }
        
       
    }
}
