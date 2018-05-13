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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    /*
     * 这里的类用以封装微信API所返回的JSON
     * 因为微信大部分接口返回的JSON格式不固定
     * 如果有异常或成功无返回值，返回{"errcode":0,"errmsg":"send job submission success"}这种
     * 如果成功并有返回数据，则返回相应的数据JSON
     * 
     * 但是，有些接口在成功时，并不返回特别的JSON，而是使用 errcode 一样的格式，用多加字段的方式返回
     * 如 消息群发接口，长链接转短链接接口 
     * >>不过可以把多加的字段单定义成类使用 RequestApiResult<T> 
     */

    /// <summary>
    /// 错误时微信会返回JSON数据包
    /// {"errcode":40013,"errmsg":"invalid appid"}
    /// </summary>
    [DataContract]
    public class WeixinApiErrorResult
    {
        /// <summary>
        /// http://mp.weixin.qq.com/wiki/17/fa4e1434e57290788bde25603fa2fcbd.html
        /// </summary>
        [DataMember(Name = "errcode")]
        public int ErrorCode
        {
            get;
            set;
        }

        [DataMember(Name = "errmsg")]
        public string ErrorMessage
        {
            get;
            set;
        }

        public bool Success
        {
            get
            {
                return ErrorCode == 0;
            }
        }
    }

    public class WeixinApiErrorResult<T> where T : WeixinApiErrorResult
    {
        public HttpRequestResult HttpRequestResult
        {
            get;
            set;
        }

        //如果使用索引器，则无法使用ref 或out传递
        public T ApiError;

        public virtual bool Success
        {
            get
            {
                if (HttpRequestResult.Success && ApiError != null && ApiError.Success)
                    return true;
                else
                    return false;
            }
        }

        public string Message
        {
            get
            {
                if (HttpRequestResult != null && HttpRequestResult.Success == false)
                    return HttpRequestResult.Exception.Message;

                if (ApiError != null && ApiError.Success == false)
                    return ApiError.ErrorCode + "-" + ApiError.ErrorMessage;

                return String.Empty;
            }
        }
    }
}
