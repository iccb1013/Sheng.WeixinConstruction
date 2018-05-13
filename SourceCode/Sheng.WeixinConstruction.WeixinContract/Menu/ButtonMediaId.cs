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
    [DataContract]
    public class ButtonMediaId : TypeButton
    {
        /// <summary>
        /// 调用新增永久素材接口返回的合法media_id
        /// </summary>
        [DataMember(Name = "media_id")]
        public string MediaId
        {
            get;
            set;
        }

        public ButtonMediaId()
        {
            this.Type = ButtonType.MediaId;
        }
    }
}
