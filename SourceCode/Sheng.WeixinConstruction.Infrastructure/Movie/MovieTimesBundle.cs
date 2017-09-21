using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    public class MovieTimesBundle
    {
        public DateTime Date
        {
            get;
            set;
        }

        private List<MovieEntity> _movieList = new List<MovieEntity>();
        public List<MovieEntity> MovieList
        {
            get { return _movieList; }
            set { _movieList = value; }
        }
    }
}
