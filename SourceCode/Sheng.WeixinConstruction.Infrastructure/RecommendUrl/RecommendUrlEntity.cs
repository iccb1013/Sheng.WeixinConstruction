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
    /*
     * 会员生成一个URL，推荐给好友，好友关注后统计此URL引流的人数
     * 把会员ID放在URL后面，访问链接时，先取到访问者的 OpenId，记录到表中
     * 当接收到关注消息时，根据 OpenId 到记录表中查找，如果找到了对应的记录
     * URL中的参数用RecommendUrl的Id
     * 则给URL所属的会员增加统计计数
     * URL：appid.wxc.shengxunwei.com/Function/RecommendUrl/{domainId}?id={id}
     * 访问此URL时，打开一个指定的落地页或是二维码关注页面
     */

    [Table("RecommendUrl")]
    public class RecommendUrlEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Guid Domain
        {
            get;
            set;
        }

        public string AppId
        {
            get;
            set;
        }

        public Guid Member
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        public string ShortUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 访问人次
        /// </summary>
        public int LandingCount
        {
            get;
            set;
        }

        //引流关注人数直接到Member表中根据RefereeMemberId字段查找
        //如果在这里用一个字段单独记录，不但要在关注的地方递增，还需要在取消关注的地方做递减
        //比较麻烦
        //public int AttentionCount
        //{
        //    get;
        //    set;
        //}


    }
}
