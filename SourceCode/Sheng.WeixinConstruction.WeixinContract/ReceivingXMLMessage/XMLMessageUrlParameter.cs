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

namespace Sheng.WeixinConstruction.WeixinContract
{
    /// <summary>
    /// 微信服务器将发送GET请求到填写的服务器地址URL上，GET请求携带四个参数：
    /// http://mp.weixin.qq.com/wiki/8/f9a0b8382e0b77d87b3bcc1ce6fbc104.html
    /// </summary>
    public class XMLMessageUrlParameter
    {
        /// <summary>
        /// 微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。
        /// </summary>
        public string Signature
        {
            get;
            set;
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// 随机数
        /// </summary>
        public string Nonce
        {
            get;
            set;
        }

        /// <summary>
        /// 加密类型，为aes
        /// </summary>
        public string Encrypt_type
        {
            get;
            set;
        }

        /// <summary>
        /// 消息体签名，用于验证消息体的正确性
        /// </summary>
        public string Msg_signature
        {
            get;
            set;
        }
    }
}
