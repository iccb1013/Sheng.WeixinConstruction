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

namespace Sheng.WeixinConstruction.WeixinContract.PayApi
{
    public class WxPayArgs
    {
        private bool _useCertificate = true;
        public bool UseCertificate
        {
            get { return _useCertificate; }
            set { _useCertificate = value; }
        }

        /// <summary>
        /// 一个证书文件的名称，物理路径
        ///  @"D:\wwwroot\WeixinConstruction\cert\apiclient_cert.p12";
        /// </summary>
        public string CertificatePath
        {
            get;
            set;
        }

        /// <summary>
        /// 证书密码
        /// </summary>
        public string CertificatePassword
        {
            get;
            set;
        }

        /// <summary>
        /// 商户支付密钥，参考开户邮件设置（必须配置）
        /// </summary>
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// 公众帐号secert（仅JSAPI支付的时候需要配置）
        /// </summary>
        public string AppSecret
        {
            get;
            set;
        }
    }
}
