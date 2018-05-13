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
     * 自定义菜单接口可实现多种类型按钮
     * http://mp.weixin.qq.com/wiki/13/43de8269be54a0a6f64413e4dfa94f39.html
     */
    public enum ButtonType
    {
        /// <summary>
        /// 点击推事件
        /// </summary>
        [EnumMember(Value = "click")]
        Click,
        /// <summary>
        /// 跳转URL
        /// </summary>
        [EnumMember(Value = "view")]
        View,
        /// <summary>
        /// 扫码推事件
        /// </summary>
        [EnumMember(Value = "scancode_push")]
        ScancodePush,
        /// <summary>
        /// 下发消息（除文本消息）
        /// </summary>
        [EnumMember(Value = "media_id")]
        MediaId
    }
}
