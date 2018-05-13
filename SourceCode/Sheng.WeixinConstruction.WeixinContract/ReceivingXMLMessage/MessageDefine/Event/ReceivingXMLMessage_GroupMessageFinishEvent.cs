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
using System.Xml.Serialization;

namespace Sheng.WeixinConstruction.WeixinContract
{
    /*
     * http://mp.weixin.qq.com/wiki/15/40b6865b893947b764e2de8e4a1fb55f.html
     * 
        <xml>
        <ToUserName><![CDATA[gh_3e8adccde292]]></ToUserName>
        <FromUserName><![CDATA[oR5Gjjl_eiZoUpGozMo7dbBJ362A]]></FromUserName>
        <CreateTime>1394524295</CreateTime>
        <MsgType><![CDATA[event]]></MsgType>
        <Event><![CDATA[MASSSENDJOBFINISH]]></Event>
        <MsgID>1988</MsgID>
        <Status><![CDATA[sendsuccess]]></Status>
        <TotalCount>100</TotalCount>
        <FilterCount>80</FilterCount>
        <SentCount>75</SentCount>
        <ErrorCount>5</ErrorCount>
        </xml>
     */
    [XmlRootAttribute("xml")]
    public class ReceivingXMLMessage_GroupMessageFinishEvent : ReceivingXMLMessageBase
    {
        /// <summary>
        /// 事件信息，此处为MASSSENDJOBFINISH
        /// </summary>
        [XmlElement("Event")]
        public string Event
        {
            get;
            set;
        }

        [XmlElement("MsgID")]
        public long MsgId
        {
            get;
            set;
        }

        /// <summary>
        /// 群发的结构，为“send success”或“send fail”或“err(num)”。但send success时，
        /// 也有可能因用户拒收公众号的消息、系统错误等原因造成少量用户接收失败。err(num)是审核失败的具体原因，
        /// 可能的情况如下：err(10001), //涉嫌广告 err(20001), //涉嫌政治 err(20004), //涉嫌社会 err(20002), 
        /// 涉嫌色情 err(20006), //涉嫌违法犯罪 err(20008), //涉嫌欺诈 err(20013), //涉嫌版权 err(22000), //
        /// 涉嫌互推(互相宣传) err(21000), //涉嫌其他
        /// </summary>
        [XmlElement("Status")]
        public string Status
        {
            get;
            set;
        }

        /// <summary>
        /// group_id下粉丝数；或者openid_list中的粉丝数
        /// </summary>
        [XmlElement("TotalCount")]
        public int TotalCount
        {
            get;
            set;
        }

        /// <summary>
        /// 过滤（过滤是指特定地区、性别的过滤、用户设置拒收的过滤，用户接收已超4条的过滤）后，
        /// 准备发送的粉丝数，原则上，FilterCount = SentCount + ErrorCount
        /// </summary>
        [XmlElement("FilterCount")]
        public int FilterCount
        {
            get;
            set;
        }

        /// <summary>
        /// 发送成功的粉丝数
        /// </summary>
        [XmlElement("SentCount")]
        public int SentCount
        {
            get;
            set;
        }

        /// <summary>
        /// 发送失败的粉丝数
        /// </summary>
        [XmlElement("ErrorCount")]
        public int ErrorCount
        {
            get;
            set;
        }
    }
}
