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
    [Table("CustomForm")]
    public class CustomFormEntity
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

        public string Introduction
        {
            get;
            set;
        }

        public string SuccessfulDescription
        {
            get;
            set;
        }

        public string ShareImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 分享到朋友圈标题
        /// </summary>
        public string ShareTimelineTitle
        {
            get;
            set;
        }

        /// <summary>
        /// 分享给好友标题
        /// </summary>
        public string ShareAppMessageTitle
        {
            get;
            set;
        }

        /// <summary>
        /// 分享给好友描述
        /// </summary>
        public string ShareAppMessageDescription
        {
            get;
            set;
        }

        public string ImageUrl
        {
            get;
            set;
        }

        private bool _onlyMember = true;
        public bool OnlyMember
        {
            get { return _onlyMember; }
            set { _onlyMember = value; }
        }

        public string GuideSubscribeUrl
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

        public string Remark
        {
            get;
            set;
        }

        private bool _fieldName = true;
        public bool FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        private bool _fieldNameRequired = true;
        public bool FieldNameRequired
        {
            get { return _fieldNameRequired; }
            set { _fieldNameRequired = value; }
        }

        private bool _fieldBirthday = false;
        public bool FieldBirthday
        {
            get { return _fieldBirthday; }
            set { _fieldBirthday = value; }
        }

        private bool _fieldBirthdayRequired = false;
        public bool FieldBirthdayRequired
        {
            get { return _fieldBirthdayRequired; }
            set { _fieldBirthdayRequired = value; }
        }

        private bool _fieldMobilePhone = true;
        public bool FieldMobilePhone
        {
            get { return _fieldMobilePhone; }
            set { _fieldMobilePhone = value; }
        }

        private bool _fieldMobilePhoneRequired = true;
        public bool FieldMobilePhoneRequired
        {
            get { return _fieldMobilePhoneRequired; }
            set { _fieldMobilePhoneRequired = value; }
        }

        private bool _fieldEmail = false;
        public bool FieldEmail
        {
            get { return _fieldEmail; }
            set { _fieldEmail = value; }
        }

        private bool _fieldEmailRequired = false;
        public bool FieldEmailRequired
        {
            get { return _fieldEmailRequired; }
            set { _fieldEmailRequired = value; }
        }

        public int PageVisitCount
        {
            get;
            set;
        }

        public int FillinCount
        {
            get;
            set;
        }

        private bool _available = true;
        /// <summary>
        /// 是否开放填写
        /// </summary>
        public bool Available
        {
            get { return _available; }
            set { _available = value; }
        }
    }
}
