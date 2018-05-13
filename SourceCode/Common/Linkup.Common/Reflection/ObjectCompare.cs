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
using System.Reflection;
using System.Diagnostics;

namespace Linkup.Common
{
    public class ObjectCompare
    {
        /// <summary>
        /// 获取两个同类型的对象实例的公共属性差异
        /// 用sourceObject去比较compareObject
        /// 以sourceObject为基础返回compareObject与它的差异
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="compareObject"></param>
        /// <returns></returns>
        public static List<ObjectCompareResult> Compare(object sourceObject, object compareObject)
        {
            Type sourceType = sourceObject.GetType();
            Type compareType = compareObject.GetType();

            if (sourceType.Equals(compareType) == false)
            {
                Debug.Assert(false, "比较的对象的类型不一样");
                throw new ArgumentException();
            }

            List<ObjectCompareResult> result = new List<ObjectCompareResult>();

            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            object sourceValue, compareValue;
            foreach (PropertyInfo property in sourceProperties)
            {
                if (property.CanRead == false || property.CanWrite == false)
                    continue;

                object [] attrs = property.GetCustomAttributes(typeof(ObjectCompareAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    ObjectCompareAttribute attr = (ObjectCompareAttribute)attrs[0];
                    if (attr.Ignore)
                        continue;
                }

                sourceValue = property.GetValue(sourceObject, null);
                compareValue = property.GetValue(compareObject, null);

                if (sourceValue != null)
                {
                    if (sourceValue.Equals(compareValue) == false)
                    {
                        result.Add(new ObjectCompareResult(property.Name, sourceValue, compareValue));
                    }
                }
                else
                {
                    if (compareValue != null)
                    {
                        if (compareValue.Equals(sourceValue) == false)
                        {
                            result.Add(new ObjectCompareResult(property.Name, sourceValue, compareValue));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 用 usageObject 中属性的值，更新 sourceObject
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="usageObject"></param>
        public static void Update(object sourceObject, object usageObject)
        {
            if (sourceObject == null || usageObject == null)
                return;

            SnapshotMemberCollection members = new SnapshotMemberCollection();
            foreach (ObjectCompareResult result in ObjectCompare.Compare(sourceObject, usageObject))
            {
                members.Add(result.MemberName, result.SourceValue, result.CompareValue);
            }

            foreach (SnapshotMember member in members)
            {
                member.SetMember(sourceObject, SnapshotMember.EnumMemberValue.NewValue);
            }
        }

    }

    public class ObjectCompareResult
    {
        public string MemberName
        {
            get;
            private set;
        }

        public object SourceValue
        {
            get;
            private set;
        }

        public object CompareValue
        {
            get;
            private set;
        }

        public ObjectCompareResult(string memberName, object sourceValue, object compareValue)
        {
            this.MemberName = memberName;
            this.SourceValue = sourceValue;
            this.CompareValue = compareValue;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ObjectCompareAttribute : Attribute
    {
        private bool _ignore = false;
        /// <summary>
        /// 忽略，不参与比较
        /// </summary>
        public bool Ignore
        {
            get { return _ignore; }
            set { _ignore = value; }
        }
    }
}
