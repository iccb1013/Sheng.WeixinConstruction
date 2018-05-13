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
using System.Runtime.Serialization;
using System.Text;

namespace Linkup.Common
{
    [Serializable]
    [DataContract]
    public class SnapshotBase : ISnapshot
    {
        [NonSerialized]
        private ObjectSnapshot _objectSnapshot;

        public SnapshotBase()
        {
            _objectSnapshot = new ObjectSnapshot(this);
        }

        public object Copy()
        {
            return this.MemberwiseClone();
        }

        public void Snapshot()
        {
            if (_objectSnapshot != null)
                _objectSnapshot.Snapshot();
        }

        public void AcceptChange()
        {
            if (_objectSnapshot != null)
                _objectSnapshot.AcceptChange();
        }

        /// <summary>
        /// 还原之后会把快照给清除掉
        /// </summary>
        public void Revert()
        {
            if (_objectSnapshot != null)
            {
                _objectSnapshot.Revert();
            }
        }

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            _objectSnapshot = new ObjectSnapshot(this);
        }

    }
}
