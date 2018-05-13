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
using Linkup.Data;
using Linkup.DataRelationalMapping;
using Newtonsoft.Json;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using Sheng.WeixinConstruction.WeixinContract.ThirdParty;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    /// <summary>
    /// 管理第三方平台运营时的相关数据
    /// </summary>
    public class ThirdPartyManager
    {
        private static readonly ThirdPartyManager _instance = new ThirdPartyManager();
        public static ThirdPartyManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;
        private LogService _log = LogService.Instance;
        private FileService _fileService = FileService.Instance;

        private ThirdPartyManager()
        {

        }

        /// <summary>
        /// 根据授权码开始维护一个新的公众号Token
        /// 此方法在 Container 中调用
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="authCode"></param>
        /// <returns></returns>
        public RequestApiResult<WeixinThirdPartyGetAuthorizationInfoResult> CreateAuthorizer(Guid domainId, string authCode)
        {
            RequestApiResult<WeixinThirdPartyGetAuthorizationInfoResult> authorizationInfoResult =
                ThirdPartyApiWrapper.GetAuthorizationInfo(authCode);
            if (authorizationInfoResult.Success == false)
            {
                return authorizationInfoResult;
            }

            WeixinThirdPartyAuthorizationInfo info = authorizationInfoResult.ApiResult.AuthorizationInfo;

            //一个公众号不能同时授权给两个帐户，因为微信在推送数据时只带一个APPID，我无法判断其属于哪个Domain
            //但是允许其在解除授权后得新授权给另一个帐户
            //所以首先判断有没有已经授权过的且online的domain，如果有，先解除原有授权
            //解除的办法直接更新原domain的lastDockingDate，并把Online置为0，
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", info.AppId));

            DataTable dt = _dataBase.ExecuteDataSet(
                "SELECT [Domain] FROM [Authorizer] WHERE [Online] = 1 AND [AppId] = @appId AND [Domain] <> @domainId",
                parameterList, new string[] { "table" }).Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                //解除授权
                Unauthorized(Guid.Parse(dr["Domain"].ToString()), info.AppId);
            }


            //还有可能是同一个Domain以前授权过的，这种情况直接更新即可
            AuthorizerEntity entity = new AuthorizerEntity();
            entity.AppId = info.AppId;
            entity.Domain = domainId;

            bool exist = _dataBase.Fill<AuthorizerEntity>(entity);

            //保存RefreshToken到数据库
            //非常重要，一旦丢失则需要公众号重新授权
            entity.AccessToken = info.AccessToken;
            entity.AccessTokenExpiryTime = DateTime.Now.AddSeconds(info.ExpiresIn);
            entity.RefreshToken = info.RefreshToken;
            entity.RefreshTokenGetTime = DateTime.Now;
            ////////////

            entity.AuthorizationTime = DateTime.Now;
            entity.Online = true;
            entity.FuncScopeCategory = info.FuncScopeCategoryList.ToString();

            if (exist)
            {
                _dataBase.Update(entity);
            }
            else
            {
                _dataBase.Insert(entity);
            }

            //更新LastDockingTime
            DomainManager.Instance.UpdateLastDockingTime(domainId);

            return authorizationInfoResult;
        }

        //解除授权
        public void Unauthorized(Guid domainId, string appId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));

            _dataBase.ExecuteNonQuery(
                "UPDATE [Authorizer] SET [Online] = 0 WHERE [AppId] = @appId AND [Domain] = @domainId", 
                parameterList);

            //更新LastUpdateTime
            DomainManager.Instance.UpdateLastDockingTime(domainId);
        }

        /// <summary>
        /// 从数据库中取出平台自身的token
        /// </summary>
        public WeixinThirdPartyGetAccessTokenResult GetAccessToken()
        {
            DataTable dt = _dataBase.ExecuteDataSet(
                "SELECT [AccessToken],[AccessTokenExpiryTime] FROM [ThirdParty]", new string[] { "table" }).Tables[0];
            if (dt.Rows.Count == 0)
            {
                return null;
            }

            WeixinThirdPartyGetAccessTokenResult token = new WeixinThirdPartyGetAccessTokenResult();

            token.AccessToken = dt.Rows[0]["AccessToken"].ToString();
            if (dt.Rows[0]["AccessTokenExpiryTime"] != null && dt.Rows[0]["AccessTokenExpiryTime"].ToString() != String.Empty)
            {
                token.AccessTokenExpiryTime = DateTime.Parse(dt.Rows[0]["AccessTokenExpiryTime"].ToString());
            }
            else
            {
                token.AccessTokenExpiryTime = DateTime.MinValue;
            }

            return token;
        }

        /// <summary>
        /// 更新平台自身的token，用于启动时恢复
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="accessTokenExpiryTime"></param>
        public void UpdateAccessToken(string accessToken, DateTime accessTokenExpiryTime)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@accessToken", accessToken));
            parameterList.Add(new CommandParameter("@accessTokenExpiryTime", accessTokenExpiryTime));

            _dataBase.ExecuteNonQuery(
                "UPDATE [ThirdParty] SET [AccessToken] = @accessToken,[AccessTokenExpiryTime] = @accessTokenExpiryTime",
                parameterList);
        }

        public string GetAuthorizerRefreshToken(string appId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@appId", appId));

            object objRefreshToken = _dataBase.ExecuteScalar(
                "SELECT [RefreshToken] FROM [Authorizer] WHERE [AppId] = @appId", parameterList);

            if (objRefreshToken == null || objRefreshToken == DBNull.Value)
            {
                return null;
            }
            else
            {
                return objRefreshToken.ToString();
            }
        }

        public void UpdateAuthorizerRefreshToken(string appId, string accessToken, DateTime accessTokenExpiryTime, string refreshToken)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@accessToken", accessToken));
            parameterList.Add(new CommandParameter("@accessTokenExpiryTime", accessTokenExpiryTime));
            parameterList.Add(new CommandParameter("@refreshToken", refreshToken));

            _dataBase.ExecuteNonQuery(
                "UPDATE [Authorizer] SET [AccessToken] = @accessToken,[AccessTokenExpiryTime] = @accessTokenExpiryTime, [RefreshToken] = @refreshToken,[RefreshTokenGetTime] = GETDATE() WHERE [AppId] = @appId", parameterList);
        }

        public NormalResult UpdateAuthorizerAccountInfo(Guid domainId, string appId)
        {
            //获取公众号帐户信息
            RequestApiResult<WeixinThirdPartyGetAuthorizerAccountInfoResult> accountInfoResult =
                ThirdPartyApiWrapper.GetAuthorizerAccountInfo(appId);

            if (accountInfoResult.Success)
            {
                WeixinThirdPartyAuthorizerAccountInfo account = accountInfoResult.ApiResult.AccountInfo;

                //微信返回的二维码图片不允许外部引用，此处必须把图片下载到本地
                FileDownloadAgentArgs downloadAgentArgs = new FileDownloadAgentArgs();
                downloadAgentArgs.Domain = domainId;
                downloadAgentArgs.Url = account.QRCodeUrl;
                FileDownloadAgentResult result = _fileService.DownloadAgent(downloadAgentArgs);
                string qrCodeUrl;
                if (result.Success)
                {
                    _log.Write("下载二维码返回", JsonConvert.SerializeObject(result), TraceEventType.Verbose);
                    qrCodeUrl = _fileService.FileServiceUri + result.OutputFile;
                }
                else
                {
                    qrCodeUrl = account.QRCodeUrl;
                }

                SqlStructureBuild sqlBuild = new SqlStructureBuild();
                sqlBuild.Table = "Authorizer";
                sqlBuild.Type = SqlExpressionType.Update;
                sqlBuild.AddParameter("AppId", appId, true);
                sqlBuild.AddParameter("Domain", domainId, true);
                sqlBuild.AddParameter("NickName", account.NickName);
                sqlBuild.AddParameter("HeadImg", account.HeadImg);
                sqlBuild.AddParameter("ServiceType", account.ServiceType.Id);
                sqlBuild.AddParameter("VerifyType", account.VerifyType.Id);
                sqlBuild.AddParameter("UserName", account.UserName);
                sqlBuild.AddParameter("Alias", account.Alias);
                sqlBuild.AddParameter("QRCodeUrl", qrCodeUrl);
                sqlBuild.AddParameter("Store", account.Business.Store);
                sqlBuild.AddParameter("Scan", account.Business.Scan);
                sqlBuild.AddParameter("Pay", account.Business.Pay);
                sqlBuild.AddParameter("Card", account.Business.Card);
                sqlBuild.AddParameter("Shake", account.Business.Shake);
                sqlBuild.AddParameter("FuncScopeCategory",
                    accountInfoResult.ApiResult.AuthorizationInfo.FuncScopeCategoryList.ToString());

                _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());

                //更新LastUpdateTime
                DomainManager.Instance.UpdateLastUpdateTime(domainId);

            }

            NormalResult normalResult = new NormalResult();
            normalResult.Success = accountInfoResult.Success;
            normalResult.Message = accountInfoResult.Message;

            return normalResult;
        }

        /// <summary>
        /// 从数据库中恢复token数据
        /// </summary>
        /// <returns></returns>
        public List<AuthorizerAccessTokenWrapper> GetAuthorizerAccessTokenList()
        {
            DataTable dt = _dataBase.ExecuteDataSet(
                "SELECT [AppId],[RefreshToken],[AccessToken],[AccessTokenExpiryTime] FROM [Authorizer] WHERE [Online] = 1",
                new string[] { "table" }).Tables[0];

            List<AuthorizerAccessTokenWrapper> list = RelationalMappingUnity.Select<AuthorizerAccessTokenWrapper>(dt);

            return list;
        }
        
        /// <summary>
        /// 保存JsApiTicket数据到数据库
        /// </summary>
        /// <returns></returns>
        public void UpdateAuthorizerJsApiTicket(string appId, string jsApiTicket, DateTime jsApiTicketExpiryTime)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@jsApiTicket", jsApiTicket));
            parameterList.Add(new CommandParameter("@jsApiTicketExpiryTime", jsApiTicketExpiryTime));

            _dataBase.ExecuteNonQuery(
                "UPDATE [Authorizer] SET [JsApiTicket] = @jsApiTicket,[JsApiTicketExpiryTime] = @jsApiTicketExpiryTime WHERE [AppId] = @appId", parameterList);
        }

        /// <summary>
        /// 从数据库中恢复JsApiTicket数据
        /// </summary>
        /// <returns></returns>
        public List<AuthorizerJsApiTicketWrapper> GetAuthorizerJsApiTicketList()
        {
            DataTable dt = _dataBase.ExecuteDataSet(
                "SELECT [AppId],[JsApiTicket],[JsApiTicketExpiryTime] FROM [Authorizer] WHERE [Online] = 1",
                new string[] { "table" }).Tables[0];

            List<AuthorizerJsApiTicketWrapper> list = RelationalMappingUnity.Select<AuthorizerJsApiTicketWrapper>(dt);

            return list;
        }
    }
}
