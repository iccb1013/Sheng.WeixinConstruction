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
    /// <summary>
    /// 只对插入数据，更新数据影响
    /// SELECT 数据时不考虑此属性
    /// </summary>
    public class NotMappedAttribute : RelationalMappingAttribute
    {
    }
}
