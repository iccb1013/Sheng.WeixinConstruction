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
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.WeixinContract
{
    /*
     * 这里的类用以封装微信API所返回的JSON
     * 因为微信大部分接口返回的JSON格式不固定
     * 如果有异常或成功无返回值，返回{"errcode":0,"errmsg":"send job submission success"}这种
     * 如果成功并有返回数据，则返回相应的数据JSON
     */

    public class RequestApiResult
    {
        private static WeixinErrorCode _weixinErrorCode = WeixinErrorCode.Instance;

        public HttpRequestArgs HttpRequestArgs
        {
            get;
            set;
        }

        public HttpRequestResult HttpRequestResult
        {
            get;
            set;
        }

        //如果使用索引器，则无法使用ref 或out传递
        public WeixinApiErrorResult ApiError;

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

        /// <summary>
        /// 封装错误信息，来自HTTP本身或 ApiError
        /// </summary>
        public string Message
        {
            get
            {
                if (HttpRequestResult != null && HttpRequestResult.Success == false)
                    return HttpRequestResult.Exception.Message;

                if (ApiError != null && ApiError.Success == false)
                {
                    string errorMessage = _weixinErrorCode.GetMessage(ApiError.ErrorCode);
                    if (String.IsNullOrEmpty(errorMessage))
                    {
                        return ApiError.ErrorCode + "-" + ApiError.ErrorMessage;
                    }
                    else
                    {
                        return ApiError.ErrorCode + "-" + errorMessage;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// 获取详细信息，用来写日志
        /// </summary>
        /// <returns></returns>
        public string GetDetail()
        {
            StringBuilder str = new StringBuilder();
            if (HttpRequestArgs != null)
            {
                str.Append("请求的URL：");
                str.AppendLine();
                str.Append(HttpRequestArgs.Url);
                str.AppendLine();
                str.Append("发送的内容：");
                str.AppendLine();
                str.Append(HttpRequestArgs.Content);
                str.AppendLine();
                if (String.IsNullOrEmpty(HttpRequestArgs.File) == false)
                {
                    str.Append("文件：");
                    str.AppendLine();
                    str.Append(HttpRequestArgs.File);
                    str.AppendLine();
                }
            }
            if (HttpRequestResult != null)
            {
                if (HttpRequestResult.Exception != null)
                {
                    str.Append("异常：");
                    str.AppendLine();
                    str.Append(HttpRequestResult.Exception.Message);
                    str.AppendLine();
                }
                else
                {
                    str.Append("返回的内容：");
                    str.AppendLine();
                    str.Append(HttpRequestResult.Content);
                    str.AppendLine();
                }
            }

            return str.ToString();
        }

        /// <summary>
        /// 是否重试
        /// </summary>
        public bool Retry
        {
            get
            {
                //-1	系统繁忙，此时请开发者稍候再试
                //40001	获取access_token时AppSecret错误，或者access_token无效。请开发者认真比对AppSecret的正确性，或查看是否正在为恰当的公众号调用接口
                return ApiError != null && (ApiError.ErrorCode == 40001 || ApiError.ErrorCode == -1);
            }
        }
    }

    public class RequestApiResult<T> : RequestApiResult where T : class
    {
        public T ApiResult
        {
            get;
            set;
        }

        public override bool Success
        {
            get
            {
                if (HttpRequestResult != null && HttpRequestResult.Success && ApiResult != null)
                    return true;
                else
                    return false;
            }
        }
    }
}
