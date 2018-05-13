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
    [Table("Member")]
    public class MemberEntity
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

        /// <summary>
        /// 所属公众号AppId
        /// </summary>
        public string AppId
        {
            get;
            set;
        }

        #region 微信字段

        public string OpenId
        {
            get;
            set;
        }

        public string NickName
        {
            get;
            set;
        }

        /// <summary>
        /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        public int Sex
        {
            get;
            set;
        }

        [NotMapped]
        public string SexString
        {
            get
            {
                if (Sex == 1)
                    return "男";
                else if (Sex == 2)
                    return "女";
                else
                    return "未知";
            }
        }

        public string City
        {
            get;
            set;
        }

        public string Country
        {
            get;
            set;
        }

        public string Province
        {
            get;
            set;
        }

        public string Language
        {
            get;
            set;
        }

        /// <summary>
        /// http://wx.qlogo.cn/mmopen/VNMic85jx3X55pdysRvSsic17icBaKz49u3ibnoPqEYbficgfB3x0oKdlJxlEH1ZXgXvvOB0TDTcpCxt7drnJOUUX4g/0
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。
        /// </summary>
        public string Headimgurl
        {
            get;
            set;
        }

        [NotMapped]
        public string Headimgurl_64
        {
            get
            {
                if (String.IsNullOrEmpty(Headimgurl))
                    return String.Empty;

                string headImg64 = Headimgurl.TrimEnd('0');
                return headImg64 + "64";
            }
        }

        [NotMapped]
        public string Headimgurl_96
        {
            get
            {
                if (String.IsNullOrEmpty(Headimgurl))
                    return String.Empty;

                string headImg96 = Headimgurl.TrimEnd('0');
                return headImg96 + "96";
            }
        }

        [NotMapped]
        public string Headimgurl_132
        {
            get
            {
                if (String.IsNullOrEmpty(Headimgurl))
                    return String.Empty;

                string headImg132 = Headimgurl.TrimEnd('0');
                return headImg132 + "132";
            }
        }

        public DateTime SubscribeTime
        {
            get;
            set;
        }

        public string Remark
        {
            get;
            set;
        }

        /// <summary>
        /// 用户所在的分组ID（兼容旧的用户分组接口）
        /// 以前是只有分组功能，后来微信推出了Tag功能，分组就被TAG取代了
        /// </summary>
        public int GroupId
        {
            get;
            set;
        }

        /// <summary>
        /// 用户被打上的标签ID列表
        /// 128,2
        /// </summary>
        public string TagList
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 会员卡卡号
        /// </summary>
        public string CardNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 会员卡级别，留空则表示是默认会员卡
        /// </summary>
        public Guid? CardLevel
        {
            get;
            set;
        }

        /// <summary>
        /// 用户是通过哪个场景二维码关注的
        /// </summary>
        public Guid? ScenicQRCodeId
        {
            get;
            set;
        }

        /// <summary>
        /// 如果是通过RecomendUrl关注的
        /// 这里存储上级会员Id
        /// </summary>
        public Guid? RefereeMemberId
        {
            get;
            set;
        }

        private bool _attention = true;
        public bool Attention
        {
            get { return _attention; }
            set { _attention = value; }
        }

        /// <summary>
        /// 积分
        /// </summary>
        public int Point
        {
            get;
            set;
        }

        /// <summary>
        /// 账户现金余额
        /// </summary>
        public int CashAccount
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public DateTime? Birthday
        {
            get;
            set;
        }

        public string MobilePhone
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// 会员中心二维码
        /// 管理员扫码后跳转到会员管理画面
        /// </summary>
        public string MemberCenterQRCodeImageUrl
        {
            get;
            set;
        }

        public DateTime? SignInDate
        {
            get;
            set;
        }

        ///// <summary>
        ///// 是否是管理人员
        ///// </summary>
        //public bool Staff
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 同步标记，用于在同步后反向筛选出已经取消关注的用户
        /// </summary>
        public DateTime? SyncFlag
        {
            get;
            set;
        }
    }
}
