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
    /// 图文消息
    /// 有两种，一种是被动回复的XML格式
    /// 一种是客服接口所使用的JSON格式 
    /// 但是客服接口使用的图文消息又分两种：点击跳转到外链，点击跳转到图文消息页面，格式又不同
    /// 此处定义的是：客服接口使用,点击跳转到图文消息页面
    /// </summary>
    [DataContract]
    public class KFMpArticleMessage : KFMessageBase
    {
        private KfMpArticleMessage_Mpnews _mpnews = new KfMpArticleMessage_Mpnews();
        [DataMember(Name = "mpnews")]
        public KfMpArticleMessage_Mpnews Mpnews
        {
            get { return _mpnews; }
            set { _mpnews = value; }
        }

        public KFMpArticleMessage()
        {
            this.MsgType = "mpnews";
        }
    }

    [DataContract]
    public class KfMpArticleMessage_Mpnews
    {
        [DataMember(Name = "media_id")]
        public string MediaId
        {
            get;
            set;
        }
    }
}
