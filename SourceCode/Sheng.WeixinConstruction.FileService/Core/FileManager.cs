using Linkup.Common;
using Linkup.Data;
using Newtonsoft.Json;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Sheng.WeixinConstruction.FileService
{
    public class FileManager
    {
        private LogService _log = LogService.Instance;
        private ExceptionHandlingService _exceptionHandling = ExceptionHandlingService.Instance;
        private CachingService _cachingService = CachingService.Instance;
        private HttpService _httpService = HttpService.Instance;

        private DatabaseWrapper _dataBase = new DatabaseWrapper("FileConnection");

        private TimeSpan _cachingExpiresIn = new TimeSpan(2, 0, 0);

        private string _serverRootDir = AppSettings.ServerPath;

        private static readonly FileManager _instance = new FileManager();
        public static FileManager Instance
        {
            get { return _instance; }
        }

        private FileManager()
        {
        }

        internal FileUploadResult Save(SaveArgs args)
        {
            FileUploadResult result = new FileUploadResult();

            //流长度在关闭释放流之后就取不到了
            int length = (int)(args.Stream.Length / 1024);

            FileStream fsWrite = null;
            Stream stream = null;
            string storeFileName;

            #region 存储文件

            try
            {
                string targetDir = Path.Combine(_serverRootDir, "FileStore", args.Domain.ToString());
                if (Directory.Exists(targetDir) == false)
                    Directory.CreateDirectory(targetDir);

                storeFileName = Guid.NewGuid().ToString() + args.FileExtension;

                string outputFileName = Path.Combine(targetDir, storeFileName);

                ////Path.Combine 是 \， HTTP 地址用的是 /
                ////但是序列化时，会对 / 转义成 \/ 
                //result.RelativeUri = "FileStore" + "/" + _site.SiteInfo.Code + "/" + storeFileName;

                byte[] buffer = new byte[4096];

                fsWrite = new FileStream(outputFileName, FileMode.Create);
                stream = args.Stream;
                stream.Position = 0;

                int read = 0;
                do
                {
                    read = stream.Read(buffer, 0, buffer.Length);
                    fsWrite.Write(buffer, 0, read);

                } while (read > 0);
            }
            catch (Exception ex)
            {
                _exceptionHandling.HandleException(ex);
                result.Message = ex.Message;
                result.Success = false;
                return result;
            }
            finally
            {
                if (fsWrite != null)
                {
                    fsWrite.Flush();
                    fsWrite.Close();
                    fsWrite.Dispose();
                }
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }

            #endregion

            //存储数据库记录
            FileRecord record = new FileRecord();
            record.Id = args.Id;
            record.Domain = args.Domain;
            record.FileName = args.FileName;
            record.StoredFileName = storeFileName;
            record.Length = length;
            record.MD5 = args.MD5;
            record.UploadDate = args.UploadDate;
            record.UploadIPAddress = args.UploadIPAddress;

            _dataBase.Insert(record);

            result.Success = true;
            result.Id = args.Id;
            result.StoreFilePath = String.Format("FileStore/{0}/{1}", args.Domain, storeFileName);

            return result;
        }

        internal FileRecord Get(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            string cachingKey = "fsFileRecord:" + id.ToString();

            FileRecord fileRecord;

            fileRecord = _cachingService.Get<FileRecord>(cachingKey);
            if (fileRecord != null)
                return fileRecord;

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@id", id));

            List<FileRecord> fileRecordList =
                _dataBase.Select<FileRecord>("SELECT * FROM [File] Where Id = @id", parameterList);

            if (fileRecordList.Count == 0)
                return null;

            fileRecord = fileRecordList[0];

            _cachingService.Set(cachingKey, fileRecord, _cachingExpiresIn);

            return fileRecord;
        }

        internal FileDownloadAgentResult Download(FileDownloadAgentArgs args)
        {
            if (args.Sync)
            {
                return DownloadFile(args);
            }
            else
            {
                Task<FileDownloadAgentResult> downloadTask = new Task<FileDownloadAgentResult>(DownloadFileTask, args);
                downloadTask.ContinueWith((task) =>
                {
                    FileDownloadAgentResult downloadResult;
                    if (task.IsFaulted)
                    {
                        downloadResult = new FileDownloadAgentResult();
                        if (task.Exception.InnerException != null)
                        {
                            downloadResult.Message = task.Exception.InnerException.Message;
                            _log.Write("异步下载失败", task.Exception.InnerException.Message + "\r\n" +
                                task.Exception.InnerException.StackTrace, TraceEventType.Error);
                        }
                        else
                        {
                            downloadResult.Message = task.Exception.Message;
                            _log.Write("异步下载失败", task.Exception.Message + "\r\n" +
                                task.Exception.StackTrace, TraceEventType.Error);
                        }
                    }
                    else
                    {
                        downloadResult = task.Result;
                    }

                    //把下载结果POST到回调地址上
                    HttpRequestArgs requestArgs = new HttpRequestArgs();
                    requestArgs.Method = "POST";
                    requestArgs.Url = args.CallbackUrl;
                    requestArgs.Content = JsonConvert.SerializeObject(downloadResult);

                    _httpService.Request(requestArgs);

                });
                downloadTask.Start();


                FileDownloadAgentResult result = new FileDownloadAgentResult();
                result.Message = "这是一个异步下载任务。";
                return result;
            }
        }

        private FileDownloadAgentResult DownloadFileTask(object state)
        {
            FileDownloadAgentArgs args = (FileDownloadAgentArgs)state;
            FileDownloadAgentResult result = DownloadFile(args);
            return result;
        }

        private FileDownloadAgentResult DownloadFile(FileDownloadAgentArgs args)
        {
            _log.Write("发起文件代理下载请求", JsonConvert.SerializeObject(args), TraceEventType.Verbose);

            string targetDir = Path.Combine(args.ServerRootDir, "FileStore", args.Domain.ToString());
            if (Directory.Exists(targetDir) == false)
                Directory.CreateDirectory(targetDir);

            //string storeFileName = Guid.NewGuid().ToString() + args.FileExtension;
            //string outputFileName = Path.Combine(targetDir, storeFileName);

            HttpDownloadArgs downloadArgs = new HttpDownloadArgs();
            downloadArgs.Url = args.Url;
            downloadArgs.TargetDir = targetDir;
            HttpDownloadResult downloadResult = _httpService.Download(downloadArgs);

            FileDownloadAgentResult result = new FileDownloadAgentResult();
            result.Success = downloadResult.Success;
            result.Message = downloadResult.Message;
            result.OutputFile = String.Format("FileStore/{0}/{1}", args.Domain, downloadResult.StoreFileName);
            result.ContentType = downloadResult.ContentType;
            result.Tag = args.Tag;

            _log.Write("文件代理下载请求返回", JsonConvert.SerializeObject(result), TraceEventType.Verbose);

            return result;
        }

        internal FileDownloadAgentWithMediaIdResult DownloadWithMediaId(DomainContext domainContext, FileDownloadAgentWithMediaIdArgs args)
        {
            //下载多媒体文件
            //http://mp.weixin.qq.com/wiki/12/58bfcfabbd501c7cd77c19bd9cfa8354.html

            if (args.Sync)
            {
                return DownloadFileWithMediaId(domainContext, args);
            }
            else
            {
                object[] stateArray = new object[2];
                stateArray[0] = domainContext;
                stateArray[1] = args;

                Task<FileDownloadAgentWithMediaIdResult> downloadTask = new Task<FileDownloadAgentWithMediaIdResult>(DownloadFileWithMediaIdTask, stateArray);
                downloadTask.ContinueWith((task) =>
                {
                    FileDownloadAgentWithMediaIdResult downloadResult;
                    if (task.IsFaulted)
                    {
                        downloadResult = new FileDownloadAgentWithMediaIdResult();
                        if (task.Exception.InnerException != null)
                        {
                            downloadResult.Message = task.Exception.InnerException.Message;
                            _log.Write("异步下载失败", task.Exception.InnerException.Message + "\r\n" +
                                task.Exception.InnerException.StackTrace, TraceEventType.Error);
                        }
                        else
                        {
                            downloadResult.Message = task.Exception.Message;
                            _log.Write("异步下载失败", task.Exception.Message + "\r\n" +
                                task.Exception.StackTrace, TraceEventType.Error);
                        }
                    }
                    else
                    {
                        downloadResult = task.Result;
                    }

                    //把下载结果POST到回调地址上
                    HttpRequestArgs requestArgs = new HttpRequestArgs();
                    requestArgs.Method = "POST";
                    requestArgs.Url = args.CallbackUrl;
                    requestArgs.Content = JsonConvert.SerializeObject(downloadResult);

                    _httpService.Request(requestArgs);

                });
                downloadTask.Start();


                FileDownloadAgentWithMediaIdResult result = new FileDownloadAgentWithMediaIdResult();
                result.Message = "这是一个异步下载任务。";
                return result;
            }
        }

        private FileDownloadAgentWithMediaIdResult DownloadFileWithMediaIdTask(object state)
        {
            object[] stateArray = (object[])state;

            DomainContext domainContext = (DomainContext)stateArray[0];
            FileDownloadAgentWithMediaIdArgs args = (FileDownloadAgentWithMediaIdArgs)stateArray[1];
            FileDownloadAgentWithMediaIdResult result = DownloadFileWithMediaId(domainContext, args);
            return result;
        }

        private FileDownloadAgentWithMediaIdResult DownloadFileWithMediaId(DomainContext domainContext, FileDownloadAgentWithMediaIdArgs args)
        {
            _log.Write("发起文件代理下载请求（WithMediaId）", JsonConvert.SerializeObject(args), TraceEventType.Verbose);

            string targetDir = Path.Combine(args.ServerRootDir, "FileStore", args.Domain.ToString());
            if (Directory.Exists(targetDir) == false)
                Directory.CreateDirectory(targetDir);

            string accessToken = domainContext.AccessToken;

            HttpDownloadArgs downloadArgs = new HttpDownloadArgs();
            downloadArgs.Url = String.Format(
                "http://file.api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}",
                accessToken, args.MediaId);
            downloadArgs.TargetDir = targetDir;
            HttpDownloadResult downloadResult = _httpService.Download(downloadArgs);

            FileDownloadAgentWithMediaIdResult result = new FileDownloadAgentWithMediaIdResult();
            result.Success = downloadResult.Success;
            result.Message = downloadResult.Message;
            result.OutputFile = String.Format("FileStore/{0}/{1}", args.Domain, downloadResult.StoreFileName);
            result.ContentType = downloadResult.ContentType;
            result.Tag = args.Tag;
            result.MediaId = args.MediaId;

            _log.Write("文件代理下载请求返回（WithMediaId）", JsonConvert.SerializeObject(result), TraceEventType.Verbose);

            return result;
        }
    }

}