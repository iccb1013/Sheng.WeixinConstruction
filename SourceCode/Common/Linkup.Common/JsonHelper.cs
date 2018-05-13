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
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Linkup.Common
{
    /*
     * 关于Date(1294499956278+0800) 格式与普通格式互转
     * .net 4.5 以上可以用 DataContractJsonSerializerSettings 直接处理
     * 4.0 及以下，用正则替换字符串的形式处理
     * 需要注意的是 1294499956278 是以UTC时间为基准的
     * 当把 1294499956278 转成普通格式时，需要转为本地时间
     * 再把本地时间转成 Date(1294499956278+0800) 这种格式时，需要先把时间转为UTC时间
     * 另外，TotalMilliseconds 是不包括最后的毫秒数的，所以，微软序列化时得到的
     * 比如 1422798535836 ，再处理后丢失毫秒变为 1422798535000
     * 对精确要求不高时可无视
     * 
     */
    public class JsonHelper
    {
        static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings();

        // DataContractJsonSerializer 类在 .net 4.5 版本以上才有
        //static DataContractJsonSerializerSettings _dataContractJsonSerializerSettings =
        //    new DataContractJsonSerializerSettings();

        //http://blog.csdn.net/cncdns/article/details/6164389
        //将来 /Date(1422795002671+0800)/ 转换为可读字符串
        //{"head":{"content":"dateTime","sign":0,"version":1},"content":{"dateTime":"\/Date(1422795002671+0800)\/"}}
        //替换Json的Date字符串    
        static MatchEvaluator _convertMSJsonDateToDateStringEvaluator = new MatchEvaluator(ConvertMSJsonDateToDateString);
        //取出 \/Date(1422795002671+0800)\/
        //毫秒部分有可能为负数
        static Regex _msJsonDateRegex = new Regex(@"\\/Date\(-*[0-9]*\+[0-9]*\)\\/");

        //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式    
        static MatchEvaluator _convertDateStringToMSJsonDateEvaluator = new MatchEvaluator(ConvertDateStringToMSJsonDate);
        static Regex _dateStringRegex = new Regex(@"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}");

        static Regex _msJsonDateTotalMillisecondsRegex = new Regex(@"(?<=\\/Date\()-*[0-9]*(?=\+[0-9]*\)\\/)");



        static JsonHelper()
        {
            // hh 为12小时制， HH为24小时制
            _jsonSerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            //_jsonSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            //_jsonSerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            JsonConvert.DefaultSettings = () => { return _jsonSerializerSettings; };

            //_dataContractJsonSerializerSettings.DateTimeFormat = new DateTimeFormat("yyyy-MM-dd HH:mm:ss");
        }

        public static string Serializer(object target)
        {
            string json = String.Empty;

            if (target == null)
                return json;

            Type parameterType = target.GetType();
            bool isList = parameterType.GetInterface("IList") != null;
            bool isDataContract = parameterType.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0;
            bool listObjIsDataContract = true;

            if (isList)
            {
                IList list = (IList)target;
                if (list.Count > 0)
                {
                    object listObj = list[0];
                    listObjIsDataContract = listObj.GetType().GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0;
                }
            }

            if ((isList && listObjIsDataContract) || isDataContract)
                json = JsonHelper.DataContractSerializer(target);
            else
                json = JsonHelper.JavaScriptSerializer(target);

            return json;
        }

        private static string DataContractSerializer(object target)
        {
            if (target == null)
                return String.Empty;

            try
            {
                //日期时间默认的是 //Date(1294499956278+0800)// 这样的格式
                //http://blog.csdn.net/cncdns/article/details/6164389
                //DataContractJsonSerializer ser = new DataContractJsonSerializer(
                //    target.GetType(), _dataContractJsonSerializerSettings);
                DataContractJsonSerializer ser = new DataContractJsonSerializer(target.GetType());

                MemoryStream ms = new MemoryStream();
                ser.WriteObject(ms, target);
                string jsonString = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();

                jsonString = _msJsonDateRegex.Replace(jsonString, _convertMSJsonDateToDateStringEvaluator);

                return jsonString;
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                return String.Empty;
            }
        }

        private static string JavaScriptSerializer(object target)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string result = serializer.Serialize(target);
            return result;
        }

        public static T Deserialize<T>(string jsonString) where T : class
        {
            return Deserialize<T>(jsonString, true);
        }
        public static T Deserialize<T>(string jsonString, bool convertDateString) where T : class
        {
            Type type = typeof(T);
            return Deserialize(jsonString, type, convertDateString) as T;
        }

        public static object Deserialize(string jsonString, Type type)
        {
            return Deserialize(jsonString, type, true);
        }

        public static object Deserialize(string jsonString, Type type, bool convertDateString)
        {
            if (String.IsNullOrEmpty(jsonString))
                return null;

            object obj = null;

            try
            {
                if (convertDateString)
                    jsonString = _dateStringRegex.Replace(jsonString, _convertDateStringToMSJsonDateEvaluator);

                //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T),_dataContractJsonSerializerSettings);
                DataContractJsonSerializer ser = new DataContractJsonSerializer(type);
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                obj = ser.ReadObject(ms);
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }

            return obj;
        }

        public static T JavaScriptDeserialize<T>(string jsonString)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(jsonString);
        }

        //Newtonsoft方案支持dataset,datatable
        //但不支持泛型
        public static string NewtonsoftSerializer(object target)
        {
            return JsonConvert.SerializeObject(target);
        }

        public static T NewtonsoftDeserialize<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        //public static string GetProperty(string json, string property)
        //{
        //    try
        //    {
        //        JObject jObject = JObject.Parse(json);
        //        if (jObject[property] == null)
        //            return String.Empty;
        //        return jObject[property].ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Assert(false, ex.Message);
        //        return String.Empty;
        //    }
        //}

        public static string GetProperty(string json, string propertyPath)
        {
            if (String.IsNullOrEmpty(json) || String.IsNullOrEmpty(propertyPath))
                return String.Empty;

            string[] propertyList = propertyPath.Split('.');

            string jsonCodon = json;

            for (int i = 0; i < propertyList.Length; i++)
            {
                string property = propertyList[i];

                if (String.IsNullOrEmpty(property))
                    return String.Empty;

                try
                {
                    JObject jObject = JObject.Parse(jsonCodon);
                    if (jObject[property] == null)
                        return String.Empty;

                    jsonCodon = jObject[property].ToString();
                }
                catch (Exception ex)
                {
                    Debug.Assert(false, ex.Message);
                    return String.Empty;
                }
            }

            return jsonCodon;
        }

        /// <summary>    
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串    
        /// </summary>    
        private static string ConvertMSJsonDateToDateString(Match m)
        {
            //m.Groups[0]
            //{\/Date(1422796634000+0800)\/}
            string dateString = _msJsonDateTotalMillisecondsRegex.Match(m.Groups[0].Value).Value;

            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(dateString));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }
        /// <summary>    
        /// 将时间字符串转为Json时间    
        /// </summary>    
        private static string ConvertDateStringToMSJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format(@"/Date({0}+0800)/", ts.TotalMilliseconds);
            return result;
        }
    }

    /// <summary>
    /// 使用哪种json序列化方案
    /// </summary>

}
