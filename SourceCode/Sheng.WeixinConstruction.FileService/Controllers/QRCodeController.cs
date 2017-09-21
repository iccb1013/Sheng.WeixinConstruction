using Linkup.Common;
using Newtonsoft.Json;
using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.FileService.Controllers
{
    /// <summary>
    /// 生成指向URL的二维码
    /// </summary>
    public class QRCodeController : BasalController
    {
        public ActionResult GetQRCodeImage()
        {
            GetQRCodeImageArgs args = RequestArgs<GetQRCodeImageArgs>();
            GetQRCodeImageResult result = new GetQRCodeImageResult();
            if (args == null || String.IsNullOrEmpty(args.Content))
            {
                result.Message = "参数无效。";
                return new ContentResult()
                {
                    Content = JsonConvert.SerializeObject(result)
                };
            }

            Image qrImage = null;

            try
            {
                qrImage = QRCodeHelper.GetQRCode(args.Content);

                if (qrImage == null)
                {
                    result.Message = "二维码生成失败。";
                    return new ContentResult()
                    {
                        Content = JsonConvert.SerializeObject(result)
                    };
                }

                string targetDir = Path.Combine(Server.MapPath("/"), "FileStore", args.Domain.ToString());
                string storeFileName = Guid.NewGuid().ToString() + ".jpg";
                string outputFileName = Path.Combine(targetDir, storeFileName);
                qrImage.Save(outputFileName, ImageFormat.Jpeg);

                result.Success = true;
                result.FileName = String.Format("FileStore/{0}/{1}", args.Domain, storeFileName);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            finally
            {
                if (qrImage != null)
                    qrImage.Dispose();
            }


            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(result)
            };

        }
    }
}