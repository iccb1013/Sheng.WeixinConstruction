using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Linkup.Common;

namespace Linkup.Common
{
    class SnapshotMemberCollection:CollectionBase
    {
        #region 基础构造，属性，方法

        public SnapshotMemberCollection()
        {
        }

        public SnapshotMemberCollection(SnapshotMemberCollection value)
        {
            this.AddRange(value);
        }

        public SnapshotMemberCollection(SnapshotMember[] value)
        {
            this.AddRange(value);
        }

        public SnapshotMember this[int index]
        {
            get
            {
                return ((SnapshotMember)(List[index]));
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(SnapshotMember value)
        {
            return List.Add(value);
        }

        public void AddRange(SnapshotMember[] value)
        {
            for (int i = 0; (i < value.Length); i = (i + 1))
            {
                this.Add(value[i]);
            }
        }

        public void AddRange(SnapshotMemberCollection value)
        {
            for (int i = 0; (i < value.Count); i = (i + 1))
            {
                this.Add(value[i]);
            }
        }

        public bool Contains(SnapshotMember value)
        {
            return List.Contains(value);
        }

        public void CopyTo(SnapshotMember[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public int IndexOf(SnapshotMember value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, SnapshotMember value)
        {
            List.Insert(index, value);
        }

        public new SnapshotMemberEnumerator GetEnumerator()
        {
            return new SnapshotMemberEnumerator(this);
        }

        public void Remove(SnapshotMember value)
        {
            List.Remove(value);
        }

        #endregion

        #region 加的属性或方法

        /// <summary>
        /// 获取以,号分隔的所有属性(Property)名
        /// </summary>
        public string Members
        {
            get
            {
                string members = String.Empty;
                foreach (SnapshotMember member in this)
                {
                    members += member.MemberName + ",";
                }

                return members.TrimEnd(',');
            }
        }

        /// <summary>
        /// 使用指定的参数添加一个Member对象
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public int Add(string memberName, object oldValue, object newValue)
        {
            return List.Add(new SnapshotMember(memberName, oldValue, newValue));
        }

        #endregion

        #region SnapshotMemberEnumerator

        public class SnapshotMemberEnumerator : object, IEnumerator
        {

            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            public SnapshotMemberEnumerator(SnapshotMemberCollection mappings)
            {
                this.temp = ((IEnumerable)(mappings));
                this.baseEnumerator = temp.GetEnumerator();
            }

            public SnapshotMember Current
            {
                get
                {
                    return ((SnapshotMember)(baseEnumerator.Current));
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return baseEnumerator.Current;
                }
            }

            public bool MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            bool IEnumerator.MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            public void Reset()
            {
                baseEnumerator.Reset();
            }

            void IEnumerator.Reset()
            {
                baseEnumerator.Reset();
            }
        }

        #endregion
    }
}
