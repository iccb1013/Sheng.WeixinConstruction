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
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell
{
    public class ErrorMessage
    {
        private static readonly ErrorMessage _instance = new ErrorMessage();
        public static ErrorMessage Instance
        {
            get { return _instance; }
        }

        Dictionary<string, string> _messageList = new Dictionary<string, string>();

        private ErrorMessage()
        {
            _messageList.Add("td1", "网页授权接口回调失败。");
            _messageList.Add("td2", "请求网页 AccessToken 失败。");
            _messageList.Add("td3", "请求用户信息失败。");
            _messageList.Add("td4", "只有微信认证服务号具备网页授权获取用户信息权限。");
            _messageList.Add("td5", "指定的域没有授权公众号信息，或已经取消了公众号对本系统的授权。");
            _messageList.Add("td6", "指定的域不存在。");
            _messageList.Add("td7", "没有指定域。");
            _messageList.Add("td8", "指定的活动不存在，可能已被删除。");
        }

        public string GetMessage(string key)
        {
            if (_messageList.ContainsKey(key) == false)
            {
                return "未知错误";
            }
            else
            {
                return _messageList[key];
            }
        }
    }
}