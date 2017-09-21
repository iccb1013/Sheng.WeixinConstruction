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
        "msgtype":"image",
        "image":
        {
          "media_id":"MEDIA_ID"
        }
    }
     */

    /// <summary>
    /// 发送图片消息
    /// </summary>
    [DataContract]
    public class KFImageMessage : KFMessageBase
    {
        private KFImageMessage_Image _image = new KFImageMessage_Image();
        [DataMember(Name = "image")]
        public KFImageMessage_Image Image
        {
            get { return _image; }
            set { _image = value; }
        }


        public KFImageMessage()
        {
            this.MsgType = "image";
        }
    }

    [DataContract]
    public class KFImageMessage_Image
    {
        [DataMember(Name = "media_id")]
        public string MediaId
        {
            get;
            set;
        }
    }
}
