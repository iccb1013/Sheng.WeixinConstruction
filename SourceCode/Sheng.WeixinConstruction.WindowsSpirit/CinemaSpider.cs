using Linkup.Common;
using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sheng.WeixinConstruction.WindowsSpirit
{
    class CinemaSpider// : IDisposable
    {
        //这个蜘蛛没法放到windows服务中
        //按下面的方法用一个单独的线程STAMeathed方法可以运行，但是_webBrowser_DocumentCompleted不执行
        //怀疑是因为调用了GetFromMtime方法后线程直接就结束了，因为_webBrowser_DocumentCompleted事件是异步的
        //其他信息: 当前线程不在单线程单元中，因此无法实例化 ActiveX 控件“8856f961-340a-11d0-a96b-00c04fd705a2”。
        //http://blog.csdn.net/xuehuic/article/details/6426284
        //C＃Webbrowser中屏蔽弹出窗口及脚本错误提示
        //http://blog.csdn.net/wonsoft/article/details/5197015
        private WebBrowser _webBrowser = new WebBrowser();

        private MovieSettingsEntity _settings;
        private string _nextDate;
        private List<MovieTimesBundle> _bundleList = new List<MovieTimesBundle>();
        private List<string> _dateList = new List<string>();

        public event EventHandler<CinemaSpiderCompleteEventArgs> Complete;

        private static ExceptionHandlingService _exceptionHandling = ServiceUnity.Instance.ExceptionHandling;

        private int _count = 0;  

        public CinemaSpider()
        {
            _webBrowser.ScriptErrorsSuppressed = true;
            
            _webBrowser.Navigated += _webBrowser_Navigated;
            _webBrowser.DocumentCompleted += _webBrowser_DocumentCompleted;
        }

        private void _webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            _count++;
        }

        private void _webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            _count--;

            if (_count > 0)
                return;

            try
            {

                //mshtml.IHTMLDocument2 htmlDoc = _webBrowser.Document as mshtml.IHTMLDocument2;
                HtmlDocument doc = _webBrowser.Document;
                HtmlElement movieItemListRegion = doc.GetElementById("movieItemListRegion");
                if (movieItemListRegion == null)
                    return;

                MovieTimesBundle bundle = new MovieTimesBundle();

                _nextDate = null;

                HtmlElement valueDateRegion = doc.GetElementById("valueDateRegion");
                HtmlElementCollection dateLiCollection = valueDateRegion.GetElementsByTagName("li");
                for (int i = 0; i < dateLiCollection.Count; i++)
                {
                    HtmlElement dateLiItem = dateLiCollection[i];
                    if (dateLiItem.OuterHtml.Contains("class=curr"))
                    {
                        //20160304
                        string dateStr = dateLiItem.GetAttribute("_date");
                        if (_dateList.Contains(dateStr))
                            return;

                        _dateList.Add(dateStr);

                        bundle.Date = new DateTime(
                            int.Parse(dateStr.Substring(0, 4)), int.Parse(dateStr.Substring(4, 2)), int.Parse(dateStr.Substring(6, 2)));

                        if (i < dateLiCollection.Count - 1)
                        {
                            _nextDate = dateLiCollection[i + 1].GetElementsByTagName("a")[0].GetAttribute("href");
                        }
                        break;
                    }
                }

                HtmlElementCollection listBCollection = movieItemListRegion.GetElementsByTagName("b");
                foreach (HtmlElement listBItem in listBCollection)
                {
                    MovieEntity movie = new MovieEntity();
                    movie.MtimeId = int.Parse(listBItem.GetAttribute("_movieid"));
                    movie.Name = listBItem.GetElementsByTagName("a")[0].GetAttribute("title");
                    movie.Image = listBItem.GetElementsByTagName("img")[0].GetAttribute("src");

                    //模拟点击
                    mshtml.IHTMLElement movieItem = (mshtml.IHTMLElement)listBItem.DomElement;
                    movieItem.click();

                    //详情
                    HtmlElement movieDetailRegion = doc.GetElementById("movieDetailRegion");
                    HtmlElementCollection detailPCollection = movieDetailRegion.GetElementsByTagName("p");

                    HtmlElementCollection detailSpan = detailPCollection[0].GetElementsByTagName("span");
                    if (detailSpan.Count == 1)
                    {
                        movie.Director = detailSpan[0].InnerText.Replace("导演：", "");
                    }
                    else if (detailSpan.Count == 2)
                    {
                        movie.Director = detailSpan[0].InnerText.Replace("导演：", "");
                        movie.Actor = detailSpan[1].InnerText.Replace("主演：", ""); //主演有可能没有，如动画片
                    }

                    detailSpan = detailPCollection[1].GetElementsByTagName("span");
                    if (detailSpan.Count == 1)
                    {
                        movie.Type = detailSpan[0].InnerText.Replace("类型：", "");
                    }
                    else if (detailSpan.Count == 2)
                    {
                        movie.Time = detailSpan[0].InnerText.Replace("时长：", ""); //时长有可能没有
                        movie.Type = detailSpan[1].InnerText.Replace("类型：", "");
                    }

                    //场次 
                    HtmlElement showtimesRegion = doc.GetElementById("showtimesRegion");
                    HtmlElementCollection showtimeTRCollection =
                        showtimesRegion.GetElementsByTagName("tr");
                    foreach (HtmlElement trItem in showtimeTRCollection)
                    {
                        MovieTimesEntity timesEntity = new MovieTimesEntity();
                        timesEntity.Movie = movie.Id;

                        HtmlElementCollection tdList = trItem.GetElementsByTagName("td");

                        HtmlElementCollection pList = tdList[0].GetElementsByTagName("p");

                        //0点首映的片子，排片表第一行是“次日场”三个字
                        if (pList.Count == 0)
                            continue;

                        timesEntity.Time = DateTime.Parse(bundle.Date.ToShortDateString() + " " + pList[0].InnerText);
                        timesEntity.EndTime = pList[1].InnerText;

                        pList = tdList[1].GetElementsByTagName("p");
                        timesEntity.ShowType = pList[0].InnerText;
                        timesEntity.Language = pList[1].InnerText;

                        pList = tdList[2].GetElementsByTagName("p");
                        timesEntity.ScreeningRoom = pList[0].InnerText;

                        movie.TimesList.Add(timesEntity);
                    }

                    bundle.MovieList.Add(movie);
                }

                _bundleList.Add(bundle);

            }
            catch (Exception ex)
            {
                _exceptionHandling.HandleException(ex);
                return;
            }

            if (String.IsNullOrEmpty(_nextDate) == false)
            {
                _webBrowser.Navigate(_nextDate);
            }
            else
            {
                _webBrowser.DocumentCompleted -= _webBrowser_DocumentCompleted;

                if (Complete != null)
                {
                    CinemaSpiderCompleteEventArgs args = new CinemaSpiderCompleteEventArgs()
                    {
                        Settings = _settings,
                        MovieTimesBundleList = _bundleList
                    };
                    Complete(this, args);
                }
            }
        }

        public void GetFromMtime(MovieSettingsEntity settings)
        {
            _settings = settings;

            _webBrowser.Navigate(settings.MTimeUrl);
        }

        public void Dispose()
        {
            _webBrowser.Stop();
            _webBrowser.Dispose();
        }
    }

    class CinemaSpiderCompleteEventArgs
    {
        public MovieSettingsEntity Settings
        {
            get;
            set;
        }

        public List<MovieTimesBundle> MovieTimesBundleList
        {
            get;
            set;
        }
    }


}
