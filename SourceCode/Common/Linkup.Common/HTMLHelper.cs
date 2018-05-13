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
using System.Text.RegularExpressions;

namespace Linkup.Common
{
    //http://www.cnblogs.com/yeminglong/archive/2012/09/27/2705721.html
    public class HTMLHelper
    {
        /**/
        ///   <summary>
        ///   去除HTML标记，包括脚本段等
        ///   </summary>
        ///   <param   name="NoHTML">包括HTML的源码   </param>
        ///   <returns>已经去除后的文字</returns>
        public static string GetPureText(string strHtml)
        {
            if (String.IsNullOrEmpty(strHtml))
                return String.Empty;

            //删除脚本
            strHtml = Regex.Replace(strHtml, @"<script[^>]*?>.*?</script>", "",
              RegexOptions.IgnoreCase);
            //删除HTML
            strHtml = Regex.Replace(strHtml, @"<(.[^>]*)>", "",
              RegexOptions.IgnoreCase);
            strHtml = Regex.Replace(strHtml, @"([\r\n])[\s]+", "",
              RegexOptions.IgnoreCase);
            strHtml = Regex.Replace(strHtml, @"-->", "", RegexOptions.IgnoreCase);
            strHtml = Regex.Replace(strHtml, @"<!--.*", "", RegexOptions.IgnoreCase);
            strHtml = Regex.Replace(strHtml, @"&(quot|#34);", "\"",
              RegexOptions.IgnoreCase);
            strHtml = Regex.Replace(strHtml, @"&(amp|#38);", "&",
              RegexOptions.IgnoreCase);
            strHtml = Regex.Replace(strHtml, @"&(lt|#60);", "<",
              RegexOptions.IgnoreCase);
            strHtml = Regex.Replace(strHtml, @"&(gt|#62);", ">",
              RegexOptions.IgnoreCase);
            strHtml = Regex.Replace(strHtml, @"&(nbsp|#160);", "   ",
              RegexOptions.IgnoreCase);
            strHtml = Regex.Replace(strHtml, @"&(iexcl|#161);", "\xa1",
              RegexOptions.IgnoreCase);
            strHtml = Regex.Replace(strHtml, @"&(cent|#162);", "\xa2",
              RegexOptions.IgnoreCase);
            strHtml = Regex.Replace(strHtml, @"&(pound|#163);", "\xa3",
              RegexOptions.IgnoreCase);
            strHtml = Regex.Replace(strHtml, @"&(copy|#169);", "\xa9",
              RegexOptions.IgnoreCase);
            strHtml = Regex.Replace(strHtml, @"&#(\d+);", "",
              RegexOptions.IgnoreCase);

            strHtml.Replace("<", "");
            strHtml.Replace(">", "");
            strHtml.Replace("\r\n", "");
            //Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

            return strHtml;
        }

        ///   <summary>
        ///   仅移除HTML标签
        ///   </summary>
        ///   <param   name="HTMLStr">strHtml</param>
        public static string FilterTags(string strHtml)
        {
            if (String.IsNullOrEmpty(strHtml))
                return String.Empty;

            //过滤 &nbsp;
            strHtml = strHtml.Replace("&nbsp;", " ");

            //过滤表情 [=001.gif=] ，为 [表情]
            strHtml = Regex.Replace(strHtml, @"\[=.*?=\]", "[表情]",
              RegexOptions.IgnoreCase);

            return System.Text.RegularExpressions.Regex.Replace(strHtml, "<[^>]*>", "");
        }
    }
}
