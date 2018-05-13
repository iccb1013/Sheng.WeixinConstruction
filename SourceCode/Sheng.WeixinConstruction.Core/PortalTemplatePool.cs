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


using Linkup.Data;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class PortalTemplatePool
    {
        private static readonly PortalTemplatePool _instance = new PortalTemplatePool();
        public static PortalTemplatePool Instance
        {
            get { return _instance; }
        }

        //_instance = new PortalTemplatePool(); 的时候走不到这里
        private readonly DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        /// <summary>
        /// Key:Id Value:PortalPresetTemplateEntity
        /// </summary>
        private Hashtable _templateCached = new Hashtable();

        //刷新
        private Timer _refreshTimer;

        private PortalTemplatePool()
        {
            RefreshTimerCallback(null);

            _refreshTimer = new System.Threading.Timer(RefreshTimerCallback, null, 15 * 60 * 1000, 15 * 60 * 1000);
        }

        private void RefreshTimerCallback(object state)
        {
            List<PortalPresetTemplateEntity> list = _dataBase.Select<PortalPresetTemplateEntity>();
            foreach (var item in list)
            {
                if (_templateCached.ContainsKey(item.Id))
                {
                    _templateCached[item.Id] = item;
                }
                else
                {
                    _templateCached.Add(item.Id, item);
                }
            }
        }

        public PortalPresetTemplateEntity GetPortalPresetTemplate(Guid id)
        {
            if (_templateCached.ContainsKey(id))
            {
                return _templateCached[id] as PortalPresetTemplateEntity;
            }
            else
            {
                return null;
            }
        }

        public PortalPresetTemplateEntity GetDefaultPortalPresetTemplate()
        {
            return GetPortalPresetTemplate(Guid.Parse("D70FC90B-461D-4A06-8F73-39ABCBD6480B"));
        }


        public PortalPresetTemplateEntity GetPortalPresetTemplateDigest(Guid id)
        {
            if (_templateCached.ContainsKey(id))
            {
                PortalPresetTemplateEntity template = _templateCached[id] as PortalPresetTemplateEntity;
                PortalPresetTemplateEntity result = new PortalPresetTemplateEntity();
                result.Id = template.Id;
                result.Name = template.Name;
                result.Description = template.Description;
                result.PreviewImageUrl = template.PreviewImageUrl;
                result.BackgroundImageUrl = template.BackgroundImageUrl;
                result.PayRequired = template.PayRequired;
                return result;
            }
            else
            {
                return null;
            }
        }

        public List<PortalPresetTemplateEntity> GetPortalPresetTemplateList()
        {
            return _templateCached.Values.Cast<PortalPresetTemplateEntity>().ToList();
        }

        /*
         * {{#PortalImageUrl}}
         * {{#HeadImageUrl_64}} 当前用户头像 64px
         * {{#NickName}}  当前用户昵称
         * {{#Point}}  当前用户积分
         * {{#PointCommodityUrl}} 积分商城Url
         * {{#PointCommodityOrderListUrl}} 积分订单Url
         * {{#CashAccount}} 当前用户现金账户余额
         * {{#PayOrderListUrl}} 现金账户订单Url
         * {{#CashAccountTrackUrl}} 现金账户信息Url
         * {{#CashAccountDepositUrl}} 现金账户充值Url
         * {{#PointAccountUrl}} 帐户信息Url
         * {{#MemberCenterUrl}} 会员中心Url
         * {{#PersonalInfoUrl}} 个人信息Url
         * {{#CampaignListUrl}} 当前活动Url
         *  
         * signIn()  签到，<span id="spanSignIn">每日签到</span>
         * 
         * 
         */
        public string Render(PortalPresetTemplateEntity template, DomainContext domainContext, MemberContext memberContent)
        {
            if (template == null || String.IsNullOrEmpty(template.Template) || domainContext == null || memberContent == null)
                return String.Empty;

            StringBuilder result = new StringBuilder(template.Template);
            if (String.IsNullOrEmpty(domainContext.PortalImageUrl))
            {
                result.Replace("{{#PortalImageUrl}}", template.BackgroundImageUrl);
            }
            else
            {
                result.Replace("{{#PortalImageUrl}}", domainContext.PortalImageUrl);
            }
            result.Replace("{{#HeadImageUrl_64}}", memberContent.Member.Headimgurl_64);
            result.Replace("{{#NickName}}", memberContent.Member.NickName);
            result.Replace("{{#Point}}", memberContent.Member.Point.ToString());
            result.Replace("{{#PointCommodityUrl}}", "/PointCommodity/PointCommodity/" + domainContext.Domain.Id);
            result.Replace("{{#PointCommodityOrderListUrl}}", "/PointCommodity/OrderList/" + domainContext.Domain.Id);
            result.Replace("{{#PointAccountUrl}}", "/Home/PointAccount/" + domainContext.Domain.Id);
            result.Replace("{{#CashAccount}}", (memberContent.Member.CashAccount/100f).ToString());
            result.Replace("{{#PayOrderListUrl}}", "/Pay/PayOrderList/" + domainContext.Domain.Id);
            result.Replace("{{#CashAccountTrackUrl}}", "/Pay/CashAccountTrack/" + domainContext.Domain.Id);
            result.Replace("{{#CashAccountDepositUrl}}", "/Pay/Deposit/" + domainContext.Domain.Id);
            result.Replace("{{#MemberCenterUrl}}", "/Home/MemberCenter/" + domainContext.Domain.Id);
            result.Replace("{{#PersonalInfoUrl}}", "/Home/PersonalInfo/" + domainContext.Domain.Id);
            result.Replace("{{#CampaignListUrl}}", "/Campaign/CampaignList/" + domainContext.Domain.Id);

            return result.ToString();
        }

        public string Render(string template, DomainContext domainContext, MemberContext memberContent)
        {
            if (String.IsNullOrEmpty(template) || domainContext == null || memberContent == null)
                return String.Empty;

            StringBuilder result = new StringBuilder(template);

            result.Replace("{{#HeadImageUrl_64}}", memberContent.Member.Headimgurl_64);
            result.Replace("{{#NickName}}", memberContent.Member.NickName);
            result.Replace("{{#Point}}", memberContent.Member.Point.ToString());
            result.Replace("{{#PointCommodityUrl}}", "/PointCommodity/PointCommodity/" + domainContext.Domain.Id);
            result.Replace("{{#PointCommodityOrderListUrl}}", "/PointCommodity/OrderList/" + domainContext.Domain.Id);
            result.Replace("{{#PointAccountUrl}}", "/Home/PointAccount/" + domainContext.Domain.Id);
            result.Replace("{{#CashAccount}}", (memberContent.Member.CashAccount / 100f).ToString());
            result.Replace("{{#PayOrderListUrl}}", "/Pay/PayOrderList/" + domainContext.Domain.Id);
            result.Replace("{{#CashAccountTrackUrl}}", "/Pay/CashAccountTrack/" + domainContext.Domain.Id);
            result.Replace("{{#CashAccountDepositUrl}}", "/Pay/Deposit/" + domainContext.Domain.Id);
            result.Replace("{{#MemberCenterUrl}}", "/Home/MemberCenter/" + domainContext.Domain.Id);
            result.Replace("{{#PersonalInfoUrl}}", "/Home/PersonalInfo/" + domainContext.Domain.Id);
            result.Replace("{{#CampaignListUrl}}", "/Campaign/CampaignList/" + domainContext.Domain.Id);

            return result.ToString();
        }
    }
}
