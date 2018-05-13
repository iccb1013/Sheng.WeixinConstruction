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


using Linkup.Data;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class ShareManager
    {
        private static readonly ShareManager _instance = new ShareManager();
        public static ShareManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;

        private ShareManager()
        {

        }

        public ShareLogEntity GetShareLog(Guid pageId, string openId)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@pageId", pageId));
            parameterList.Add(new CommandParameter("@openId", openId));

            List<ShareLogEntity> logList = _dataBase.Select<ShareLogEntity>(
                "SELECT * FROM [ShareLog] WHERE [OpenId] = @openId AND [PageId] = @pageId",
                parameterList);

            if (logList.Count == 0)
                return null;
            else
                return logList[0];
        }

        public void Create(ShareLogEntity log)
        {
            if (log == null)
                return;

            _dataBase.Insert(log);
        }

        public void Update(ShareLogEntity log)
        {
            if (log == null)
                return;

            _dataBase.Update(log);
        }
    }
}
