using Sheng.WeixinConstruction.Infrastructure;
using Sheng.WeixinConstruction.WeixinContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheng.WeixinConstruction.Client.Shell.Models
{
    public class MovieTimesViewModel
    {
        public DateTime Date
        {
            get;
            set;
        }

        public List<MovieEntity> MovieList
        {
            get;
            set;
        }

        public MovieSettingsEntity Settings
        {
            get;
            set;
        }

        public WeixinJsApiConfig JsApiConfig
        {
            get;
            set;
        }
    }
}