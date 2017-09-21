using Linkup.Common;
using Linkup.Data;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;


namespace Sheng.WeixinConstruction.Container
{
    /// <summary>
    /// 作为第三方平台运行时
    /// 处理授权事件消息
    /// </summary>
    public class ThirdPartyAuthHandler
    {
        private static ThirdPartyAuthHandler _instance;
        public static ThirdPartyAuthHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ThirdPartyAuthHandler();

                return _instance;
            }
        }

        //private CachingService _caching = CachingService.Instance;
        private DatabaseWrapper _dataBase = new DatabaseWrapper();
        private ThirdPartyManager _thirdPartyManager = ThirdPartyManager.Instance;
        private LogService _logService = LogService.Instance;

        private AuthorizerAccessTokenPool _accessTokenPool = AuthorizerAccessTokenPool.Instance;

        //长期运行发现，把ticket放在redis中，并不是100%可靠，存在写入不成功的情况
        //明明接收到了新的ticket，但是取accessToken时，从cache中取得的ticket是旧的
        //ticket并不需要放在redis中，用一个变量保存即可.
        private string _componentVerifyTicket;

        public ThirdPartyAuthHandler()
        {

        }

        public void Handle(string message)
        {
            XElement xml = XElement.Parse(message);

            string infoType = xml.XPathSelectElement("InfoType").Value;
            if (String.IsNullOrEmpty(infoType))
                return;

            switch (infoType)
            {
                case "component_verify_ticket":
                    ProcessComponentVerifyTicket(xml);
                    break;
                case "unauthorized":
                    ProcessUnauthorized(xml);
                    break;
            }
        }

        /// <summary>
        /// 推送component_verify_ticket协议
        /// </summary>
        /// <param name="xml"></param>
        private void ProcessComponentVerifyTicket(XElement xml)
        {
            string componentVerifyTicket = xml.XPathSelectElement("ComponentVerifyTicket").Value;
            if (String.IsNullOrEmpty(componentVerifyTicket))
                return;

            //写缓存
            //_caching.Set("ComponentVerifyTicket", componentVerifyTicket);
            _componentVerifyTicket = componentVerifyTicket;

            //写数据库
            try
            {
                List<CommandParameter> parameterList = new List<CommandParameter>();
                parameterList.Add(new CommandParameter("@componentVerifyTicket", componentVerifyTicket));

                object objCount = _dataBase.ExecuteScalar("SELECT Count(1) FROM [ThirdParty]");
                if (objCount == null || objCount == DBNull.Value || int.Parse(objCount.ToString()) == 0)
                {
                    _dataBase.ExecuteNonQuery("INSERT INTO [ThirdParty] ([ComponentVerifyTicket]) VALUES (@componentVerifyTicket)",
                       parameterList);
                }
                else
                {
                    _dataBase.ExecuteNonQuery("UPDATE [ThirdParty] SET [ComponentVerifyTicket] = @componentVerifyTicket",
                        parameterList);
                }
            }
            catch (Exception ex)
            {
                _logService.Write("ComponentVerifyTicket", "写入数据库失败。" + ex.Message, TraceEventType.Error);
            }
        }

        /// <summary>
        /// 取消授权通知
        /// </summary>
        /// <param name="xml"></param>
        private void ProcessUnauthorized(XElement xml)
        {
            string appId = xml.XPathSelectElement("AuthorizerAppid").Value;
            if (String.IsNullOrEmpty(appId))
                return;

            //终止此公众号的accessToken维护
            _accessTokenPool.Remove(appId);

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@appId", appId));

            Guid domainId = Guid.Empty;
            if (_dataBase.ExecuteScalar<Guid>(
                "SELECT [Domain] FROM [Authorizer] WHERE [AppId] = @appId AND [Online] = 1", parameterList,
                (scalarString) => { domainId = scalarString; }))
            {
                _thirdPartyManager.Unauthorized(domainId, appId);
            }
        }

        public string GetComponentVerifyTicket()
        {
            //string componentVerifyTicket = _caching.Get("ComponentVerifyTicket");
            if (String.IsNullOrEmpty(_componentVerifyTicket) == false)
            {
                return _componentVerifyTicket;
            }

            //读数据库
            object objComponentVerifyTicket =
                _dataBase.ExecuteScalar("SELECT [ComponentVerifyTicket] FROM [ThirdParty]");

            if (objComponentVerifyTicket == null || objComponentVerifyTicket == DBNull.Value)
                return null;

            _componentVerifyTicket = objComponentVerifyTicket.ToString();

            return _componentVerifyTicket;
        }
    }
}