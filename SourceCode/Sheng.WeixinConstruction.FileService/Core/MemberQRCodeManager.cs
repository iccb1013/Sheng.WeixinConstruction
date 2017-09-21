using Linkup.Common;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.FileService
{
    public class MemberQRCodeManager
    {
        private static readonly MemberQRCodeManager _instance = new MemberQRCodeManager();
        public static MemberQRCodeManager Instance
        {
            get { return _instance; }
        }

        private static readonly MemberManager _memberManager = MemberManager.Instance;
        private static readonly FileManager _fileManager = FileManager.Instance;

        private MemberQRCodeManager()
        {
        }

        internal GetMemberQRCodeImageResult GetMemberQRCodeImage(string serverRootDir, GetCampaign_MemberQRCodeImageArgs args)
        {
            GetMemberQRCodeImageResult result = new GetMemberQRCodeImageResult();
            if (args == null || String.IsNullOrEmpty(args.LandingUrl))
            {
                result.Message = "参数不正确";
                return result;
            }

            MemberEntity member = _memberManager.GetMember(args.MemberId);
            if (member == null)
            {
                result.Message = "会员信息不存在";
                return result;
            }

            FileRecord fileRecord = _fileManager.Get(args.BackgroundImageId);
            if (fileRecord == null)
            {
                result.Message = "背景图信息不存在";
                return result;
            }

            string backgroundImageFile =
                Path.Combine(serverRootDir, String.Format("FileStore/{0}/{1}", args.Domain, fileRecord.StoredFileName));

            if (File.Exists(backgroundImageFile) == false)
            {
                result.Message = "背景图文件不存在";
                return result;
            }

            FileDownloadAgentArgs downloadArgs = new FileDownloadAgentArgs();
            downloadArgs.Domain = args.Domain;
            downloadArgs.Url = member.Headimgurl_96;
            downloadArgs.ServerRootDir = serverRootDir;

            FileDownloadAgentResult downloadResult = _fileManager.Download(downloadArgs);
            if (downloadResult.Success == false)
            {
                result.Message = downloadResult.Message;
                return result;
            }

            string headImageFile = Path.Combine(serverRootDir, downloadResult.OutputFile);

            Image qrImage = null;
            Image backgroundImage = null;
            Image headImage = null;

            try
            {
                qrImage = QRCodeHelper.GetQRCode(args.LandingUrl);

                if (qrImage == null)
                {
                    result.Message = "二维码生成失败。";
                    return result;
                }

                backgroundImage = Image.FromFile(backgroundImageFile);
                Graphics g = Graphics.FromImage(backgroundImage);

                //qrImage
                float x = backgroundImage.Width - qrImage.Width - 20;
                float y = backgroundImage.Height - qrImage.Height - 20;
                g.DrawImage(qrImage, x, y);
                
                //headImage
                x = 20;
                y = backgroundImage.Height - 20 - 96;
                headImage = Image.FromFile(headImageFile);
                g.DrawImage(headImage, x, y, 96, 96);

                ////nickName
                //Font font = new Font("黑体", 35f);
                //x = 96 + 40;
                //y = backgroundImage.Height - font.Size - 50;
                //g.DrawString(member.NickName, font, Brushes.Black, new PointF(x, y));

                g.Save();

                string targetDir = Path.Combine(serverRootDir, "FileStore", args.Domain.ToString());
                string storeFileName = Guid.NewGuid().ToString() + ".jpg";
                string outputFileName = Path.Combine(targetDir, storeFileName);
                backgroundImage.Save(outputFileName, ImageFormat.Jpeg);

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

                if (backgroundImage != null)
                    backgroundImage.Dispose();

                if (headImage != null)
                    headImage.Dispose();
            }

            return result;
        }
      
    }
}