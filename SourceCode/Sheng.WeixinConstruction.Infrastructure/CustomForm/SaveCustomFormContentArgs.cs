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
    public class SaveCustomFormContentArgs
    {
        /// <summary>
        /// 必须要求已关注或者能够获取到他的个人信息，因为基本联系信息是保存在Member表中的
        /// </summary>
        public Guid MemberId
        {
            get;
            set;
        }

        public Guid FormId
        {
            get;
            set;
        }

        public Guid? ContentId
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
    }
}
