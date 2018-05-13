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
     * 当RecommendUrl被访问时，取到访问者OpenId
     * 记录到表中
     * 如果有同样的OpenId存在，也直接Insert（更高性能）
     */

    [Table("RecommendUrlLog")]
    public class RecommendUrlLogEntity
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

        public Guid UrlOwnMember
        {
            get;
            set;
        }

        public string VisitOpenId
        {
            get;
            set;
        }

        public DateTime Time
        {
            get;
            set;
        }
    }
}
