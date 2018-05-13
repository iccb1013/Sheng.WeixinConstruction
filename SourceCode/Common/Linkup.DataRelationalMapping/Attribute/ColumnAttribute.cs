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

namespace Linkup.DataRelationalMapping
{
    public class ColumnAttribute : RelationalMappingAttribute
    {
        public string Name
        {
            get;
            private set;
        }

        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }
}
