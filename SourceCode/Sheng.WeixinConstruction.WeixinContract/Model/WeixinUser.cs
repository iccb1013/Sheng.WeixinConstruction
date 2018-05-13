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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    /*
     {
        "subscribe": 1, 
        "openid": "o6_bmjrPTlm6_2sgVt7hMZOPfL2M", 
        "nickname": "Band", 
        "sex": 1, 
        "language": "zh_CN", 
        "city": "广州", 
        "province": "广东", 
        "country": "中国", 
        "headimgurl":    "http://wx.qlogo.cn/mmopen/g3MonUZtNHkdmzicIlibx6iaFqAc56vxLSUfpb6n5WKSYVY0ChQKkiaJSgQ1dZuTOgvLLrhJbERQQ4eMsv84eavHiaiceqxibJxCfHe/0", 
       "subscribe_time": 1382694957,
       "unionid": " o6_bmasdasdsad6_2sgVt7hMZOPfL"
     * 
        subscribe	用户是否订阅该公众号标识，值为0时，代表此用户没有关注该公众号，拉取不到其余信息。
        openid	用户的标识，对当前公众号唯一
        nickname	用户的昵称
        sex	用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        city	用户所在城市
        country	用户所在国家
        province	用户所在省份
        language	用户的语言，简体中文为zh_CN
        headimgurl	用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空
        subscribe_time	用户关注时间，为时间戳。如果用户曾多次关注，则取最后关注时间
        unionid	只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段。详见：获取用户个人信息（UnionID机制）
     * 
    } 
     */

    [DataContract]
    public class WeixinUser
    {
        /// <summary>
        /// 用户是否订阅该公众号标识，值为0时，代表此用户没有关注该公众号，拉取不到其余信息。
        /// </summary>
        [DataMember(Name = "subscribe")]
        public int Subscribe
        {
            get;
            set;
        }

        [DataMember(Name = "openid")]
        public string OpenId
        {
            get;
            set;
        }

        [DataMember(Name = "nickname")]
        public string Nickname
        {
            get;
            set;
        }

        /// <summary>
        /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        [DataMember(Name = "sex")]
        public int Sex
        {
            get;
            set;
        }

        [DataMember(Name = "language")]
        public string Language
        {
            get;
            set;
        }

        [DataMember(Name = "city")]
        public string City
        {
            get;
            set;
        }

        [DataMember(Name = "province")]
        public string Province
        {
            get;
            set;
        }

        [DataMember(Name = "country")]
        public string Country
        {
            get;
            set;
        }

        [DataMember(Name = "headimgurl")]
        public string Headimgurl
        {
            get;
            set;
        }

        [DataMember(Name = "subscribe_time")]
        public int Subscribe_time
        {
            get;
            set;
        }

        public DateTime SubscribeTime
        {
            get
            {
                return WeixinApiHelper.ConvertIntToDateTime(Subscribe_time);
            }
        }

        [DataMember(Name = "unionid")]
        public string Unionid
        {
            get;
            set;
        }

        [DataMember(Name = "remark")]
        public string Remark
        {
            get;
            set;
        }

        [DataMember(Name = "groupid")]
        public int GroupId
        {
            get;
            set;
        }

        [DataMember(Name = "tagid_list")]
        public int[] TagList
        {
            get;
            set;
        }

        /// <summary>
        /// 返回 1,2,3,
        /// 最后一位还是要多带一个分隔符
        /// 否则SQL操作很麻烦
        /// 如果不带，那 REPLACE([TagList],'5,','')
        /// 这里的 5 是带还是不带分割符号就要特别处理
        /// </summary>
        /// <returns></returns>
        public string GetTagListString()
        {
            if (TagList.Length == 0)
                return String.Empty;

            StringBuilder str = new StringBuilder(TagList.Length * 2);
            for (int i = 0; i < TagList.Length; i++)
            {
                str.Append(TagList[i]);
                str.Append(",");
            }

            return str.ToString();
        }

    }
}
