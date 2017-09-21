using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class PictureVoteViewModel
    {
        public Campaign_PictureVoteBundle CampaignBundle
        {
            get;
            set;
        }

        public Campaign_PictureVoteDataReport DataReport
        {
            get;
            set;
        }

        /// <summary>
        /// 当前用户上传的项目（如果有）
        /// </summary>
        public Campaign_PictureVoteItemEntity PictureVoteItem
        {
            get;
            set;
        }

        public WeixinJsApiConfig JsApiConfig
        {
            get;
            set;
        }

        public bool Attention
        {
            get;
            set;
        }

        /// <summary>
        /// 是否已达最大允许参与的人数
        /// </summary>
        public bool FullParticipant
        {
            get;
            set;
        }
    }
}