using Linkup.Common;
using Linkup.Data;
using Linkup.DataRelationalMapping;
using Newtonsoft.Json;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.FileService.Models;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace Sheng.WeixinConstruction.FileService
{
    public class FileDownloadQueue
    {
        private static readonly FileDownloadQueue _instance = new FileDownloadQueue();
        public static FileDownloadQueue Instance
        {
            get { return _instance; }
        }

        private LogService _log = LogService.Instance;
        private ExceptionHandlingService _exceptionHandling = ExceptionHandlingService.Instance;
        private CachingService _cachingService = CachingService.Instance;
        private HttpService _httpService = HttpService.Instance;
        private FileDomainPool _fileDomainPool = FileDomainPool.Instance;

        private DatabaseWrapper _dataBase = new DatabaseWrapper("FileConnection");

        private Thread _searchThread;
        private Thread _downloadThread1;
        private Thread _downloadThread2;
        private Thread _downloadThread3;
        private Thread _downloadThread4;
        private Thread _downloadThread5;

        private ConcurrentQueue<FileDownloadQueueEntity> _itemQueue = new ConcurrentQueue<FileDownloadQueueEntity>();

        private string _serverRootDir = AppSettings.ServerPath;

        private FileDownloadQueue()
        {
            _searchThread = new Thread(SearchThreadMethod);
            _searchThread.Start();

            _downloadThread1 = new Thread(DownloadThreadMethod);
            _downloadThread1.Start();

            _downloadThread2 = new Thread(DownloadThreadMethod);
            _downloadThread2.Start();

            _downloadThread3 = new Thread(DownloadThreadMethod);
            _downloadThread3.Start();

            _downloadThread4 = new Thread(DownloadThreadMethod);
            _downloadThread4.Start();

            _downloadThread5 = new Thread(DownloadThreadMethod);
            _downloadThread5.Start();
        }

        public void Enqueue(FileDownloadQueueEntity item)
        {
            if (item == null)
            {
                _log.Write("FileDownloadQueueEntity == null", String.Empty, TraceEventType.Warning);
                return;
            }

            _dataBase.Insert(item);
        }

        private void SearchThreadMethod()
        {
            _log.Write("下载队列中的文件线程启动", TraceEventType.Verbose);

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@status", FileDownloadQueueItemStatus.Pending));

            while (true)
            {
                if (_itemQueue.Count > 0)
                {
                    Thread.Sleep(10 * 1000);
                    continue;
                }

                List<FileDownloadQueueEntity> queueList = _dataBase.Select<FileDownloadQueueEntity>(
                    "SELECT * FROM [FileDownloadQueue] WHERE [Status] = @status ORDER BY [CreateTime] ASC", parameterList);

                if (queueList.Count == 0)
                {
                    Thread.Sleep(60 * 1000);
                    continue;
                }

                foreach (var item in queueList)
                {
                    _itemQueue.Enqueue(item);
                }
            }
        }

        private void DownloadThreadMethod()
        {
            while (true)
            {
                FileDownloadQueueEntity item;
                if (_itemQueue.TryDequeue(out item))
                {
                    try
                    {
                        Download(item);
                    }
                    catch (Exception ex)
                    {
                        _exceptionHandling.HandleException(ex);
                    }
                }
                else
                {
                    Thread.Sleep(10 * 1000);
                    continue;
                }
            }
        }

        private void Download(FileDownloadQueueEntity item)
        {
            _log.Write("下载队列中的文件", JsonConvert.SerializeObject(item), TraceEventType.Verbose);

            DomainContext domainContext = _fileDomainPool.GetDomainContext(item.Domain);
            if (domainContext == null)
            {
                item.Status = FileDownloadQueueItemStatus.Done;
                item.Success = false;
                item.Message = "无效域";
                item.FinishTime = DateTime.Now;
                _dataBase.Update(item);
                return;
            }

            string targetDir = Path.Combine(_serverRootDir, "FileStore", item.Domain.ToString());
            if (Directory.Exists(targetDir) == false)
                Directory.CreateDirectory(targetDir);

            string accessToken = domainContext.AccessToken;

            HttpDownloadArgs downloadArgs = new HttpDownloadArgs();
            downloadArgs.Url = String.Format(
                "http://file.api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}",
                accessToken, item.MediaId);
            downloadArgs.TargetDir = targetDir;

            HttpDownloadResult downloadResult = _httpService.Download(downloadArgs);

            if (downloadResult.Success == false)
            {
                item.Status = FileDownloadQueueItemStatus.Done;
                item.Success = false;
                item.Message = "下载失败：" + downloadResult.Message;
                item.FinishTime = DateTime.Now;
                _dataBase.Update(item);
                return;
            }

            item.Status = FileDownloadQueueItemStatus.Done;
            item.Success = downloadResult.Success;
            item.Message = downloadResult.Message;
            item.OutputFile = String.Format("FileStore/{0}/{1}", item.Domain, downloadResult.StoreFileName);
            item.ContentType = downloadResult.ContentType;
            item.FileLength = (int)(downloadResult.FileLength / 1024);

            FileDownloadQueueWithMediaIdResult result = new FileDownloadQueueWithMediaIdResult();
            result.Success = item.Success;
            result.Message = item.Message;
            result.OutputFile = item.OutputFile;
            result.ContentType = item.ContentType;
            result.Tag = item.Tag;
            result.MediaId = item.MediaId;
            result.FileLength = item.FileLength;

            _log.Write("下载队列中的文件返回（WithMediaId）", JsonConvert.SerializeObject(result), TraceEventType.Verbose);

            //把下载结果POST到回调地址上
            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = item.CallbackUrl;
            requestArgs.Content = JsonConvert.SerializeObject(result);

            HttpRequestResult requestResult = _httpService.Request(requestArgs);
            if (requestResult.Success)
            {
                ApiResult callbackApiResult = JsonConvert.DeserializeObject<ApiResult>(requestResult.Content);
                if (callbackApiResult.Success == false)
                {
                    item.Status = FileDownloadQueueItemStatus.Done;
                    item.Success = false;
                    item.Message = "回调失败：" + callbackApiResult.Message;
                    item.FinishTime = DateTime.Now;
                }
            }
            else
            {
                item.Status = FileDownloadQueueItemStatus.Done;
                item.Success = false;
                item.Message = "回调失败：" + requestResult.Exception.Message;
                item.FinishTime = DateTime.Now;
            }

            _dataBase.Update(item);

        }
    }
}