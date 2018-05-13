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

namespace Sheng.WeixinConstruction.WeixinContract.PayApi
{
    [DataContract]
    public class WeixinPayGetBrandWCPayRequestArgs
    {
        /// <summary>
        /// 公众号名称，由商户传入     
        /// </summary>
        [DataMember(Name = "appId")]
        public string AppId
        {
            get;
            set;
        }

        /// <summary>
        /// 时间戳，自1970年以来的秒数     
        /// </summary>
        [DataMember(Name = "timeStamp")]
        public string TimeStamp
        {
            get;
            set;
        }

        /// <summary>
        /// 随机串     
        /// </summary>
        [DataMember(Name = "nonceStr")]
        public string NonceStr
        {
            get;
            set;
        }

        /// <summary>
        /// prepay_id=u802345jgfjsdfgsdg888
        /// </summary>
        [DataMember(Name = "package")]
        public string Package
        {
            get;
            set;
        }

        /// <summary>
        /// 微信签名方式：     MD5
        /// </summary>
        [DataMember(Name = "signType")]
        public string SignType
        {
            get;
            set;
        }

        /// <summary>
        /// 微信签名 
        /// </summary>
        [DataMember(Name = "paySign")]
        public string PaySign
        {
            get;
            set;
        }
    }
}
