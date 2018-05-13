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
    [Table("ScenicQRCode")]
    public class ScenicQRCodeEntity
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

        public string Name
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public Guid CreateUser
        {
            get;
            set;
        }

        /// <summary>
        /// 获取的二维码ticket，凭借此ticket可以在有效时间内换取二维码。
        /// </summary>
        public string Ticket
        {
            get;
            set;
        }

        /// <summary>
        /// 二维码图片解析后的地址，开发者可根据该地址自行生成需要的二维码图片
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        public string QRCodeImageUrl
        {
            get;
            set;
        }

        public string Remark
        {
            get;
            set;
        }

        public int LandingCount
        {
            get;
            set;
        }

        public int AttentionPersonCount
        {
            get;
            set;
        }
    }
}
