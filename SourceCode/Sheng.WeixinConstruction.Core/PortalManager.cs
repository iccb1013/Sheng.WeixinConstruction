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
using Linkup.Data;
using Linkup.DataRelationalMapping;
using Newtonsoft.Json.Linq;
using Sheng.WeixinConstruction.ApiContract;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Core
{
    public class PortalManager
    {
        private static readonly PortalManager _instance = new PortalManager();
        public static PortalManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;
        private PortalTemplatePool _portalTemplatePool = PortalTemplatePool.Instance;
        private DomainManager _domainManager = DomainManager.Instance;


        private object _settingsLockObj = new object();

        private PortalManager()
        {

        }

        #region 微主页

        public PortalStyleSettingsEntity GetPortalStyleSettings(Guid domainId, string appId)
        {
            if (String.IsNullOrEmpty(appId))
                return null;

            PortalStyleSettingsEntity settings = new PortalStyleSettingsEntity();
            settings.Domain = domainId;
            settings.AppId = appId;
            if (_dataBase.Fill<PortalStyleSettingsEntity>(settings) == false)
            {
                lock (_settingsLockObj)
                {
                    if (_dataBase.Fill<PortalStyleSettingsEntity>(settings) == false)
                    {
                        //初始化一条默认设置
                        _dataBase.Insert(settings);
                    }
                }
            }

            //不管模式是不是模版，只要有值，就取出来
            //避免从自定义切回模版画面时不显示名称和说明
            if (settings.PortalPresetTemplateId.HasValue)
            {
                settings.PortalPresetTemplate =
                       _portalTemplatePool.GetPortalPresetTemplate(settings.PortalPresetTemplateId.Value);
            }
            else
            {
                if (settings.PortalMode == EnumPortalMode.Template)
                {
                    settings.PortalPresetTemplate = _portalTemplatePool.GetDefaultPortalPresetTemplate();
                    settings.PortalPresetTemplateId = settings.PortalPresetTemplate.Id;
                }
            }

            return settings;
        }

        /// <summary>
        /// 保存样式设置，但是不包括 PortalCustomTemplate
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="args"></param>
        public void SavePortalStyleSettings_Template(Guid domainId, PortalStyleSettingsEntity args)
        {
            if (args == null)
            {
                Debug.Assert(false, "PortalStyleSettingsEntity 为空");
                return;
            }

            //_dataBase.Update(args);
            //不能直接update，会把 PortalCustomTemplate 覆盖掉

            SqlStructureBuild sqlBuild = new SqlStructureBuild();
            sqlBuild.Table = "StyleSettings";
            sqlBuild.Type = SqlExpressionType.Update;
            sqlBuild.AddParameter("Domain", args.Domain, true);
            sqlBuild.AddParameter("AppId", args.AppId, true);
            sqlBuild.AddParameter("PortalMode", args.PortalMode);
            sqlBuild.AddParameter("PortalImageUrl", args.PortalImageUrl);
            sqlBuild.AddParameter("PortalPresetTemplateId", args.PortalPresetTemplateId);
            _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());

            _domainManager.UpdateLastUpdateTime(domainId);
        }

        public void SavePortalStyleSettings_Custom(Guid domainId, PortalStyleSettingsEntity args)
        {
            if (args == null)
            {
                Debug.Assert(false, "PortalStyleSettingsEntity 为空");
                return;
            }

            //_dataBase.Update(args);
            //不能直接update，会把 PortalCustomTemplate 覆盖掉

            SqlStructureBuild sqlBuild = new SqlStructureBuild();
            sqlBuild.Table = "StyleSettings";
            sqlBuild.Type = SqlExpressionType.Update;
            sqlBuild.AddParameter("Domain", args.Domain, true);
            sqlBuild.AddParameter("AppId", args.AppId, true);
            sqlBuild.AddParameter("PortalMode", args.PortalMode);
            sqlBuild.AddParameter("PortalCustomTemplate", args.PortalCustomTemplate);
            _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());

            _domainManager.UpdateLastUpdateTime(domainId);
        }

        #endregion

        #region 快捷菜单

        public ShortcutMenuEntity GetShortcutMenu(Guid domainId, string appId)
        {
            ShortcutMenuEntity menuEntity = new ShortcutMenuEntity();
            menuEntity.Domain = domainId;
            menuEntity.AppId = appId;
            if (_dataBase.Fill<ShortcutMenuEntity>(menuEntity))
                return menuEntity;
            else
                return null;
        }

        public void SaveShortcutMenu(string json, UserContext userContext, DomainContext domainContext)
        {
            ShortcutMenuEntity menuEntity = new ShortcutMenuEntity();
            menuEntity.Domain = domainContext.Domain.Id;
            menuEntity.AppId = domainContext.AppId;
            menuEntity.Menu = json;
            _dataBase.Remove(menuEntity);
            _dataBase.Insert(menuEntity);
        }

        #endregion
    }
}
