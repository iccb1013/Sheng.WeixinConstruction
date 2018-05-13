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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [DataContract]
    [Table("AutoReplyOnKeyWordsRule")]
    public class AutoReplyOnKeyWordsRuleEntity
    {
        private Guid _id = Guid.NewGuid();
        [Key]
        [DataMember]
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

        [DataMember]
        public string RuleName
        {
            get;
            set;
        }

        [NotMapped]
        [DataMember]
        public List<AutoReplyKeyWord> KeywordList
        {
            get;
            set;
        }

        public string Keyword
        {
            get
            {
                if (KeywordList == null || KeywordList.Count == 0)
                    return String.Empty;
                else
                    return JsonConvert.SerializeObject(KeywordList);
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    KeywordList = new List<AutoReplyKeyWord>();
                else
                    KeywordList = JsonConvert.DeserializeObject<List<AutoReplyKeyWord>>(value);
            }
        }

        [DataMember]
        public bool ReplyAll
        {
            get;
            set;
        }

        [DataMember]
        [OrderBy(OrderBy = OrderBy.DESC)]
        public DateTime CreateTime
        {
            get;
            set;
        }

        [NotMapped]
        [DataMember]
        public List<AutoReplyOnKeyWordsContentEntity> ContentList
        {
            get;
            set;
        }

        [NotMapped]
        [DataMember]
        public string ContentDigest
        {
            get
            {
                List<AutoReplyOnKeyWordsContentEntity> contentList = this.ContentList;
                if (contentList != null)
                {
                    string digest = "{0}条（ {1}条文字， {2}条图片， {3}条图文）";
                    return String.Format(digest, contentList.Count,
                        (from c in contentList where c.Type == EnumAutoReplyType.Text select c).Count(),
                        (from c in contentList where c.Type == EnumAutoReplyType.Image select c).Count(),
                        (from c in contentList where c.Type == EnumAutoReplyType.Article select c).Count());
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        /// <summary>
        /// 判断给定的内容是否被命中
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool IsMatch(string content)
        {
            if (String.IsNullOrEmpty(content))
                return false;

            if (KeywordList == null || KeywordList.Count == 0)
                return false;

            //没有任何回复内容直接判断为未命中
            if (ContentList == null || ContentList.Count == 0)
                return false;

            foreach (AutoReplyKeyWord item in KeywordList)
            {
                if (item.WholeMatch)
                {
                    if (content.ToLower() == item.Keyword.ToLower())
                        return true;
                }
                else
                {
                    if (content.ToLower().IndexOf(item.Keyword.ToLower()) >= 0)
                        return true;
                }
            }

            return false;
        }
    }

    [DataContract]
    public class AutoReplyKeyWord
    {
        [DataMember]
        public string Keyword
        {
            get;
            set;
        }

        [DataMember]
        public bool WholeMatch
        {
            get;
            set;
        }
    }
}
