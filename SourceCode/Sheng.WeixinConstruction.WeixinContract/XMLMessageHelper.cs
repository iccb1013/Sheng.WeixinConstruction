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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Sheng.WeixinConstruction.WeixinContract
{
    public static class XMLMessageHelper
    {
        public static string XmlSerialize(object obj)
        {
            /*
            * C# 的 xml 序列化时，文档总会带有 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
            * xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            * XmlSerializerNamespaces 用来去掉这个
            */
            MemoryStream memoryStream = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            // Remove the <?xml version="1.0" encoding="utf-8"?>
            settings.OmitXmlDeclaration = true;
            XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings);
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer ser = new XmlSerializer(obj.GetType());
            ser.Serialize(xmlWriter, obj, ns);
            byte[] byteArray = memoryStream.ToArray();
            string strResult = System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            xmlWriter.Close();
            xmlWriter.Dispose();
            memoryStream.Close();
            memoryStream.Dispose();

            return strResult;
        }

        
    }
}
