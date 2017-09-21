using Newtonsoft.Json;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.FileService.Models;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.FileService
{
    public class FileController : BasalController
    {
        private static readonly ControlledCachingService _controlledCachingService = ControlledCachingService.Instance;
        private static readonly CachingService _cachingService = CachingService.Instance;

        private static readonly FileDomainPool _fileDomainPool = FileDomainPool.Instance;
        
        private static readonly FileManager _fileManager = FileManager.Instance;
        private static readonly FileDownloadQueue _fileDownloadQueue = FileDownloadQueue.Instance;

        private TimeSpan _uploadResultExpiresIn = new TimeSpan(0, 0, 20);

        public FileController()
        {
            //_fileDownloadQueue.ServerRootDir = Server.MapPath("/");
        }

        //此处返回 ApiResult 没有意义
        //因为WEB上传无法跨域获得返回的结果
        //此处直接通过返回HTTP200或500告知上传结果(chrome 下不灵)
        //Chrome下完全无法取得任何返回信息
        //解决方案是上传后20秒之内通过 GetUploadResult 方法取结果
        /// <summary>
        /// 上传到本地文件服务器
        /// </summary>
        /// <returns></returns>
        public ActionResult Upload()
        {
            SaveFile();

            return new HttpStatusCodeResult(200);
        }

        /// <summary>
        /// 上传到微信素材库
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadToWeixinMaterial()
        {
            HttpRequestBase request = this.HttpContext.Request;

            string strFileId = request.Form["txtFileUpload_Id"];
            string cachingKey = "fsUploadResult:" + strFileId;

            ApiResult<WeixinAddMaterialResult> apiResult = new ApiResult<WeixinAddMaterialResult>();

            FileUploadResult saveResult = SaveFile();

            if (saveResult.Success == false)
            {
                apiResult.Message = saveResult.Message;
                _cachingService.Set(cachingKey, apiResult, _uploadResultExpiresIn);
                return new HttpStatusCodeResult(200);
            }

            string domainId = request.Form["txtFileUpload_Domain"];
            DomainContext domainContext = _fileDomainPool.GetDomainContext(Guid.Parse(domainId));

            string file = Server.MapPath("/") + saveResult.StoreFilePath;

            RequestApiResult<WeixinAddMaterialResult> addNormalMaterialResult =
                MaterialApiWrapper.AddNormalMaterial(domainContext, file, MaterialType.Image);
          
            apiResult.Success = addNormalMaterialResult.Success;
            if (addNormalMaterialResult.Success)
            {
                FileInfo fileInfo = new FileInfo(request.Files[0].FileName);
                addNormalMaterialResult.ApiResult.FileName = fileInfo.Name;

                apiResult.Message = saveResult.StoreFilePath;
            }
            else
            {
                apiResult.Message = addNormalMaterialResult.Message;
            }
            apiResult.Data = addNormalMaterialResult.ApiResult;
            _cachingService.Set(cachingKey, apiResult, _uploadResultExpiresIn);

            return new HttpStatusCodeResult(200);
        }

        /// <summary>
        /// 在图文消息的具体内容中，将过滤外部的图片链接，开发者可以通过下述接口上传图片得到URL，放到图文内容中使用
        /// 本接口所上传的图片不占用公众号的素材库中图片数量的5000个的限制。图片仅支持jpg/png格式，大小必须在1MB以下。
        /// https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1444738729&token=&lang=zh_CN
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadToWeixinImg()
        {
            HttpRequestBase request = this.HttpContext.Request;

            string strFileId = request.Form["txtFileUpload_Id"];
            string cachingKey = "fsUploadResult:" + strFileId;

            UploadToWeixinImgResult uploadToWeixinImgResult = new UploadToWeixinImgResult();

            FileUploadResult saveResult = SaveFile();

            if (saveResult.Success == false)
            {
                uploadToWeixinImgResult.Message = saveResult.Message;
                _cachingService.Set(cachingKey, uploadToWeixinImgResult, _uploadResultExpiresIn);
                return new HttpStatusCodeResult(200);
            }

            string domainId = request.Form["txtFileUpload_Domain"];
            DomainContext domainContext = _fileDomainPool.GetDomainContext(Guid.Parse(domainId));

            string file = Server.MapPath("/") + saveResult.StoreFilePath;

            RequestApiResult<WeixinUploadImgResult> weixinUploadImgResult =
                MaterialApiWrapper.UploadImg(domainContext, file);

            uploadToWeixinImgResult.Success = weixinUploadImgResult.Success;
            if (weixinUploadImgResult.Success == false)
            {
                uploadToWeixinImgResult.Message = weixinUploadImgResult.Message;
            }
            else
            {
                uploadToWeixinImgResult.Id = saveResult.Id;
                uploadToWeixinImgResult.WeixinUrl = weixinUploadImgResult.ApiResult.Url;
                uploadToWeixinImgResult.StoreFilePath = saveResult.StoreFilePath;
            }
            _cachingService.Set(cachingKey, uploadToWeixinImgResult, _uploadResultExpiresIn);

            return new HttpStatusCodeResult(200);
        }

        //public ActionResult Test()
        //{
        //    string accessToken = "prE27SQYE58WyfmXx0RD6RQSqmKoHHxmGUQTojSgVsL0lV7g1RcI2DpA3ELJdNvdWNjFsR9kMbhC2Mht9bJDPva7ZRcRYRDV73DDUsYpa-YTZBcAIAPVX";
        //    MaterialApi.AddNormalMaterial(accessToken, "D:\\a.jpg", MaterialType.Image);

        //    return View();
        //}

        private FileUploadResult SaveFile()
        {
            FileUploadResult fileUploadResult = new FileUploadResult();

            HttpRequestBase request = this.HttpContext.Request;

            string strFileId = request.Form["txtFileUpload_Id"];
            if (String.IsNullOrEmpty(strFileId))
            {
                fileUploadResult.Message = "参数不正确。";
                return fileUploadResult;
            }

            Guid fileId = Guid.Parse(strFileId);

            string cachingKey = "fsUploadResult:" + strFileId;

            if (_cachingService.ContainsKey(cachingKey))
            {
                fileUploadResult.Message = "请勿重复上传。";
                _cachingService.Set(cachingKey, fileUploadResult, _uploadResultExpiresIn);
                return fileUploadResult;
            }

            //鉴权
            string userId = request.Form["txtFileUpload_UserId"];
            if (_controlledCachingService.Contains(userId) == false)
            {
                fileUploadResult.Message = "用户鉴权失败。";
                _cachingService.Set(cachingKey, fileUploadResult, _uploadResultExpiresIn);
                return fileUploadResult;
            }

            if (request.Files.Count == 0)
            {
                fileUploadResult.Message = "没有要上传的文件。";
                _cachingService.Set(cachingKey, fileUploadResult, _uploadResultExpiresIn);
                return fileUploadResult;
            }

            if (request.Files.Count > 1)
            {
                fileUploadResult.Message = "一次只能上传一个文件。";
                _cachingService.Set(cachingKey, fileUploadResult, _uploadResultExpiresIn);
                return fileUploadResult;
            }

            HttpPostedFileBase file = request.Files[0];

            if (file.ContentLength / 1024 > 2048)
            {
                fileUploadResult.Message = String.Format("文件大小请勿超过：{0} KB", 2048);
                _cachingService.Set(cachingKey, fileUploadResult, _uploadResultExpiresIn);
                return fileUploadResult;
            }

            FileInfo fileInfo = new FileInfo(file.FileName);

            SaveArgs args = new SaveArgs();
            args.Id = fileId;
            args.Domain = Guid.Parse(request.Form["txtFileUpload_Domain"]);
            args.FileName = fileInfo.Name;
            args.FileExtension = fileInfo.Extension;
            //args.MD5 = request.Form["md5"];
            args.UploadDate = DateTime.Now;
            args.UploadIPAddress = Request.UserHostAddress;
            args.Stream = file.InputStream;

            //"D:\\WorkFloder\\Linkup\\trunk\\Source Code\\FileService\\Linkup.FileService\\File"
            //最后会有一个控制器的虚拟路径，所以 MapPath("/")
            //args.ServerRootDir = Server.MapPath("/");

            fileUploadResult = _fileManager.Save(args);

            _cachingService.Set(cachingKey, fileUploadResult, _uploadResultExpiresIn);

            return fileUploadResult;
        }

        /// <summary>
        /// 下载代理
        /// 微信服务器上的一些图片禁止直接引用，必须下载到本地
        /// 此处根据一个绝对URL地址进行下载
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadAgent()
        {
            FileDownloadAgentArgs args = RequestArgs<FileDownloadAgentArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }
            args.ServerRootDir = Server.MapPath("/");

            FileDownloadAgentResult result = _fileManager.Download(args);
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(result)
            };
        }

        /// <summary>
        /// 下载代理
        /// 将指定MediaId的文件下载到地文件服务器
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadAgentWithMediaId()
        {
            FileDownloadAgentWithMediaIdArgs args = RequestArgs<FileDownloadAgentWithMediaIdArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }
            args.ServerRootDir = Server.MapPath("/");

            DomainContext domainContext = _fileDomainPool.GetDomainContext(args.Domain);

            FileDownloadAgentWithMediaIdResult result = _fileManager.DownloadWithMediaId(domainContext, args);
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(result)
            };
        }

        public ActionResult DownloadQueueWithMediaId()
        {
            FileDownloadQueueWithMediaIdArgs args = RequestArgs<FileDownloadQueueWithMediaIdArgs>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            //_fileDownloadQueue.ServerRootDir = Server.MapPath("/");

            //DomainContext domainContext = _fileDomainPool.GetDomainContext(args.Domain);

            FileDownloadQueueEntity item = new FileDownloadQueueEntity();
            item.Domain = args.Domain;
            item.AppId = args.AppId;
            item.CreateTime = DateTime.Now;
            item.MediaId = args.MediaId;
            item.CallbackUrl = args.CallbackUrl;
            item.Tag = args.Tag;

            _fileDownloadQueue.Enqueue(item);

            return RespondResult();
        }
    }
}