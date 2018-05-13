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


using Linkup.Common;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class GetMessageListArgs : GetItemListArgs
    {
        public EnumReceivingMessageType? ReceivingMessageType
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public string MemberOpenId
        {
            get;
            set;
        }

        public string MemberNickName
        {
            get;
            set;
        }
    }
}
