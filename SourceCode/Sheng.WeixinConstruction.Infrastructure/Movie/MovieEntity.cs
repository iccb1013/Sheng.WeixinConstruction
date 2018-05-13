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


using Linkup.DataRelationalMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.WeixinConstruction.Infrastructure
{
    [Table("Movie")]
    public class MovieEntity
    {
        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get;
            set;
        }

        public string Image
        {
            get;
            set;
        }

        /// <summary>
        /// 导演
        /// </summary>
        public string Director
        {
            get;
            set;
        }

        /// <summary>
        /// 主演
        /// </summary>
        public string Actor
        {
            get;
            set;
        }

        /// <summary>
        /// 时长
        /// </summary>
        public string Time
        {
            get;
            set;
        }

        /// <summary>
        /// 类型：爱情/剧情/科幻
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// 时光网ID
        /// </summary>
        public int MtimeId
        {
            get;
            set;
        }

        private List<MovieTimesEntity> _timesList = new List<MovieTimesEntity>();
        [NotMapped]
        public List<MovieTimesEntity> TimesList
        {
            get { return _timesList; }
            set { _timesList = value; }
        }

        private List<string> _showTypeList = null;
        [NotMapped]
        public List<string> ShowTypeList
        {
            get
            {
                if(_showTypeList==null)
                {
                    _showTypeList = new List<string>();
                    foreach (var item in _timesList)
                    {
                        if (_showTypeList.Contains(item.ShowType) == false)
                            _showTypeList.Add(item.ShowType);
                    }
                    _showTypeList = _showTypeList.OrderBy((s) => { return s; }).ToList();
                }

                return _showTypeList;
            }
        }
    }
}
