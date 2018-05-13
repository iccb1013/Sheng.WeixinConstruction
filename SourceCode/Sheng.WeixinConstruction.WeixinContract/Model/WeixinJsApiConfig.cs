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
    [DataContract]
    public class WeixinJsApiConfig
    {
        /// <summary>
        /// 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，
        /// 可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
        /// </summary>
        [DataMember(Name = "debug")]
        public bool Debug
        {
            get;
            set;
        }

        /// <summary>
        /// 公众号的唯一标识
        /// </summary>
        [DataMember(Name = "appId")]
        public string AppId
        {
            get;
            set;
        }

        /// <summary>
        /// 生成签名的时间戳
        /// </summary>
        [DataMember(Name = "timestamp")]
        public long Timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// 生成签名的随机串
        /// </summary>
        [DataMember(Name = "nonceStr")]
        public string NonceStr
        {
            get;
            set;
        }

        /// <summary>
        /// 签名，见附录1
        /// </summary>
        [DataMember(Name = "signature")]
        public string Signature
        {
            get;
            set;
        }

        private List<string> _jsApiList = new List<string>();
        /// <summary>
        /// 需要使用的JS接口列表，所有JS接口列表见附录2
        /// </summary>
        [DataMember(Name = "jsApiList")]
        public List<string> JsApiList
        {
            get { return _jsApiList; }
            set { _jsApiList = value; }
        }
    }
}
