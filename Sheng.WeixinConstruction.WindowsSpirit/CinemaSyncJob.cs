using Linkup.Common;
using Sheng.WeixinConstruction.Core;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sheng.WeixinConstruction.WindowsSpirit
{
    /// <summary>
    /// 爬虫不能放到Quartz中执行，因为WebBrowser必须工作在单线程单元中
    /// </summary>
    class CinemaSyncJob //: IJob
    {
        private static readonly LogService _log = LogService.Instance;
        private static readonly MovieManager _movieManager = MovieManager.Instance;

        private CinemaSpider _spider;

        /// <summary>
        /// 同步队列
        /// 因为 _webBrowser_DocumentCompleted 是异步的，所以必须通过事件回调来判断是否执行完毕
        /// </summary>
        private Queue<MovieSettingsEntity> _pendingSync = new Queue<MovieSettingsEntity>();

        private bool _running = false;

        private Regex _mTimeUrlRegex = new Regex(@"^http://theater.mtime.com/\w+/\d+/$");

        public event EventHandler Running;
        public event EventHandler End;

        public void Execute()
        {
            if (_running)
                return;

            _running = true;

            //_log.Write("CinemaSyncJob Execute", DateTime.Now.ToString());

            try
            {
                LoadSettingsQueue();
            }
            catch (Exception ex)
            {
                _log.Write("同步电影排片数据异常", ex.Message, TraceEventType.Error);
            }
            finally
            {

            }

            //Thread.Sleep(1000 * 60 * 30);
        }

        private void LoadSettingsQueue()
        {
            //_log.Write("开始同步电影排片数据", String.Empty, TraceEventType.Verbose);

            if (Running != null)
                Running(this, new EventArgs());

            List<MovieSettingsEntity> settingsList = _movieManager.GetPrepareSyncSettingsList();
            if (settingsList == null)
                return;

            _log.Write("获取待同步 MovieSettingsEntity：", settingsList.Count.ToString(), TraceEventType.Verbose);

            foreach (var item in settingsList)
            {
                if (String.IsNullOrEmpty(item.MTimeUrl))
                    continue;

                //^http://theater.mtime.com/\w+/\d+/$
                //http://theater.mtime.com/China_Anhui_Province_Chuzhou_LangYaQu/8372/
                if (_mTimeUrlRegex.IsMatch(item.MTimeUrl) == false)
                    continue;

                if (item.SyncTime != null && (DateTime.Now - item.SyncTime.Value).TotalMinutes <= 60)
                    continue;

                _pendingSync.Enqueue(item);
            }

            SyncNext();
        }

        private void SyncNext()
        {
            if (_pendingSync.Count == 0)
            {
                if (End != null)
                    End(this, new EventArgs());
                _running = false;
                return;
            }

            MovieSettingsEntity settings = _pendingSync.Peek();

            Sync(settings);
        }

        private void Sync(MovieSettingsEntity settings)
        {
            _log.Write("同步 MovieSettingsEntity：", JsonHelper.Serializer(settings), TraceEventType.Verbose);

            _spider = new CinemaSpider();
            _spider.Complete += spider_Complete;
            _spider.GetFromMtime(settings);
        }

        void spider_Complete(object sender, CinemaSpiderCompleteEventArgs e)
        {
            CinemaSpider spider = (CinemaSpider)sender;
            spider.Dispose();

            foreach (var bundle in e.MovieTimesBundleList)
            {
                _movieManager.Sync(e.Settings.Domain, e.Settings.AppId, bundle);
            }

            _pendingSync.Dequeue();
            SyncNext();
        }

    }
}
