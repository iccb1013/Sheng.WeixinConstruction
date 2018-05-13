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
    [Table("Campaign_PictureVoteLog")]
    public class Campaign_PictureVoteLogEntity
    {
        public Guid CampaignId
        {
            get;
            set;
        }

        public Guid Domain
        {
            get;
            set;
        }

        public Guid ItemId
        {
            get;
            set;
        }

        public string OpenId
        {
            get;
            set;
        }

        public Guid? Member
        {
            get;
            set;
        }

        /// <summary>
        /// 投票时间
        /// </summary>
        public DateTime VoteTime
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
        ///// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        ///// </summary>
        //public int Sex
        //{
        //    get;
        //    set;
        //}

        //[NotMapped]
        //public string SexString
        //{
        //    get
        //    {
        //        if (Sex == 1)
        //            return "男";
        //        else if (Sex == 2)
        //            return "女";
        //        else
        //            return "未知";
        //    }
        //}

        //public string City
        //{
        //    get;
        //    set;
        //}

        //public string Country
        //{
        //    get;
        //    set;
        //}

        //public string Province
        //{
        //    get;
        //    set;
        //}

        //public string Language
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// http://wx.qlogo.cn/mmopen/VNMic85jx3X55pdysRvSsic17icBaKz49u3ibnoPqEYbficgfB3x0oKdlJxlEH1ZXgXvvOB0TDTcpCxt7drnJOUUX4g/0
        ///// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。
        ///// </summary>
        //public string Headimgurl
        //{
        //    get;
        //    set;
        //}

        //[NotMapped]
        //public string Headimgurl_64
        //{
        //    get
        //    {
        //        if (String.IsNullOrEmpty(Headimgurl))
        //            return String.Empty;

        //        string headImg64 = Headimgurl.TrimEnd('0');
        //        return headImg64 + "64";
        //    }
        //}
    }
}
