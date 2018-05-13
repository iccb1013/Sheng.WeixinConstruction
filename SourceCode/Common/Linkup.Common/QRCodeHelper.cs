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


using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Linkup.Common
{
    public class QRCodeHelper
    {
        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="content">待编码的字符</param>
        /// <param name="ms">输出流</param>
        ///<returns>True if the encoding succeeded, false if the content is empty or too large to fit in a QR code</returns>
        public static bool GetQRCode(string content, MemoryStream ms)
        {
            ErrorCorrectionLevel ecl = ErrorCorrectionLevel.M; //误差校正水平 
            QuietZoneModules quietZones = QuietZoneModules.Two;  //空白区域 
            int moduleSize = 3;//大小
            var encoder = new QrEncoder(ecl);
            QrCode qr;
            if (encoder.TryEncode(content, out qr))//对内容进行编码，并保存生成的矩阵
            {
                var render = new GraphicsRenderer(new FixedModuleSize(moduleSize, quietZones));
                //render.WriteToStream(qr.Matrix, ImageFormat.Png, ms, new Point(72, 72));
                render.WriteToStream(qr.Matrix, ImageFormat.Png, ms);//默认96
            }
            else
            {
                return false;
            }
            return true;
        }

        public static Image GetQRCode(string content)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Image qrImage = null;
                if (QRCodeHelper.GetQRCode(content, ms) == false)
                {
                    return null;
                }
                qrImage = Image.FromStream(ms);  //8w232  7w203
                return qrImage;
            }
        }
    }
}
