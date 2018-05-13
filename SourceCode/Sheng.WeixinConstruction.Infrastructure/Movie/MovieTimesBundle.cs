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
