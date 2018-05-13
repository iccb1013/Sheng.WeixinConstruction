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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class WxConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// 作为第三方平台运行时的参数配置
        /// </summary>
        [ConfigurationProperty("thirdParty", IsRequired = false)]
        public ThirdPartyConfigurationElement ThirdParty
        {
            get { return base["thirdParty"] as ThirdPartyConfigurationElement; }
            set { base["thirdParty"] = value; }
        }

        /// <summary>
        /// 作为第三方平台运行时的参数配置
        /// </summary>
        [ConfigurationProperty("aliyun", IsRequired = false)]
        public AliyunConfigurationElement Aliyun
        {
            get { return base["aliyun"] as AliyunConfigurationElement; }
            set { base["aliyun"] = value; }
        }
    }

    #region ThirdParty

    /// <summary>
    /// 系统拓扑结构设置
    /// </summary>
    public class ThirdPartyConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("appId", IsRequired = true)]
        public string AppId
        {
            get { return base["appId"].ToString(); }
            set { base["appId"] = value; }
        }

        [ConfigurationProperty("appSecret", IsRequired = true)]
        public string AppSecret
        {
            get { return base["appSecret"].ToString(); }
            set { base["appSecret"] = value; }
        }

        [ConfigurationProperty("token", IsRequired = true)]
        public string Token
        {
            get { return base["token"].ToString(); }
            set { base["token"] = value; }
        }

        [ConfigurationProperty("encodingAESKey", IsRequired = true)]
        public string EncodingAESKey
        {
            get { return base["encodingAESKey"].ToString(); }
            set { base["encodingAESKey"] = value; }
        }
    }

    #endregion

    #region Aliyun

    public class AliyunConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("regionId", IsRequired = true)]
        public string RegionId
        {
            get { return base["regionId"].ToString(); }
            set { base["regionId"] = value; }
        }

        [ConfigurationProperty("accessKeyId", IsRequired = true)]
        public string AccessKeyId
        {
            get { return base["accessKeyId"].ToString(); }
            set { base["accessKeyId"] = value; }
        }

        [ConfigurationProperty("secret", IsRequired = true)]
        public string Secret
        {
            get { return base["secret"].ToString(); }
            set { base["secret"] = value; }
        }

    }

    #endregion

}
