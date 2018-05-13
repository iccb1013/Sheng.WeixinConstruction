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
     {
        "touser":"OPENID",
        "msgtype":"text",
        "text":
        {
             "content":"Hello World"
        }
     }
     * http://mp.weixin.qq.com/wiki/11/c88c270ae8935291626538f9c64bd123.html#.E5.AE.A2.E6.9C.8D.E6.8E.A5.E5.8F.A3-.E5.8F.91.E6.B6.88.E6.81.AF
     */

    /// <summary>
    /// 发送文本消息
    /// </summary>
    [DataContract]
    public class KFTextMessage : KFMessageBase
    {
        private KFTextMessage_Text _text = new KFTextMessage_Text();
        [DataMember(Name = "text")]
        public KFTextMessage_Text Text
        {
            get { return _text; }
            //set { _text = value; }
        }

        public KFTextMessage()
        {
            this.MsgType = "text";
        }
    }

    [DataContract]
    public class KFTextMessage_Text
    {
        [DataMember(Name = "content")]
        public string Content
        {
            get;
            set;
        }
    }
}
