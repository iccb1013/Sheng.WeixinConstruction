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
using Linkup.DataRelationalMapping;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("Message")]
    public class MessageEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Guid Domain
        {
            get;
            set;
        }

        public string AppId
        {
            get;
            set;
        }

        /// <summary>
        /// true:接收的消息  false：通过客服接口回复的消息
        /// </summary>
        public bool Receive
        {
            get;
            set;
        }

        /// <summary>
        /// 开发者微信号
        /// </summary>
        public string OfficalWeixinId
        {
            get;
            set;
        }

        /// <summary>
        /// 关注者的OpenId
        /// </summary>
        public string MemberOpenId
        {
            get;
            set;
        }

        /// <summary>
        /// 消息创建时间 （整型）
        /// </summary>
        public DateTime CreateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 接收到的消息类型
        /// </summary>
        [NotMapped]
        public EnumReceivingMessageType? ReceivingMessageType
        {
            get;
            set;
        }

        [Column("ReceivingMessageType")]
        public string ReceivingMessageTypeString
        {
            get
            {
                if (ReceivingMessageType.HasValue)
                    return EnumHelper.GetEnumMemberValue(ReceivingMessageType);
                else
                    return null;
            }
            set
            {
                if (String.IsNullOrEmpty(value)==false)
                    ReceivingMessageType = EnumHelper.GetEnumFieldByMemberValue<EnumReceivingMessageType>(value);
                else
                    ReceivingMessageType = null;
            }
        }

        /// <summary>
        /// 客服所回复的消息类型
        /// </summary>
        [NotMapped]
        public KFMessageType? ReplyMessageType
        {
            get;
            set;
        }

        [Column("ReplyMessageType")]
        public string ReplyMessageTypeString
        {
            get
            {
                if (ReplyMessageType.HasValue)
                    return EnumHelper.GetEnumMemberValue(ReplyMessageType);
                else
                    return null;
            }
            set
            {
                if (String.IsNullOrEmpty(value) == false)
                    ReplyMessageType = EnumHelper.GetEnumFieldByMemberValue<KFMessageType>(value);
                else
                    ReplyMessageType = null;
            }
        }

        /// <summary>
        /// 文本消息内容
        /// </summary>
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// 图片消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// 语音消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// 视频消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId
        {
            get;
            set;
        }

        /// <summary>
        /// 图片链接
        /// </summary>
        public string Image_PicUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 语音格式，如amr，speex等
        /// </summary>
        public string Voice_Format
        {
            get;
            set;
        }

        /// <summary>
        /// 开通语音识别后，用户每次发送语音给公众号时，微信会在推送的语音消息XML数据包中，增加一个Recongnition字段
        /// Recognition为语音识别结果，使用UTF8编码。
        /// </summary>
        public string Voice_Recognition
        {
            get;
            set;
        }

        /// <summary>
        /// 视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数据。
        /// 小视频
        /// </summary>
        public string ThumbMediaId
        {
            get;
            set;
        }

        /// <summary>
        /// 地理位置维度
        /// </summary>
        public decimal? Location_X
        {
            get;
            set;
        }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public decimal? Location_Y
        {
            get;
            set;
        }

        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public int? Location_Scale
        {
            get;
            set;
        }

        /// <summary>
        /// 地理位置信息
        /// </summary>
        public string Location_Label
        {
            get;
            set;
        }

        public string Link_Title
        {
            get;
            set;
        }

        public string Link_Description
        {
            get;
            set;
        }

        public string Link_Url
        {
            get;
            set;
        }

        /// <summary>
        /// 接收到的消息id，64位整型
        /// </summary>
        public long? MsgId
        {
            get;
            set;
        }

        /// <summary>
        /// 回复此消息的用户id
        /// </summary>
        public Guid? ReplyUser
        {
            get;
            set;
        }

        public string ContentType
        {
            get;
            set;
        }

        public string FileUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 文件大小（KB）
        /// </summary>
        public int FileLength
        {
            get;
            set;
        }

        public string ThumbContentType
        {
            get;
            set;
        }

        public string ThumbUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 文件大小（KB）
        /// </summary>
        public int ThumbLength
        {
            get;
            set;
        }

        public MessageEntity()
        {

        }
    }
}
