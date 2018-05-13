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
    //根据分组进行群发【订阅号与服务号认证后均可用】
    //http://mp.weixin.qq.com/wiki/15/40b6865b893947b764e2de8e4a1fb55f.html#.E6.A0.B9.E6.8D.AE.E5.88.86.E7.BB.84.E8.BF.9B.E8.A1.8C.E7.BE.A4.E5.8F.91.E3.80.90.E8.AE.A2.E9.98.85.E5.8F.B7.E4.B8.8E.E6.9C.8D.E5.8A.A1.E5.8F.B7.E8.AE.A4.E8.AF.81.E5.90.8E.E5.9D.87.E5.8F.AF.E7.94.A8.E3.80.91

    /*
     *{
           "filter":{
              "is_to_all":false
              "group_id":"2"
           },
           "mpnews":{
              "media_id":"123dsdajkasd231jhksad"
           },
            "msgtype":"mpnews"
        }
     */
    /// <summary>
    /// 不同类型的消息其json结构不同
    /// </summary>
    [DataContract]
    public abstract class WeixinGroupMessageSendAllArgs
    {
        private WeixinGroupMessageSendAllArgs_Filter _filter = new WeixinGroupMessageSendAllArgs_Filter();
        [DataMember(Name = "filter")]
        public WeixinGroupMessageSendAllArgs_Filter Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        /// <summary>
        /// 群发的消息类型，图文消息为mpnews，文本消息为text，语音为voice，音乐为music，
        /// 图片为image，视频为video，卡券为wxcard
        /// </summary>
        [DataMember(Name = "msgtype")]
        public string MsgType
        {
            get;
            set;
        }
    }

    [DataContract]
    public class WeixinGroupMessageSendAllArgs_Filter
    {
        [DataMember(Name = "is_to_all")]
        public bool IsToAll
        {
            get;
            set;
        }

        /// <summary>
        /// 群发接口中的GroupId是个string
        /// </summary>
        [DataMember(Name = "group_id")]
        public string GroupId
        {
            get;
            set;
        }
    }

    #region 图文消息

    [DataContract]
    public class WeixinGroupMessageSendAllArgs_Mpnews : WeixinGroupMessageSendAllArgs
    {
        private WeixinGroupMessageSendAllArgs_MpnewsContent _mpnews = new WeixinGroupMessageSendAllArgs_MpnewsContent();
        [DataMember(Name = "mpnews")]
        public WeixinGroupMessageSendAllArgs_MpnewsContent Mpnews
        {
            get { return _mpnews; }
            set { _mpnews = value; }
        }

        public WeixinGroupMessageSendAllArgs_Mpnews()
        {
            this.MsgType = "mpnews";
        }
    }

    [DataContract]
    public class WeixinGroupMessageSendAllArgs_MpnewsContent
    {
        [DataMember(Name = "media_id")]
        public string MediaId
        {
            get;
            set;
        }
    }

    #endregion

    #region 文本

    [DataContract]
    public class WeixinGroupMessageSendAllArgs_Text : WeixinGroupMessageSendAllArgs
    {
        private WeixinGroupMessageSendAllArgs_TextContent _content = new WeixinGroupMessageSendAllArgs_TextContent();
        [DataMember(Name = "text")]
        public WeixinGroupMessageSendAllArgs_TextContent Content
        {
            get { return _content; }
            set { _content = value; }
        }

        public WeixinGroupMessageSendAllArgs_Text()
        {
            this.MsgType = "text";
        }
    }

    [DataContract]
    public class WeixinGroupMessageSendAllArgs_TextContent
    {
        [DataMember(Name = "content")]
        public string Content
        {
            get;
            set;
        }
    }

    #endregion

    #region 图片

    [DataContract]
    public class WeixinGroupMessageSendAllArgs_Image : WeixinGroupMessageSendAllArgs
    {
        private WeixinGroupMessageSendAllArgs_ImageContent _image = new WeixinGroupMessageSendAllArgs_ImageContent();
        [DataMember(Name = "image")]
        public WeixinGroupMessageSendAllArgs_ImageContent Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public WeixinGroupMessageSendAllArgs_Image()
        {
            this.MsgType = "image";
        }
    }

    [DataContract]
    public class WeixinGroupMessageSendAllArgs_ImageContent
    {
        [DataMember(Name = "media_id")]
        public string MediaId
        {
            get;
            set;
        }
    }

    #endregion
}
