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

namespace Linkup.Data
{
    /// <summary>
    /// 用这个对象可以避免sqlclient的引用被传递到各项目中
    /// </summary>
    public class CommandParameter
    {
        /// <summary>
        /// 要加 @
        /// </summary>
        public string ParameterName { get; set; }


        public object Value { get; set; }

        public CommandParameter()
        {

        }

        /// <summary>
        /// ParameterName 要加 @
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public CommandParameter(string name,object value)
        {
            ParameterName = name;
            if (value == null)
            {
                Value = DBNull.Value;
            }
            else
            {
                Value = value;
            }
        }

    }
}
