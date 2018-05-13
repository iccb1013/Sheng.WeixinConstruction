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


using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    /// <summary>
    /// 群发消息
    /// </summary>
    [Table("GroupMessage")]
    public class GroupMessageEntity
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
        /// 用于设定是否向全部用户发送，值为true或false，选择true该消息群发给所有用户，
        /// 选择false可根据group_id发送给指定群组的用户
        /// </summary>
        public bool IsToAll
        {
            get;
            set;
        }

        /// <summary>
        /// 群发到的分组的group_id，参加用户管理中用户分组接口，若is_to_all值为true，可不填写group_id
        /// </summary>
        public int? GroupId
        {
            get;
            set;
        }

        /// <summary>
        /// 全部
        /// 或分组名
        /// </summary>
        [NotMapped]
        public string FilterOption
        {
            get;
            set;
        }

        public EnumGroupMessageType Type
        {
            get;
            set;
        }

        [NotMapped]
        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case EnumGroupMessageType.Text:
                        return "文本";
                    case EnumGroupMessageType.Image:
                        return "图片";
                    case EnumGroupMessageType.Article:
                        return "图文";
                    default:
                        return Type.ToString();
                }
            }
        }

        /// <summary>
        /// 文本消息时的文本内容
        /// </summary>
        public string Content
        {
            get;
            set;
        }

        //图片素材时，下面存储其信息

        public string Url
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 素材消息时的media_id
        /// 除图文消息
        /// </summary>
        public string MediaId
        {
            get;
            set;
        }

        /////

        /// <summary>
        /// 图文消息时图文消息的Id
        /// </summary>
        public Guid? ArticleId
        {
            get;
            set;
        }

        /// <summary>
        /// 消息发送任务的ID
        /// </summary>
        public long MsgId
        {
            get;
            set;
        }

        /// <summary>
        /// 消息的数据ID，该字段只有在群发图文消息时，才会出现。
        /// 可以用于在图文分析数据接口中，获取到对应的图文消息的数据，是图文分析数据接口中的msgid字段中的前半部分，
        /// 详见图文分析数据接口中的msgid字段的介绍。
        /// </summary>
        public long MsgDataId
        {
            get;
            set;
        }

        //结果

        /// <summary>
        /// 群发的结构，为“send success”或“send fail”或“err(num)”。但send success时，
        /// 也有可能因用户拒收公众号的消息、系统错误等原因造成少量用户接收失败。err(num)是审核失败的具体原因，
        /// 可能的情况如下：err(10001), //涉嫌广告 err(20001), //涉嫌政治 err(20004), //涉嫌社会 err(20002), 
        /// 涉嫌色情 err(20006), //涉嫌违法犯罪 err(20008), //涉嫌欺诈 err(20013), //涉嫌版权 err(22000), //
        /// 涉嫌互推(互相宣传) err(21000), //涉嫌其他
        /// </summary>
        public string Status
        {
            get;
            set;
        }

        [NotMapped]
        public string StatusString
        {
            get
            {
                if (String.IsNullOrEmpty(Status))
                    return "已提交";

                switch (Status)
                {
                    case "send success":
                        return "成功";
                    case "send fail":
                        return "失败";
                    case "err(10001)":
                        return "涉嫌广告";
                    case "err(20001)":
                        return "涉嫌政治";
                    case "err(20004)":
                        return "涉嫌社会";
                    case "err(20002)":
                        return "涉嫌色情";
                    case "err(20006)":
                        return "涉嫌违法犯罪";
                    case "err(20008)":
                        return "涉嫌欺诈";
                    case "err(20013)":
                        return "涉嫌版权";
                    case "err(22000)":
                        return "涉嫌互推(互相宣传)";
                    case "err(21000)":
                        return "涉嫌其他";
                    default:
                        return Status;
                }
            }
        }

        /// <summary>
        /// group_id下粉丝数；或者openid_list中的粉丝数
        /// </summary>
        public int TotalCount
        {
            get;
            set;
        }

        /// <summary>
        /// 过滤（过滤是指特定地区、性别的过滤、用户设置拒收的过滤，用户接收已超4条的过滤）后，
        /// 准备发送的粉丝数，原则上，FilterCount = SentCount + ErrorCount
        /// </summary>
        public int FilterCount
        {
            get;
            set;
        }

        /// <summary>
        /// 发送成功的粉丝数
        /// </summary>
        public int SentCount
        {
            get;
            set;
        }

        /// <summary>
        /// 发送失败的粉丝数
        /// </summary>
        public int ErrorCount
        {
            get;
            set;
        }

        public DateTime SubmitTime
        {
            get;
            set;
        }
    }
}
