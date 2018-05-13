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
using Newtonsoft.Json.Linq;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class SettingsManager
    {
        private static readonly SettingsManager _instance = new SettingsManager();
        public static SettingsManager Instance
        {
            get { return _instance; }
        }

        private static readonly DomainManager _domainManager = DomainManager.Instance;
        private static readonly DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;
        private static readonly CachingService _caching = CachingService.Instance;
        private static readonly LogService _log = LogService.Instance;
        private static readonly PortalTemplatePool _portalTemplatePool = PortalTemplatePool.Instance;

        private string _clientAddress = System.Configuration.ConfigurationManager.AppSettings["ClientAddress"];
        private object _settingsLockObj = new object();

        private SettingsManager()
        {

        }

        public SettingsEntity GetSettings(Guid domainId, string appId)
        {
            if (String.IsNullOrEmpty(appId))
                return null;

            SettingsEntity settings = new SettingsEntity();
            settings.Domain = domainId;
            settings.AppId = appId;
            if (_dataBase.Fill<SettingsEntity>(settings) == false)
            {
                lock (_settingsLockObj)
                {
                    if (_dataBase.Fill<SettingsEntity>(settings) == false)
                    {
                        //初始化一条默认设置
                        _dataBase.Insert(settings);
                    }
                }
            }

            return settings;
        }

        public void SaveSettings(Guid domainId, SettingsEntity args)
        {
            _dataBase.Update(args);

            _domainManager.UpdateLastUpdateTime(domainId);
        }

        /// <summary>
        /// 获取客户端URL地址
        /// http://$APPID$.wxc.shengxunwei.com/
        /// </summary>
        /// <param name="domainContext"></param>
        /// <returns></returns>
        public string GetClientAddress(DomainContext domainContext)
        {
            return _clientAddress.Replace("$APPID$", domainContext.AppId);
        }

        public string GetClientAddress(string appId)
        {
            return _clientAddress.Replace("$APPID$", appId);
        }

        /// <summary>
        /// 获取完整的样式设置
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public StyleSettingsEntity GetStyleSettings(Guid domainId, string appId)
        {
            if (String.IsNullOrEmpty(appId))
                return null;

            StyleSettingsEntity settings = new StyleSettingsEntity();
            settings.Domain = domainId;
            settings.AppId = appId;
            if (_dataBase.Fill<StyleSettingsEntity>(settings) == false)
            {
                lock (_settingsLockObj)
                {
                    if (_dataBase.Fill<StyleSettingsEntity>(settings) == false)
                    {
                        //初始化一条默认设置
                        _dataBase.Insert(settings);
                    }
                }
            }

            //不管模式是不是模版，只要有值，就取出来
            //避免从自定义切回模版画面时不显示名称和说明
            if (settings.PortalPresetTemplateId.HasValue)
            {
                settings.PortalPresetTemplate =
                       _portalTemplatePool.GetPortalPresetTemplate(settings.PortalPresetTemplateId.Value);
            }
            else
            {
                if (settings.PortalMode == EnumPortalMode.Template)
                {
                    settings.PortalPresetTemplate = _portalTemplatePool.GetDefaultPortalPresetTemplate();
                    settings.PortalPresetTemplateId = settings.PortalPresetTemplate.Id;
                }
            }

            return settings;
        }

        #region Theme

        public ThemeStyleSettingsEntity GetThemeStyleSettings(Guid domainId, string appId)
        {
            if (String.IsNullOrEmpty(appId))
                return null;

            ThemeStyleSettingsEntity settings = new ThemeStyleSettingsEntity();
            settings.Domain = domainId;
            settings.AppId = appId;
            if (_dataBase.Fill<ThemeStyleSettingsEntity>(settings) == false)
            {
                lock (_settingsLockObj)
                {
                    if (_dataBase.Fill<ThemeStyleSettingsEntity>(settings) == false)
                    {
                        //初始化一条默认设置
                        _dataBase.Insert(settings);
                    }
                }
            }

            return settings;
        }

        public void SaveThemeStyleSettings(Guid domainId, ThemeStyleSettingsEntity args)
        {
            _dataBase.Update(args);

            _domainManager.UpdateLastUpdateTime(domainId);
        }

        #endregion

        #region Menu

        public ApiResult SaveAndPublishMenu(string json, UserContext userContext, DomainContext domainContext)
        {
            MenuEntity menuEntity = new MenuEntity();
            menuEntity.DomainId = userContext.User.Domain;
            menuEntity.Menu = json;
            _dataBase.Remove(menuEntity);
            _dataBase.Insert(menuEntity);

            ApiResult result = new ApiResult();

            //未认证订阅号没有自定义菜单权限
            if (domainContext.Authorizer == null ||
                domainContext.Authorizer.AuthorizationType == EnumAuthorizationType.UnauthorizedSubscription)
            {
                result.Success = false;
                result.Message = "未认证订阅号没有自定义菜单权限。";
                return result;
            }

            List<ButtonBase> _buttonList = new List<ButtonBase>();

            JObject jObject = JObject.Parse(json);

            #region 解析

            JArray buttonArray = (JArray)jObject["button"];

            foreach (var button in buttonArray)
            {
                if (button["sub_button"] == null || button["sub_button"].Count() == 0)
                {
                    ButtonBase newButton = null;
                    switch (button["type"].ToString())
                    {
                        case "wxc_function":
                            newButton = GetWxcFunctionButton(button, domainContext);
                            break;
                        case "click":
                            newButton = GetClickButton(button, domainContext);
                            break;
                        case "view":
                            newButton = JsonHelper.Deserialize<ButtonView>(button.ToString());
                            break;
                        case "wxc_campaign":
                            newButton = GetWxcCampaignButton(button, domainContext);
                            break;
                        case "wxc_information":
                            newButton = GetWxcInformationButton(button, domainContext);
                            break;
                        case "wxc_customForm":
                            newButton = GetWxcCustomFormButton(button, domainContext);
                            break;
                        case "mediaId":
                            newButton = GetMediaIdButton(button);
                            break;
                    }
                    _buttonList.Add(newButton);
                }
                else
                {
                    ParentButton parentButton = JsonHelper.Deserialize<ParentButton>(button.ToString());
                    parentButton.SubButton = new List<ButtonBase>();

                    JArray subButtonArray = (JArray)button["sub_button"];
                    foreach (var subButton in subButtonArray)
                    {
                        ButtonBase newSubButton = null;
                        switch (subButton["type"].ToString())
                        {
                            case "wxc_function":
                                newSubButton = GetWxcFunctionButton(subButton, domainContext);
                                break;
                            case "click":
                                newSubButton = GetClickButton(subButton, domainContext);
                                break;
                            case "view":
                                newSubButton = JsonHelper.Deserialize<ButtonView>(subButton.ToString());
                                break;
                            case "wxc_campaign":
                                newSubButton = GetWxcCampaignButton(subButton, domainContext);
                                break;
                            case "wxc_information":
                                newSubButton = GetWxcInformationButton(subButton, domainContext);
                                break;
                            case "wxc_customForm":
                                newSubButton = GetWxcCustomFormButton(subButton, domainContext);
                                break;
                            case "mediaId":
                                newSubButton = GetMediaIdButton(subButton);
                                break;
                        }
                        parentButton.SubButton.Add(newSubButton);
                    }

                    _buttonList.Add(parentButton);
                }
            }

            #endregion

            ButtonWrapper menu = new ButtonWrapper();
            menu.Button = _buttonList;

            RequestApiResult apiResult = MenuApi.Create(menu, domainContext.AccessToken);
            result.Success = apiResult.Success;
            result.Message = apiResult.Message;

            return result;
        }

        public MenuEntity GetMenu(Guid domainId)
        {
            MenuEntity menuEntity = new MenuEntity();
            menuEntity.DomainId = domainId;
            if (_dataBase.Fill<MenuEntity>(menuEntity))
                return menuEntity;
            else
                return null;
        }

        private ButtonView GetWxcFunctionButton(JToken json, DomainContext domainContext)
        {
            string clientAddress = _clientAddress.Replace("$APPID$", domainContext.AppId);

            ButtonView button = new ButtonView();
            button.Name = json["name"].ToString();
            string uri = null;
            switch (json["function"].ToString())
            {
                case "portal":
                    uri = "Home/Portal";
                    break;
                case "memberCenter":
                    uri = "Home/MemberCenter";
                    break;
                case "pointCommodity":
                    uri = "PointCommodity/PointCommodity";
                    break;
                case "pointCommodityOrderList":
                    uri = "PointCommodity/OrderList";
                    break;
                case "pointAccount":
                    uri = "Home/PointAccount";
                    break;
                case "cashAccount":
                    uri = "Pay/CashAccountTrack";
                    break;
                case "oneDollarBuying":
                    uri = "Campaign/OneDollarBuying";
                    break;
                case "personalInfo":
                    uri = "Home/PersonalInfo";
                    break;
                case "movieTimes":
                    uri = "Home/MovieTimes";
                    break;
                case "staff":
                    uri = "Staff/Home/Portal";
                    break;
                default:
                    Debug.Assert(false, "GetWxcFunctionButton：" + json["name"].ToString());
                    _log.Write("发布菜单时遇到未知的功能类型", json["function"].ToString(), TraceEventType.Error);
                    break;
            }

            button.Url = clientAddress + uri + "/" + domainContext.Domain.Id;

            return button;
        }

        private ButtonClick GetClickButton(JToken json, DomainContext domainContext)
        {
            ButtonClick button = new ButtonClick();
            button.Name = json["name"].ToString();
            string key = json["key"].ToString();
            switch (json["clickType"].ToString())
            {
                case "MemberQRCode":
                    key = "MemberQRCode:" + key;
                    break;
            }

            return button;
        }

        private ButtonView GetWxcCampaignButton(JToken json, DomainContext domainContext)
        {
            string clientAddress = _clientAddress.Replace("$APPID$", domainContext.AppId);

            EnumCampaignType type = (EnumCampaignType)Enum.Parse(typeof(EnumCampaignType), json["campaignType"].ToString());

            ButtonView button = new ButtonView();
            button.Name = json["name"].ToString();
            //http://wx8c36b3c0000a0a49.wxc.shengxunwei.com/Campaign/PictureVote/2a58d820-de07-4c8f-80b9-b5cb5a1028b4?id=16bc2e8e-8dbd-405a-85cc-34fb1879fd8a
            string campaignUri = null;
            switch (type)
            {
                case EnumCampaignType.PictureVote:
                    campaignUri = "PictureVote";
                    break;
                case EnumCampaignType.MemberQRCode:
                    campaignUri = "MemberQRCode";
                    break;
                case EnumCampaignType.Lottery:
                    campaignUri = "Lottery";
                    break;
                case EnumCampaignType.LuckyTicket:
                    campaignUri = "LuckyTicket";
                    break;
                case EnumCampaignType.ShakingLottery:
                    campaignUri = "ShakingLottery";
                    break;
                default:
                    Debug.Assert(false, "未知的活动类型");
                    _log.Write("发布菜单时遇到未知的活动类型", type.ToString(), TraceEventType.Error);
                    break;
            }

            button.Url = String.Format(clientAddress + "Campaign/{0}/{1}?campaignId={2}",
                campaignUri, domainContext.Domain.Id, json["campaign"].ToString());

            return button;
        }

        private ButtonView GetWxcInformationButton(JToken json, DomainContext domainContext)
        {
            string clientAddress = _clientAddress.Replace("$APPID$", domainContext.AppId);

            ButtonView button = new ButtonView();
            button.Name = json["name"].ToString();
            button.Url = String.Format(clientAddress + "Home/Information/{0}?informationId={1}",
               domainContext.Domain.Id, json["information"].ToString());

            return button;
        }

        private ButtonView GetWxcCustomFormButton(JToken json, DomainContext domainContext)
        {
            string clientAddress = _clientAddress.Replace("$APPID$", domainContext.AppId);

            ButtonView button = new ButtonView();
            button.Name = json["name"].ToString();
            button.Url = String.Format(clientAddress + "Home/CustomForm/{0}?formId={1}",
               domainContext.Domain.Id, json["customForm"].ToString());

            return button;
        }

        private ButtonMediaId GetMediaIdButton(JToken json)
        {
            ButtonMediaId button = new ButtonMediaId();
            button.Name = json["name"].ToString();
            button.MediaId = json["mediaId"].ToString();

            return button;
        }

        #endregion

        #region AutoReply

        public void SaveAutoReplyOnSubscribe(DomainContext domainContext, AutoReplyOnSubscribeEntity args)
        {
            if (args == null)
            {
                return;
            }

            lock (domainContext)
            {
                _dataBase.Remove(args);
                _dataBase.Insert(args);

                _domainManager.UpdateLastUpdateTime(domainContext.Domain.Id);
            }
        }

        public AutoReplyOnSubscribeEntity GetAutoReplyOnSubscribe(Guid domainId)
        {
            AutoReplyOnSubscribeEntity autoReplyOnSubscribeEntity = new AutoReplyOnSubscribeEntity();
            autoReplyOnSubscribeEntity.Domain = domainId;

            if (_dataBase.Fill<AutoReplyOnSubscribeEntity>(autoReplyOnSubscribeEntity))
                return autoReplyOnSubscribeEntity;
            else
                return null;
        }

        public void SaveAutoReplyOnMessage(DomainContext domainContext, AutoReplyOnMessageEntity args)
        {
            if (args == null)
            {
                return;
            }

            lock (domainContext)
            {
                _dataBase.Remove(args);
                _dataBase.Insert(args);

                _domainManager.UpdateLastUpdateTime(domainContext.Domain.Id);
            }
        }

        public AutoReplyOnMessageEntity GetAutoReplyOnMessage(Guid domainId)
        {
            AutoReplyOnMessageEntity autoReplyOnMessageEntity = new AutoReplyOnMessageEntity();
            autoReplyOnMessageEntity.Domain = domainId;

            if (_dataBase.Fill<AutoReplyOnMessageEntity>(autoReplyOnMessageEntity))
                return autoReplyOnMessageEntity;
            else
                return null;
        }

        public void AddAutoReplyOnKeyWordsRule(AutoReplyOnKeyWordsRuleEntity args)
        {
            if (args == null)
            {
                return;
            }

            args.CreateTime = DateTime.Now;
            _dataBase.Insert(args);

            if (args.ContentList != null)
            {
                foreach (var content in args.ContentList)
                {
                    content.Domain = args.Domain;
                    content.RuleId = args.Id;
                    _dataBase.Insert(content);
                }
            }

            _domainManager.UpdateLastUpdateTime(args.Domain);
        }

        public void UpdateAutoReplyOnKeyWordsRule(AutoReplyOnKeyWordsRuleEntity args)
        {
            if (args == null)
            {
                return;
            }

            RemoveAutoReplyOnKeyWordsRule(args.Domain, args.Id);
            AddAutoReplyOnKeyWordsRule(args);
        }

        public void RemoveAutoReplyOnKeyWordsRule(Guid domainId, Guid id)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            _dataBase.ExecuteNonQuery("DELETE FROM [AutoReplyOnKeyWordsRule] WHERE [Id] = @id",
                parameterList);

            _dataBase.ExecuteNonQuery("DELETE FROM [AutoReplyOnKeyWordsContent] WHERE [RuleId] = @id",
                parameterList);

            _domainManager.UpdateLastUpdateTime(domainId);
        }

        public AutoReplyOnKeyWordsRuleEntity GetAutoReplyOnKeyWordsRule(Guid id)
        {
            AutoReplyOnKeyWordsRuleEntity rule = new AutoReplyOnKeyWordsRuleEntity();
            rule.Id = id;
            if (_dataBase.Fill<AutoReplyOnKeyWordsRuleEntity>(rule))
            {
                List<AttachedWhereItem> attachedWhere = new List<AttachedWhereItem>();
                attachedWhere.Add(new AttachedWhereItem("RuleId", rule.Id));

                rule.ContentList = _dataBase.Select<AutoReplyOnKeyWordsContentEntity>(attachedWhere);

                return rule;
            }
            else
            {
                return null;
            }
        }

        public List<AutoReplyOnKeyWordsRuleEntity> GetAutoReplyOnKeyWordsRuleList(Guid domainId)
        {
            List<AttachedWhereItem> attachedWhere = new List<AttachedWhereItem>();
            attachedWhere.Add(new AttachedWhereItem("Domain", domainId));

            List<AutoReplyOnKeyWordsRuleEntity> ruleList =
                _dataBase.Select<AutoReplyOnKeyWordsRuleEntity>(attachedWhere);

            if (ruleList == null)
                return null;


            foreach (AutoReplyOnKeyWordsRuleEntity rule in ruleList)
            {
                attachedWhere = new List<AttachedWhereItem>();
                attachedWhere.Add(new AttachedWhereItem("RuleId", rule.Id));

                rule.ContentList = _dataBase.Select<AutoReplyOnKeyWordsContentEntity>(attachedWhere);
            }

            return ruleList;
        }

        #endregion

       
    }
}
