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


using Linkup.Common;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace Sheng.WeixinConstruction.Client.Core
{
    public class SessionPool
    {
        private static readonly SessionPool _instance = new SessionPool();
        public static SessionPool Instance
        {
            get { return _instance; }
        }

        private static readonly LogService _log = LogService.Instance;

        private Hashtable _sessionTable = new Hashtable();

        private object _lockObj = new object();

        private SessionPool()
        {

        }

        public void Set(string openId, HttpSessionStateBase session)
        {
            if (session == null)
                return;

            _log.Write("SessionPool.Set", "OpenId：" + openId, TraceEventType.Verbose);

            lock (_lockObj)
            {
                if (_sessionTable.ContainsKey(openId))
                {
                    _sessionTable[openId] = session;
                }
                else
                {
                    _sessionTable.Add(openId, session);
                }
            }
        }

        public HttpSessionStateBase Get(string openId)
        {
            if (String.IsNullOrEmpty(openId))
                return null;

            HttpSessionStateBase session = _sessionTable[openId] as HttpSessionStateBase;

            return session;
        }

        public void Abandon(string openId)
        {
            _log.Write("SessionPool.Abandon", "OpenId：" + openId, TraceEventType.Verbose);

            if (String.IsNullOrEmpty(openId))
                return;

            HttpSessionStateBase session = _sessionTable[openId] as HttpSessionStateBase;
            if (session == null)
                return;

            session.Clear();
            session.Abandon();

            lock (_lockObj)
            {
                _sessionTable.Remove(openId);
            }

            _log.Write("SessionPool.Abandon Done", "OpenId：" + openId, TraceEventType.Verbose);
        }
    }
}
