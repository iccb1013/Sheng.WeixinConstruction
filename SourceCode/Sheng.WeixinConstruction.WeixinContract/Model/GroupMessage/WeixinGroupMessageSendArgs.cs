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
    /*
     * 根据OpenID列表群发【订阅号不可用，服务号认证后可用】
     * http://mp.weixin.qq.com/wiki/15/40b6865b893947b764e2de8e4a1fb55f.html#.E6.A0.B9.E6.8D.AEOpenID.E5.88.97.E8.A1.A8.E7.BE.A4.E5.8F.91.E3.80.90.E8.AE.A2.E9.98.85.E5.8F.B7.E4.B8.8D.E5.8F.AF.E7.94.A8.EF.BC.8C.E6.9C.8D.E5.8A.A1.E5.8F.B7.E8.AE.A4.E8.AF.81.E5.90.8E.E5.8F.AF.E7.94.A8.E3.80.91
     */

    /*
     * {
       "touser":[
        "OPENID1",
        "OPENID2"
       ],
       "mpnews":{
          "media_id":"123dsdajkasd231jhksad"
       },
        "msgtype":"mpnews"
    }
     */

    [DataContract]
    public class WeixinGroupMessageSendArgs
    {
        private List<string> _toUser = new List<string>();
        /// <summary>
        /// 填写图文消息的接收者，一串OpenID列表，OpenID最少2个，最多10000个
        /// </summary>
        [DataMember(Name = "touser")]
        public List<string> ToUser
        {
            get { return _toUser; }
            set { _toUser = value; }
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


    #region 图文消息

    [DataContract]
    public class WeixinMessageSendArgs_Mpnews : WeixinGroupMessageSendArgs
    {
        private WeixinGroupMessageSendAllArgs_MpnewsContent _mpnews = new WeixinGroupMessageSendAllArgs_MpnewsContent();
        [DataMember(Name = "mpnews")]
        public WeixinGroupMessageSendAllArgs_MpnewsContent Mpnews
        {
            get { return _mpnews; }
            set { _mpnews = value; }
        }

        public WeixinMessageSendArgs_Mpnews()
        {
            this.MsgType = "mpnews";
        }
    }

    #endregion
}
