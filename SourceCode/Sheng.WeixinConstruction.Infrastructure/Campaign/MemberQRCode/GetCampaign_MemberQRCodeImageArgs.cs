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

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetCampaign_MemberQRCodeImageArgs
    {
        public string LandingUrl
        {
            get;
            set;
        }

        //public string NickName
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 必须是本地绝对路径
        ///// </summary>
        //public string HeadImage
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 必须是本地绝对路径
        ///// </summary>
        //public string BackgroundImage
        //{
        //    get;
        //    set;
        //}

        public Guid Domain
        {
            get;
            set;
        }

        /// <summary>
        /// 获取会员头像和昵称
        /// </summary>
        public Guid MemberId
        {
            get;
            set;
        }

        /// <summary>
        /// 文件服务器记录Id
        /// </summary>
        public Guid BackgroundImageId
        {
            get;
            set;
        }
    }

    public class GetMemberQRCodeImageResult
    {
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 文件服务器上的相对路径
        /// </summary>
        public string FileName
        {
            get;
            set;
        }
    }
}
