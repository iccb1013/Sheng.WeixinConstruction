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
     * 
     * 傻逼微信同时使用了XML和JSON
     * 
     * 此处为客服接口发送消息的JSON定义
     * 
     * XML：
     * http://mp.weixin.qq.com/wiki/18/c66a9f0b5aa952346e46dc39de20f672.html#.E5.9B.9E.E5.A4.8D.E5.9B.BE.E7.89.87.E6.B6.88.E6.81.AF
     * JSON:
     * http://mp.weixin.qq.com/wiki/14/d9be34fe03412c92517da10a5980e7ee.html#.E6.8E.A5.E5.8F.A3.E7.9A.84.E7.BB.9F.E4.B8.80.E5.8F.82.E6.95.B0.E8.AF.B4.E6.98.8E
    */

    [DataContract]
    public class KFMessageBase
    {
        [DataMember(Name = "touser")]
        public string ToUserName
        {
            get;
            set;
        }

        [DataMember(Name = "msgtype")]
        public string MsgType
        {
            get;
            set;
        }
    }
}
