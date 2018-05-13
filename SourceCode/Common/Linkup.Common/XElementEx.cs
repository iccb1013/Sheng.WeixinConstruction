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
using System.Xml.Linq;
using System.Xml.XPath;
using System.Diagnostics;

namespace Linkup.Common
{
    /// <summary>
    /// 继承自XmlDocument
    /// 为添加节点，获取节点提供更方便的方法
    /// 
    /// 注意：XElement在使用XPath查询时，不需要加根路径
    /// 这一点不同于 XmlDocument
    /// </summary>
    public class XElementEx : XElement
    {
        #region 构造

        /// <summary>
        /// 从其他 System.Xml.Linq.XElement 对象初始化 System.Xml.Linq.XElement 类的新实例。
        /// </summary>
        /// <param name="other"></param>
        public XElementEx(XElement other)
            : base(other)
        {
        }

        /// <summary>
        /// 用指定的名称初始化 System.Xml.Linq.XElement 类的新实例。
        /// </summary>
        /// <param name="name"></param>
        public XElementEx(XName name)
            : base(name)
        {
        }

        /// <summary>
        /// 从 System.Xml.Linq.XStreamingElement 对象初始化 System.Xml.Linq.XElement 类的新实例。
        /// </summary>
        /// <param name="other"></param>
        public XElementEx(XStreamingElement other)
            : base(other)
        {
        }

        /// <summary>
        /// 用指定的名称和内容初始化 System.Xml.Linq.XElement 类的新实例。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public XElementEx(XName name, object content)
            : base(name, content)
        {
        }

        /// <summary>
        /// 用指定的名称和内容初始化 System.Xml.Linq.XElement 类的新实例。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public XElementEx(XName name, params object[] content)
            : base(name, content)
        {
        }

        #endregion

        #region AppendChild

        /// <summary>
        /// 添加子节点 指定子节点的名称
        /// </summary>
        /// <param name="name"></param>
        public void AppendChild(string name)
        {
            AppendChild(String.Empty, name, null);
        }

        /// <summary>
        /// 添加子节点 指定 xpath 和子节点的名称
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="name"></param>
        public void AppendChild(string xPath, string name)
        {
            AppendChild(xPath, name, null);
        }

        /// <summary>
        /// 添加子节点 指定 xpath 和子节点的名称 和其值
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="name"></param>
        /// <param name="innerText"></param>
        public void AppendChild(string xPath, string name, object innerText)
        {
            XElement xmlEle = new XElement(name);

            if (innerText != null)
                xmlEle.Value = innerText.ToString();

            //如果没有指定xpath查询,则为this创建根节点
            if (String.IsNullOrEmpty(xPath) == false)
            {
                XElement targetElement = this.XPathSelectElement(xPath);
                if (targetElement != null)
                {
                    targetElement.Add(xmlEle);
                }
                else
                {
                    Debug.Assert(false, "没有找到指定的节点", xPath);
                }
            }
            else
            {
                this.Add(xmlEle);
            }
        }

        public void AppendChild(string xPath, XElement element)
        {
            Debug.Assert(element != null, "element 为 null", xPath);

            //如果没有指定xpath查询,则为this创建根节点
            if (String.IsNullOrEmpty(xPath) == false)
            {
                XElement targetElement = this.XPathSelectElement(xPath);
                if (targetElement != null)
                {
                    targetElement.Add(element);
                }
                else
                {
                    Debug.Assert(false, "没有找到指定的节点", xPath);
                }
            }
            else
            {
                this.Add(element);
            }
        }

        #endregion

        #region AppendInnerXml

        public void AppendInnerXml(string innerXml)
        {
            this.AppendInnerXml("/", innerXml);
        }

        public void AppendInnerXml(string xPath, string innerXml)
        {
            Debug.Assert(String.IsNullOrEmpty(innerXml) == false, "innerXml 为空");

            //如果没有指定xpath查询,则为this创建根节点
            if (String.IsNullOrEmpty(xPath) == false)
            {
                XElement targetElement = this.XPathSelectElement(xPath);
                if (targetElement != null)
                {
                    //innerXml 有可能不止一个根 ，如 <Top/><Left/> 这样的文本
                    //XElement.Parse(innerXml) 中的 innerXml 必须且只能有一个根
                    //所以这里为 innerXml 添加一个 temp 的根，再迭代其中的子元素
                    XElement xTemp = XElement.Parse("<Temp>" + innerXml + "</Temp>");
                    foreach (var item in xTemp.Elements())
                    {
                        targetElement.Add(item);
                    }
                }
                else
                {
                    Debug.Assert(false, "没有找到指定的节点", xPath);
                }
            }
            else
            {
                this.AppendInnerXml(innerXml);
            }
        }

        #endregion

        #region AppendAttribute

        public void AppendAttribute(string xPath, string name)
        {
            AppendAttribute(xPath, name, null);
        }

        public void AppendAttribute(string xPath, string name, object value)
        {
            Debug.Assert(String.IsNullOrEmpty(name) == false, "name 为空");

            if (String.IsNullOrEmpty(xPath))
                xPath = "/";

            XElement element = this.SelectSingleNode(xPath);

            if (element == null)
            {
                Debug.Assert(false, "没有找到指定的节点", xPath);
                return;
            }

            if (value == null)
                value = String.Empty;

            element.Add(new XAttribute(name, value));
        }

        #endregion

        /// <summary>
        /// 是否存在指定的节点
        /// </summary>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public bool Contains(string xPath)
        {
            XElement element;//= this.XPathSelectElement(xPath);

            if (String.IsNullOrEmpty(xPath))
            {
                return true;
            }
            else
            {
                element = this.XPathSelectElement(xPath);
            }

            return element != null;
        }

        #region GetInnerObject

        public T GetInnerObject<T>(string xPath, T defaultValue)
        {
            XElement element ;//= this.XPathSelectElement(xPath);

            if (String.IsNullOrEmpty(xPath))
            {
                element = this;
            }
            else
            {
                element = this.XPathSelectElement(xPath);
            }

            if (element != null)
                return (T)Convert.ChangeType(element.Value, defaultValue.GetType());
            else
            {
                Debug.Assert(false, "没有找到指定的节点", xPath);
                return (T)Convert.ChangeType(defaultValue, defaultValue.GetType());
            }
        }

        public string GetInnerObject(string xPath)
        {
            return GetInnerObject(xPath, String.Empty);
        }

        #endregion

        #region GetAttributeObject

        public T GetAttributeObject<T>(string xPath, string name, T defaultValue)
        {
            XAttribute attr;

            if (String.IsNullOrEmpty(xPath))
            {
                attr = this.Attribute(name);
            }
            else
            {
                attr = this.XPathSelectElement(xPath).Attribute(name);
            }

            if (attr != null)
                return (T)Convert.ChangeType(attr.Value, defaultValue.GetType());
            else
            {
                Debug.Assert(false, "没有找到指定的节点", xPath);
                return (T)Convert.ChangeType(defaultValue, defaultValue.GetType());
            }
        }


        public T GetAttributeObject<T>(string name, T defaultValue)
        {
            return GetAttributeObject<T>(null, name, defaultValue);
        }

        public string GetAttributeObject(string xPath, string name)
        {
            return GetAttributeObject<string>(xPath, name, String.Empty);
        }

        /// <summary>
        /// 从根节点获取指定的属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetAttributeObject(string name)
        {
            return GetAttributeObject("/", name);
        }

        #endregion

        #region GetOuterXml

        public string GetOuterXml(string xPath)
        {
            Debug.Assert(String.IsNullOrEmpty(xPath) == false, "xPath 为空");

            XElement xmlNode = this.XPathSelectElement(xPath);

            if (xmlNode != null)
                return xmlNode.ToString();
            else
            {
                Debug.Assert(false, "没有找到指定的节点", xPath);
                return null;
            }
        }

        #endregion

        #region SetInnerText

        public void SetInnerText(string xPath, object value)
        {
            Debug.Assert(String.IsNullOrEmpty(xPath) == false, "xPath 为空");

            XElement xmlNode = this.XPathSelectElement(xPath);

            if (xmlNode == null)
            {
                Debug.Assert(false, "没有找到指定的节点", xPath);
                return;
            }
            if (value == null)
                xmlNode.Value = String.Empty;
            else
                xmlNode.Value = value.ToString();
        }

        #endregion

        /// <summary>
        /// 判断指定的节点下是否有子节点
        /// </summary>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public bool HasElementsByPath(string xPath)
        {
            Debug.Assert(String.IsNullOrEmpty(xPath) == false, "xPath 为空");

            XElement xmlNode = this.XPathSelectElement(xPath);

            Debug.Assert(xmlNode != null, "xmlNode 为空");

            if (xmlNode == null)
                return false;
            else
                return xmlNode.HasElements;
        }

        public XElement SelectSingleNode(string xPath)
        {
            Debug.Assert(String.IsNullOrEmpty(xPath) == false, "xPath 为空");

            return this.XPathSelectElement(xPath);
        }

        public IEnumerable<XElement> SelectNodes(string xPath)
        {
            Debug.Assert(String.IsNullOrEmpty(xPath) == false, "xPath 为空");

            return this.XPathSelectElements(xPath);
        }

        public static new XElementEx Parse(string xml)
        {
            return new XElementEx(XElement.Parse(xml));
        }

        //错误	1	“Sheng.SailingEase.Kernal.SEXElement.implicit operator Sheng.SailingEase.Kernal.SEXElement(System.Xml.Linq.XElement)”: 
        //不允许进行以基类为转换源或目标的用户定义转换
        ///// <summary>
        ///// 由 XElement 到 SEXElement 的隐士转换
        ///// </summary>
        ///// <param name="element"></param>
        ///// <returns></returns>
        //public static implicit operator SEXElement(XElement element)
        //{
        //    return new SEXElement(element);
        //}
    }
}
