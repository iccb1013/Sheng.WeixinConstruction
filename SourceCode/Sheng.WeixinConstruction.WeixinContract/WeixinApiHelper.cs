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


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    public static class WeixinApiHelper
    {
        /// <summary>
        /// 将Unix时间戳转换为DateTime类型时间
        /// </summary>
        /// <param name="d">double 型数字</param>
        /// <returns>DateTime</returns>
        public static DateTime ConvertIntToDateTime(int d)
        {
            System.DateTime time = System.DateTime.MinValue;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            time = startTime.AddSeconds(d);
            return time;
        }

        /// <summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>double</returns>
        public static int ConvertDateTimeToInt(System.DateTime time)
        {
            int intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = (int)((time - startTime).TotalSeconds);
            return intResult;
        }

        /// <summary>
        /// 用于微信支付
        /// 格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010。其他详见时间规则
        /// </summary>
        /// <returns></returns>
        public static DateTime ConvertStringToDateTime(string str)
        {
            if (String.IsNullOrEmpty(str) || str.Length != 14)
                return DateTime.MinValue;

            int year = int.Parse(str.Substring(0, 4));
            int month = int.Parse(str.Substring(4, 2));
            int day = int.Parse(str.Substring(6, 2));

            int hour = int.Parse(str.Substring(8, 2));
            int minute = int.Parse(str.Substring(10, 2));
            int second = int.Parse(str.Substring(12, 2));

            DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
            return dateTime;
        }

        /// <summary>
        /// 因为weixin的SB接口返回的JSON格式不固定
        /// 所以需要特别解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static T Parse<T>(string json, ref WeixinApiErrorResult error) where T : class,new()
        {
            if (String.IsNullOrEmpty(json))
            {
                error = new WeixinApiErrorResult();
                error.ErrorMessage = "JSON Parse Error";
                return null;
            }

            /*
             * 有些接口在成功时，并不返回特别的JSON，而是使用 errcode 一样的格式，用多加字段的方式返回
             * 所以除了要判断有没有errcode，还要判断它的值，0表示成功
             */
            JObject jObject = JObject.Parse(json);
            JToken jToken = jObject["errcode"];
            if (jToken != null && jToken.Value<int>() != 0) //&& jToken.HasValues 为false，原因不明  //&& jToken.Value<int>() != 0
            {
                //Debug.Assert(jToken.Value<int>() != 0, "");
                error = JsonConvert.DeserializeObject<WeixinApiErrorResult>(json);
                return null;
            }
            else
            {
                T result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
        }

        /// <summary>
        /// 专门用来处理 
        /// {"errcode":40013,"errmsg":"invalid appid"}
        /// {"errcode": 0, "errmsg": "ok"}
        /// 这种固定格式的json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static WeixinApiErrorResult Parse(string json)
        {
            WeixinApiErrorResult error = new WeixinApiErrorResult();
            if (String.IsNullOrEmpty(json))
            {
                error.ErrorCode = -1;
                error.ErrorMessage = "JSON Parse Error";
                return error;
            }

            /*
             * 有些接口在成功时，并不返回特别的JSON，而是使用 errcode 一样的格式，用多加字段的方式返回
             * 所以除了要判断有没有errcode，还要判断它的值，0表示成功
             */
            JObject jObject = JObject.Parse(json);
            JToken jToken = jObject["errcode"];
            if (jToken != null) //&& jToken.HasValues 为false，原因不明  //&& jToken.Value<int>() != 0
            {
                error = JsonConvert.DeserializeObject<WeixinApiErrorResult>(json);
                return error;
            }
            else
            {
                error.ErrorCode = -1;
                error.ErrorMessage = "JSON Parse Error";
                return error;
            }
        }

        /// <summary>
        /// 随机字符串
        /// </summary>
        /// <returns></returns>
        public static string GetNonceStr()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static long GetTimesTamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }
    }
}
