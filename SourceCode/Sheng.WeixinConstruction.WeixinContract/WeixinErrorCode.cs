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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sheng.WeixinConstruction.WeixinContract
{
    public class WeixinErrorCode
    {
        private static readonly WeixinErrorCode _instance = new WeixinErrorCode();
        public static WeixinErrorCode Instance
        {
            get { return _instance; }
        }

        Dictionary<int, string> _messageList = new Dictionary<int, string>();

        private WeixinErrorCode()
        {
            //Windows服务中取不到 AppDomain.CurrentDomain.RelativeSearchPath
            //要用 AppDomain.CurrentDomain.BaseDirectory 取
            string dir = AppDomain.CurrentDomain.RelativeSearchPath;
            if (String.IsNullOrEmpty(dir))
            {
                dir = AppDomain.CurrentDomain.BaseDirectory;
            }

            Debug.Assert(String.IsNullOrEmpty(dir) == false, "没有取到 ErrorCode.xml 所在目录。");

            XmlDocument xml = new XmlDocument();
            xml.Load(Path.Combine(dir, "ErrorCode.xml"));
            XmlNodeList itemList = xml.GetElementsByTagName("item");
            foreach (XmlElement item in itemList)
            {
                _messageList.Add(int.Parse(item.Attributes["code"].Value), item.InnerText);
            }
        }

        public string GetMessage(int code)
        {
            if (_messageList.ContainsKey(code) == false)
            {
                return null;
            }
            else
            {
                return _messageList[code];
            }
        }
    }
}
