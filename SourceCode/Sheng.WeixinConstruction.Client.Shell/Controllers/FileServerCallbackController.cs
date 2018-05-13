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


using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sheng.WeixinConstruction.Client.Shell.Controllers
{
    public class FileServerCallbackController : BasalController
    {
        private static readonly MessageManager _messageManager = MessageManager.Instance;

        /// <summary>
        /// 消息文件
        /// </summary>
        /// <returns></returns>
        public ActionResult MessageFile()
        {
            FileDownloadQueueWithMediaIdResult args = RequestArgs<FileDownloadQueueWithMediaIdResult>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            _messageManager.MessageFile(args);

            return RespondResult();
        }

        /// <summary>
        /// 视频消息文件的缩略图
        /// </summary>
        /// <returns></returns>
        public ActionResult MessageThumbFile()
        {
            FileDownloadQueueWithMediaIdResult args = RequestArgs<FileDownloadQueueWithMediaIdResult>();
            if (args == null)
            {
                return RespondResult(false, "参数无效。");
            }

            _messageManager.MessageThumbFile(args);

            return RespondResult();
        }
	}
}