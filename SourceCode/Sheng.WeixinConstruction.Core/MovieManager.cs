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
    public class MovieManager
    {
        private static readonly MovieManager _instance = new MovieManager();
        public static MovieManager Instance
        {
            get { return _instance; }
        }

        private DatabaseWrapper _dataBase = ServiceUnity.Instance.Database;
        private LogService _log = LogService.Instance;

        private object _settingsLockObj = new object();

        private MovieManager()
        {

        }

        public MovieSettingsEntity GetSettings(Guid domainId, string appId)
        {
            if (String.IsNullOrEmpty(appId))
                return null;

            MovieSettingsEntity settings = new MovieSettingsEntity();
            settings.Domain = domainId;
            settings.AppId = appId;
            if (_dataBase.Fill<MovieSettingsEntity>(settings) == false)
            {
                lock (_settingsLockObj)
                {
                    if (_dataBase.Fill<MovieSettingsEntity>(settings) == false)
                    {
                        //初始化一条默认设置
                        _dataBase.Insert(settings);
                    }
                }
            }

            return settings;
        }

        public NormalResult SaveSettings(Guid domainId, MovieSettingsEntity args)
        {
            if (args == null)
            {
                Debug.Assert(false, "args 为空");
                return new NormalResult("参数错误");
            }

            SqlStructureBuild sqlBuild = new SqlStructureBuild();
            sqlBuild.Table = "MovieSettings";
            sqlBuild.Type = SqlExpressionType.Update;
            sqlBuild.AddParameter("Domain", args.Domain, true);
            sqlBuild.AddParameter("AppId", args.AppId, true);
            sqlBuild.AddParameter("ShareImageUrl", args.ShareImageUrl);
            sqlBuild.AddParameter("ShareTimelineTitle", args.ShareTimelineTitle);
            sqlBuild.AddParameter("ShareAppMessageTitle", args.ShareAppMessageTitle);
            sqlBuild.AddParameter("ShareAppMessageDescription", args.ShareAppMessageDescription);
            _dataBase.ExcuteSqlExpression(sqlBuild.GetSqlExpression());

            return new NormalResult();

        }

        /// <summary>
        /// 获取待同步列表
        /// </summary>
        /// <returns></returns>
        public List<MovieSettingsEntity> GetPrepareSyncSettingsList()
        {
            List<MovieSettingsEntity> settingsList = _dataBase.Select<MovieSettingsEntity>(
                "SELECT * FROM [MovieSettings] WHERE [MTimeUrl] IS NOT NULL AND [MTimeUrl] <> ''");
            return settingsList;
        }

        public void Sync(Guid domainId, string appId, MovieTimesBundle bundle)
        {
            if (bundle == null)
                return;

            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));

            _dataBase.ExecuteNonQuery(
                "UPDATE [MovieSettings] SET [SyncTime] = GETDATE() WHERE [Domain] = @domainId AND [AppId] = @appId",
                parameterList);

            parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@date", bundle.Date));
            _dataBase.ExecuteNonQuery(
               "DELETE FROM [MoviePlan] WHERE [Domain] = @domainId AND [AppId] = @appId AND [Date] = @date",
               parameterList);

            _dataBase.ExecuteNonQuery(
              "DELETE FROM [MovieTimes] WHERE [Domain] = @domainId AND [AppId] = @appId AND CONVERT(VARCHAR(10),[Time],120) = CONVERT(VARCHAR(10),@date,120)",
              parameterList);

            int i = 0;
            foreach (MovieEntity movie in bundle.MovieList)
            {
                i++;

                parameterList = new List<CommandParameter>();
                parameterList.Add(new CommandParameter("@mtimeId", movie.MtimeId));

                List<MovieEntity> movieList = _dataBase.Select<MovieEntity>(
                    "SELECT * FROM [Movie] WHERE [MtimeId] = @mtimeId", parameterList);
                if (movieList.Count == 0)
                {
                    _dataBase.Insert(movie);
                }
                else
                {
                    movie.Id = movieList[0].Id;
                }

                MoviePlanEntity plan = new MoviePlanEntity();
                plan.Domain = domainId;
                plan.AppId = appId;
                plan.Date = bundle.Date;
                plan.Movie = movie.Id;
                plan.Sort = i;
                _dataBase.Insert(plan);

                foreach (MovieTimesEntity item in movie.TimesList)
                {
                    item.Domain = domainId;
                    item.AppId = appId;
                    item.Movie = movie.Id;
                    _dataBase.Insert(item);
                }
            }

        }

        public List<MovieEntity> GetMovieTimes(Guid domainId, string appId, DateTime date)
        {
            List<CommandParameter> parameterList = new List<CommandParameter>();
            parameterList.Add(new CommandParameter("@domainId", domainId));
            parameterList.Add(new CommandParameter("@appId", appId));
            parameterList.Add(new CommandParameter("@date", date));

            DataSet dsResult =
                _dataBase.ExecuteDataSet(CommandType.StoredProcedure, "GetMovieTimes", parameterList, new string[] { "movies", "times" });

            DataTable dtMovies = dsResult.Tables["movies"];
            DataTable dtTimes = dsResult.Tables["times"];

            List<MovieEntity> list = RelationalMappingUnity.Select<MovieEntity>(dtMovies);

            if (list == null)
            {
                list = new List<MovieEntity>();
                return list;
            }

            foreach (MovieEntity item in list)
            {
                item.TimesList = RelationalMappingUnity.Select<MovieTimesEntity>(dtTimes.Select("Movie = '" + item.Id + "'"));
            }

            return list;

        }
    }
}
