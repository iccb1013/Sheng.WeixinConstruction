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
     * 成功：
     * {
           "errcode":0,
           "errmsg":"send job submission success",
           "msg_id":34182, 
           "msg_data_id": 206227730
        }
     * 失败错误时微信会返回错误码等信息，请根据错误码查询错误信息: 全局返回码说明
     * 
     * http://mp.weixin.qq.com/wiki/15/40b6865b893947b764e2de8e4a1fb55f.html#.E6.A0.B9.E6.8D.AE.E5.88.86.E7.BB.84.E8.BF.9B.E8.A1.8C.E7.BE.A4.E5.8F.91.E3.80.90.E8.AE.A2.E9.98.85.E5.8F.B7.E4.B8.8E.E6.9C.8D.E5.8A.A1.E5.8F.B7.E8.AE.A4.E8.AF.81.E5.90.8E.E5.9D.87.E5.8F.AF.E7.94.A8.E3.80.91
     */
    [DataContract]
    public class WeixinGroupMessageSendAllResult
    {
        [DataMember(Name = "msg_id")]
        public long MsgId
        {
            get;
            set;
        }

        [DataMember(Name = "msg_data_id")]
        public long MsgDataId
        {
            get;
            set;
        }
    }
}
