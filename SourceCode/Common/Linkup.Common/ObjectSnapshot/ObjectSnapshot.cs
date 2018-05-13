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
using System.Diagnostics;
using Linkup.Common;

namespace Linkup.Common
{
    /// <summary>
    /// 对象快照
    /// </summary>
    public class ObjectSnapshot
    {
        private object _object;
        private object _mirrorObject;

        private ISnapshot _snapShot;

        /// <summary>
        /// obj 必须实现 ISnapshot 接口
        /// </summary>
        /// <param name="snapshot"></param>
        public ObjectSnapshot(ISnapshot snapshot)
        {
            _snapShot = snapshot;

            if (_snapShot == null)
            {
                Debug.Assert(false, "snapshot 为 null ");
                throw new ArgumentNullException();
            }

            _object = snapshot;
        }

        /// <summary>
        /// 创建对象快照
        /// </summary>
        public void Snapshot()
        {
            if (_snapShot == null)
                return;

            _mirrorObject = _snapShot.Copy();
        }

        /// <summary>
        /// 撤销对传入的 obj 对象的所有更改
        /// 还原之后会把快照给清除掉
        /// </summary>
        public void Revert()
        {
            if (_mirrorObject == null || _object == null)
                return;

            SnapshotMemberCollection members = new SnapshotMemberCollection();
            foreach (ObjectCompareResult result in ObjectCompare.Compare(_mirrorObject, _object))
            {
                members.Add(result.MemberName, result.SourceValue, result.CompareValue);
            }

            foreach (SnapshotMember member in members)
            {
                member.SetMember(_object, SnapshotMember.EnumMemberValue.OldValue);
            }

            _mirrorObject = null;
        }

        /// <summary>
        /// 接受传入对象 obj 的所有更改
        /// 将创建新的对象快照，丢弃调用此方法之前的镜像
        /// </summary>
        public void AcceptChange()
        {
            Snapshot();
        }
    }
}
