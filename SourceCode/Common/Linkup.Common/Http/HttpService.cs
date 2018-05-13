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
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Web;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace Linkup.Common
{
    /*
     * http://www.cnblogs.com/jingmoxukong/p/4793643.html
     */

    public class HttpService
    {
        private static HttpService _instance;
        public static HttpService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new HttpService();
                return _instance;
            }
            set { _instance = value; }
        }

        private HttpService()
        {

        }

        public HttpRequestResult Request(HttpRequestArgs args)
        {
            HttpWebResponse response = null;
            Stream receiveStream = null;
            StreamReader readStream = null;

            MemoryStream postStream = new MemoryStream();

            HttpRequestResult result = new HttpRequestResult();

            string uri = args.Url;

            if (String.IsNullOrEmpty(uri))
            {
                result.Exception = new Exception("没有要请求的API URI信息");
                return result;
            }

            try
            {
                HttpWebRequest request;

                request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = args.Method;

                //判断是否使用证书
                if (args.UseCertificate)
                {
                    X509Certificate2 cert = new X509Certificate2(args.CertificatePath, args.CertificatePassword);
                    request.ClientCertificates.Add(cert);
                }

                //直接POST字符串内容，一般是JSON直接发到WEB API上
                if (String.IsNullOrEmpty(args.Content) == false)
                {
                    StreamWriter writer = new StreamWriter(postStream);
                    writer.Write(args.Content);
                    writer.Flush();                   
                }
                else if (String.IsNullOrEmpty(args.File) == false)
                {
                    RequestWithFile(request, postStream, args.File);
                }

                if (postStream.Length > 0)
                {
                    using (Stream stream = request.GetRequestStream())
                    {
                        postStream.WriteTo(stream);
                    }
                }

                response = (HttpWebResponse)request.GetResponse();

                receiveStream = response.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                readStream = new StreamReader(receiveStream, encode);

                result.Content = readStream.ReadToEnd();
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                result.Exception = ex;
            }
            finally
            {
                if (response != null)
                    response.Close();
                if (receiveStream != null)
                    receiveStream.Close();
                if (readStream != null)
                    readStream.Close();
                if (postStream != null)
                    postStream.Close();
            }

            return result;
        }

        private void RequestWithFile(HttpWebRequest request, MemoryStream postStream, string file)
        {
            string boundary = "----" + DateTime.Now.Ticks.ToString("x");
            string fileFormdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
            //string dataFormdataTemplate = "\r\n--" + boundary +
            //                              "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

            //media是微信定制需求
            //http://mp.weixin.qq.com/wiki/0/21cad57369e3652f6be542c1e3a531ec.html
            string formdata = string.Format(fileFormdataTemplate, "media", Path.GetFileName(file));

            var formdataBytes = Encoding.UTF8.GetBytes(postStream.Length == 0 ? formdata.Substring(2, formdata.Length - 2) : formdata);//第一行不需要换行
            postStream.Write(formdataBytes, 0, formdataBytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);

            byte[] buffer = new byte[1024];
            int bytesRead = 0;

            while (true)
            {
                bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;

                postStream.Write(buffer, 0, bytesRead);
            }

            postStream.Flush();

            fileStream.Close();
            fileStream.Dispose();

            //结尾
            var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            postStream.Write(footer, 0, footer.Length);

            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

            //request.ContentLength = memoryStream != null ? memoryStream.Length : 0;
            //request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            //request.KeepAlive = true;
            //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";

        }

        public HttpDownloadResult Download(HttpDownloadArgs args)
        {
            HttpDownloadResult result = new HttpDownloadResult();
            HttpWebRequest request;
            HttpWebResponse response = null;
            Stream receiveStream = null;
            FileStream fileStream = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(args.Url);
                request.Method = "GET";
                response = (HttpWebResponse)request.GetResponse();

                //因为微信上很多文件的地址是类似：
                //http://wx.qlogo.cn/mmopen/bFj91MgL38ds7PnEHJWDIPwHOvicfjnXc4IVDWvCia9FI06TibM2QbTicPt5ThWibIa8ObFxvdwEicqzsp1XcexaZr2uBf07Sjc75I/0
                //这样的形式，所以需要通过contentType判断文件的类型
                result.ContentType = response.Headers["Content-Type"];
                string fileExtension = String.Empty;
                switch (result.ContentType)
                {
                    case "image/jpeg":
                    case "image/jpg":
                    case "application/x-jpg":
                        fileExtension = ".jpg";
                        break;
                    case "audio/amr":
                        fileExtension = ".amr";
                        break;
                    case "video/mpeg4":
                        fileExtension = ".mp4";
                        break;
                    default:
                        break;
                }

                string storeFileName = Guid.NewGuid().ToString() + fileExtension;
                string outputFileName = Path.Combine(args.TargetDir, storeFileName);

                receiveStream = response.GetResponseStream();
                fileStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write);
               
                byte [] buffer = new byte[102400]; //100KB
                int readCount = receiveStream.Read(buffer, 0, buffer.Length);
                while (readCount > 0)
                {
                    fileStream.Write(buffer, 0, readCount);
                    fileStream.Flush();
                    readCount = receiveStream.Read(buffer, 0, buffer.Length);
                }

                result.Success = true;
                result.StoreFileName = storeFileName;
                result.FileLength = fileStream.Length;
            }
            catch(Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            finally
            {
                if (response != null)
                    response.Close();
                if (receiveStream != null)
                {
                    receiveStream.Close();
                    receiveStream.Dispose();
                }
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
            return result;
        }
    }
}
