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

namespace Linkup.Common
{
    /// <summary>
    /// 允许对象返回自己的浅表拷贝
    /// </summary>
    public interface ISnapshot
    {
        /// <summary>
        /// 创建当前 System.Object 的浅表副本
        /// 实现时直接调用 Object.MemberwiseClone() 方法即可
        /// </summary>
        /// <returns></returns>
        object Copy();

        /// <summary>
        /// 快照
        /// 创建当前 System.Object 的浅表副本
        /// </summary>
        void Snapshot();

        /// <summary>
        /// 接受更改
        /// 将创建新的对象快照，丢弃调用此方法之前的镜像
        /// </summary>
        void AcceptChange();

        /// <summary>
        /// 恢复到上次快照时的状态
        /// </summary>
        void Revert();
    }
}
