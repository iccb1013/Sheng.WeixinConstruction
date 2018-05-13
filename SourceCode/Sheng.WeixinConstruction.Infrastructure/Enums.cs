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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    /// <summary>
    /// 帐户类型
    /// </summary>
    public enum EnumDomainType
    {
        /// <summary>
        /// 免费帐户
        /// </summary>
        Free = 0,
        /// <summary>
        /// 付费帐户
        /// </summary>
        Paid = 1,
        /// <summary>
        /// 赞助帐户
        /// 介于免费和付费之前，功能等于付费帐户，但会显示不太明显的小广告
        /// </summary>
        Sponsor = 2
    }

    public enum EnumPortalMode
    {
        Template = 0,
        Custom = 1
    }

    public enum EnumSort
    {
        ASC,
        DESC
    }

    public enum EnumAutoReplyType
    {
        /// <summary>
        /// 文本
        /// </summary>
        Text = 0,
        /// <summary>
        /// 图片
        /// </summary>
        Image = 1,
        /// <summary>
        /// 文章
        /// </summary>
        Article = 2,
    }

    /// <summary>
    /// 群发消息的类型
    /// </summary>
    public enum EnumGroupMessageType
    {
        /// <summary>
        /// 文本
        /// </summary>
        Text = 0,
        /// <summary>
        /// 图片
        /// </summary>
        Image = 1,
        /// <summary>
        /// 文章
        /// </summary>
        Article = 2,
    }

    /// <summary>
    /// 公众号类型的接口权限类型
    /// 1未认证订阅号 2微信认证订阅号 3未认证服务号 4微信认证服务号
    /// </summary>
    public enum EnumAuthorizationType
    {
        Unknow = 0,
        /// <summary>
        /// 未认证订阅号
        /// </summary>
        UnauthorizedSubscription = 1,
        /// <summary>
        /// 微信认证订阅号
        /// </summary>
        AuthorizedSubscription = 2,
        /// <summary>
        /// 未认证服务号
        /// </summary>
        UnauthorizedService = 3,
        /// <summary>
        /// 微信认证服务号
        /// </summary>
        AuthorizedService = 4,
    }

    public enum EnumModule
    {
        Unknow = 0,
        /// <summary>
        /// 系统
        /// </summary>
        System = 1,
        /// <summary>
        /// 设置
        /// </summary>
        Settings = 2,
        /// <summary>
        /// 会员
        /// </summary>
        Member = 3,
        /// <summary>
        /// 活动
        /// </summary>
        Campaign = 4,
        /// <summary>
        /// 分类信息
        /// </summary>
        Information = 5,
        /// <summary>
        /// 素材
        /// </summary>
        Material = 6,
        /// <summary>
        /// 积分商品
        /// </summary>
        PointCommodity = 7,
        /// <summary>
        /// 场景二维码
        /// </summary>
        ScenicQRCode = 8,
        /// <summary>
        /// 自定义页面
        /// </summary>
        Page = 9,
        /// <summary>
        /// 自定义表单
        /// </summary>
        CustomForm = 10,
        /// <summary>
        /// 微官网
        /// </summary>
        Portal = 11,
        /// <summary>
        /// 卡券
        /// </summary>
        Coupon = 12,
        /// <summary>
        /// 微信支付
        /// </summary>
        Pay = 13,
        /// <summary>
        /// 用户
        /// </summary>
        User = 14,
        /// <summary>
        /// 1元抢购
        /// </summary>
        OneDollarBuying = 15,
        /// <summary>
        /// 高级图文
        /// </summary>
        AdvancedArticle = 16

    }

}
