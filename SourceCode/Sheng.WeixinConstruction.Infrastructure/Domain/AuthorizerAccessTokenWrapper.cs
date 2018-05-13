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
    /// 公众号accessToken封装
    /// </summary>
    public class AuthorizerAccessTokenWrapper
    {
        public string AppId
        {
            get;
            set;
        }

        public string AccessToken
        {
            get;
            set;
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken
        {
            get;
            set;
        }

        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime AccessTokenExpiryTime
        {
            get;
            set;
        }

        [NotMapped]
        /// <summary>
        /// 快要到期了
        /// </summary>
        public bool WillbeTimeout
        {
            get
            {
                //小于30分钟即准备刷新
                if ((AccessTokenExpiryTime - DateTime.Now).TotalSeconds <= 1800)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
