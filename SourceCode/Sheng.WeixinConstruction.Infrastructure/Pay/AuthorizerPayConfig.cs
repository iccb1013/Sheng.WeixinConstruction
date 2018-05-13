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
using Sheng.WeixinConstruction.WeixinContract.PayApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("AuthorizerPayConfig")]
    public class AuthorizerPayConfig
    {
        /// <summary>
        /// 一个域下面有可能存在多个 授权过 的公众 号
        /// 但只允许一个是 当前 正在运营的
        /// </summary>
        [Key]
        public Guid Domain
        {
            get;
            set;
        }

        /// <summary>
        /// 授权方appid
        /// </summary>
        [Key]
        public string AppId
        {
            get;
            set;
        }

        /// <summary>
        /// 微信支付分配的商户号
        /// mch_id
        /// </summary>
        public string MchId
        {
            get;
            set;
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

        public WxPayArgs GetWxPayArgs(bool useCertificate)
        {
            WxPayArgs args = new WxPayArgs();
            args.UseCertificate = useCertificate;
            if (useCertificate)
            {
                args.CertificatePath = CertificatePath;
                args.CertificatePassword = CertificatePassword;
            }
            args.Key = Key;
            args.AppSecret = AppSecret;
            return args;
        }
    }
}
