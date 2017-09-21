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

                if (String.IsNullOrEmpty(args.Content) == false)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(memoryStream);
                    writer.Write(args.Content);
                    writer.Flush();

                    using (Stream stream = request.GetRequestStream())
                    {
                        memoryStream.WriteTo(stream);
                    }
                }
                else if (String.IsNullOrEmpty(args.File) == false)
                {
                    //request.Headers.Add("name", "media");
                    //request.Headers.Add("filename", "a.jpg");
                    request.ContentType = "application/octet-stream";

                    MemoryStream memoryStream = new MemoryStream();
                    //StreamWriter writer = new StreamWriter(memoryStream);

                    // 边界符  
                    var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
                    // 边界符  
                    var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");  

                    const string filePartHeader =
                        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                        "Content-Type: application/octet-stream\r\n\r\n";
                    var header = string.Format(filePartHeader, "media", args.File);
                    var headerbytes = Encoding.UTF8.GetBytes(header);

                    memoryStream.Write(beginBoundary, 0, beginBoundary.Length);  
                    memoryStream.Write(headerbytes, 0, headerbytes.Length);  

                    FileStream fileStream = new FileStream(args.File, FileMode.Open, FileAccess.Read);

                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;

                    while (true)
                    {
                        bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break;

                        memoryStream.Write(buffer, 0, bytesRead);
                    }

                    memoryStream.Flush();

                    using (Stream stream = request.GetRequestStream())
                    {
                        memoryStream.WriteTo(stream);
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
            }

            return result;
        }

    }
}
