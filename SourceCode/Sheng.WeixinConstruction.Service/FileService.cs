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
using Newtonsoft.Json;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Service
{
    /// <summary>
    /// 封装文件服务
    /// </summary>
    public class FileService
    {
        private static readonly FileService _instance = new FileService();
        public static FileService Instance
        {
            get { return _instance; }
        }

        private LogService _log = LogService.Instance;

        private readonly string _fileServiceUri = ConfigurationManager.AppSettings["FileService"];
        private readonly HttpService _httpService = HttpService.Instance;

        public string FileServiceUri
        {
            get
            {
                return _fileServiceUri;
            }
        }

        public FileDownloadAgentResult DownloadAgent(FileDownloadAgentArgs args)
        {
            FileDownloadAgentResult result;

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = _fileServiceUri + "File/DownloadAgent";
            requestArgs.Content = JsonConvert.SerializeObject(args);

            _log.Write("发起文件代理下载请求", requestArgs.Content, TraceEventType.Verbose);

            HttpRequestResult requestResult = _httpService.Request(requestArgs);

            if (requestResult.Success)
            {
                _log.Write("文件代理下载请求返回", requestResult.Content, TraceEventType.Verbose);
                result = JsonConvert.DeserializeObject<FileDownloadAgentResult>(requestResult.Content);
            }
            else
            {
                result = new FileDownloadAgentResult();
                result.Message = requestResult.Exception.Message;
            }
            return result;
        }

        public FileDownloadAgentWithMediaIdResult DownloadAgentWithMediaId(FileDownloadAgentWithMediaIdArgs args)
        {
            FileDownloadAgentWithMediaIdResult result;

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = _fileServiceUri + "File/DownloadAgentWithMediaId";
            requestArgs.Content = JsonConvert.SerializeObject(args);

            _log.Write("发起文件代理下载请求（DownloadAgentWithMediaId）", requestArgs.Content, TraceEventType.Verbose);

            HttpRequestResult requestResult = _httpService.Request(requestArgs);

            if (requestResult.Success)
            {
                _log.Write("文件代理下载请求返回（DownloadAgentWithMediaId）", requestResult.Content, TraceEventType.Verbose);
                result = JsonConvert.DeserializeObject<FileDownloadAgentWithMediaIdResult>(requestResult.Content);
            }
            else
            {
                result = new FileDownloadAgentWithMediaIdResult();
                result.Message = requestResult.Exception.Message;
            }
            return result;
        }

        public ApiResult DownloadQueueWithMediaId(FileDownloadQueueWithMediaIdArgs args)
        {
            ApiResult result;

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = _fileServiceUri + "File/DownloadQueueWithMediaId";
            requestArgs.Content = JsonConvert.SerializeObject(args);

            _log.Write("发起队列文件代理下载请求（DownloadQueueWithMediaId）", requestArgs.Content, TraceEventType.Verbose);

            HttpRequestResult requestResult = _httpService.Request(requestArgs);

            if (requestResult.Success)
            {
                _log.Write("队列文件代理下载请求返回（DownloadQueueWithMediaId）", requestResult.Content, TraceEventType.Verbose);
                result = JsonConvert.DeserializeObject<ApiResult>(requestResult.Content);
            }
            else
            {
                result = new ApiResult();
                result.Message = requestResult.Exception.Message;
            }

            return result;
        }

        /// <summary>
        /// 粉丝海报
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GetMemberQRCodeImageResult GetMemberQRCodeImage(GetCampaign_MemberQRCodeImageArgs args)
        {
            GetMemberQRCodeImageResult result;

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = _fileServiceUri + "MemberQRCode/GetMemberQRCodeImage";
            requestArgs.Content = JsonConvert.SerializeObject(args);

            _log.Write("向文件服务器发起 GetMemberQRCodeImage 请求", requestArgs.Content, TraceEventType.Verbose);

            HttpRequestResult requestResult = _httpService.Request(requestArgs);

            if (requestResult.Success)
            {
                _log.Write("文件服务器 GetMemberQRCodeImage 返回", requestResult.Content, TraceEventType.Verbose);
                result = JsonConvert.DeserializeObject<GetMemberQRCodeImageResult>(requestResult.Content);
            }
            else
            {
                result = new GetMemberQRCodeImageResult();
                result.Message = requestResult.Exception.Message;
            }
            return result;
        }

        public GetQRCodeImageResult GetQRCodeImage(GetQRCodeImageArgs args)
        {
            GetQRCodeImageResult result;

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = _fileServiceUri + "QRCode/GetQRCodeImage";
            requestArgs.Content = JsonConvert.SerializeObject(args);

            _log.Write("向文件服务器发起 GetQRCodeImage 请求", requestArgs.Content, TraceEventType.Verbose);

            HttpRequestResult requestResult = _httpService.Request(requestArgs);

            if (requestResult.Success)
            {
                _log.Write("文件服务器 GetQRCodeImage 返回", requestResult.Content, TraceEventType.Verbose);
                result = JsonConvert.DeserializeObject<GetQRCodeImageResult>(requestResult.Content);
            }
            else
            {
                result = new GetQRCodeImageResult();
                result.Message = requestResult.Exception.Message;
            }
            return result;
        }

    }
}
