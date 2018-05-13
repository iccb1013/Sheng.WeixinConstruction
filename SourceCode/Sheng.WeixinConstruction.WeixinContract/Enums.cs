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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    /// <summary>
    /// 素材管理中
    /// 素材的类型，图片（image）、视频（video）、语音 （voice）、图文（news）
    /// 在使用素材时，有些接口图文是 mpnews
    /// 在有些接口中,news不是表示素材的，而是表示封装了一个完整的图文消息
    /// </summary>
    public enum MaterialType
    {
        /// <summary>
        /// 图片
        /// </summary>
        [EnumMember(Value = "image")]
        Image,
        /// <summary>
        /// 视频
        /// </summary>
        [EnumMember(Value = "video")]
        Video,
        /// <summary>
        /// 语音
        /// </summary>
        [EnumMember(Value = "voice")]
        Voice,
        /// <summary>
        /// 图文
        /// 注意：SB微信中的图文，有些地方是 news，有些地方是 mpnews
        /// 素材管理中是news，发消息是mpnews
        /// </summary>
        [EnumMember(Value = "news")]
        News
    }

    /// <summary>
    /// 接收普通消息时的消息类型
    /// http://mp.weixin.qq.com/wiki/17/f298879f8fb29ab98b2f2971d42552fd.html
    /// </summary>
    public enum EnumReceivingMessageType
    {
        /*
         *  1 文本消息
            2 图片消息
            3 语音消息
            4 视频消息
            5 小视频消息
            6 地理位置消息
            7 链接消息
         */
        /// <summary>
        /// 文本消息
        /// </summary>
        [EnumMember(Value = "text")]
        Text,
        /// <summary>
        /// 图片消息
        /// </summary>
        [EnumMember(Value = "image")]
        Image,
        /// <summary>
        /// 语音消息
        /// </summary>
        [EnumMember(Value = "voice")]
        Voice,
        /// <summary>
        /// 视频消息
        /// </summary>
        [EnumMember(Value = "video")]
        Video,
        /// <summary>
        /// 小视频消息
        /// </summary>
        [EnumMember(Value = "shortvideo")]
        Shortvideo,
        /// <summary>
        /// 地理位置消息
        /// </summary>
        [EnumMember(Value = "location")]
        Location,
        /// <summary>
        /// 链接消息
        /// </summary>
        [EnumMember(Value = "link")]
        Link
    }

    /// <summary>
    /// 客服接口所能够发送的消息类型
    /// http://mp.weixin.qq.com/wiki/11/c88c270ae8935291626538f9c64bd123.html#.E5.AE.A2.E6.9C.8D.E6.8E.A5.E5.8F.A3-.E5.8F.91.E6.B6.88.E6.81.AF
    /// </summary>
    public enum KFMessageType
    {
        /*
         *  1 文本消息
            2 图片消息
            3 语音消息
            4 视频消息
            5 小视频消息
            6 地理位置消息
            7 链接消息
         */
        /// <summary>
        /// 文本消息
        /// </summary>
        [EnumMember(Value = "text")]
        Text,
        /// <summary>
        /// 图片消息
        /// </summary>
        [EnumMember(Value = "image")]
        Image,
        /// <summary>
        /// 视频消息
        /// </summary>
        [EnumMember(Value = "video")]
        Video,
        /// <summary>
        /// 音乐消息
        /// </summary>
        [EnumMember(Value = "music")]
        Music,
        /// <summary>
        /// 图文消息（点击跳转到外链）
        /// </summary>
        [EnumMember(Value = "news")]
        News,
        /// <summary>
        /// 图文消息（点击跳转到图文消息页面） 
        /// </summary>
        [EnumMember(Value = "mpnews")]
        Mpnews,
        /// <summary>
        /// 卡券
        /// </summary>
        [EnumMember(Value = "wxcard")]
        Wxcard
    }

}
