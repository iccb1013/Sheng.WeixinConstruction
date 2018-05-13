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
    /// <summary>
    /// 可撤销工作单元中表示变更成员的对象
    /// </summary>
    class SnapshotMember
    {
        #region 构造

        public SnapshotMember()
        {

        }

        public SnapshotMember(string memberName,object oldValue,object newValue)
        {
            this.MemberName = memberName;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        #endregion

        #region 公开属性

        private string[] _memberNames;
        private string _memberName;
        /// <summary>
        /// 更改的属性（Property）名称
        /// </summary>
        public string MemberName
        {
            get { return this._memberName; }
            set
            {
                this._memberName = value;
                _memberNames = value.Split('/');
            }
        }

        private object _oldValue;
        /// <summary>
        /// 旧值
        /// </summary>
        public object OldValue
        {
            get { return this._oldValue; }
            set { this._oldValue = value; }
        }

        private object _newValue;
        /// <summary>
        /// 新值
        /// </summary>
        public object NewValue
        {
            get { return this._newValue; }
            set { this._newValue = value; }
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 为指定的对象设置属性（Property）值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="enumMemberValue"></param>
        public void SetMember(object obj, EnumMemberValue enumMemberValue)
        {
            PropertyInfo propertyInfo = null;
            object currentObj = obj; 
            for (int i = 0; i < this._memberNames.Length; i++)
            {
                //如果已经到了属性(Property)路径的底层
                if (i == this._memberNames.Length - 1)
                {
                    propertyInfo = currentObj.GetType().GetProperty(this._memberNames[i]);
                }
                else
                {
                    //如果还没有到路径底层
                    currentObj = currentObj.GetType().GetProperty(this._memberNames[i]).GetValue(currentObj, null);

                    //此时currentObj有可能为null，如窗体元素的字体，就可能发生为null的情况
                    //但是照正常逻辑，应该不会出现这种情况，这里还是做个判断，防止程序意外出错
                    if (currentObj == null)
                        return;
                }
            }

            if (propertyInfo == null)
                return;

            object value;

            if (enumMemberValue == EnumMemberValue.NewValue)
                value = this.NewValue;
            else
                value = this.OldValue;

            try
            {
                if (propertyInfo.CanWrite)
                {
                    if (propertyInfo.PropertyType.IsEnum)
                    {
                        propertyInfo.SetValue(currentObj, Enum.Parse(propertyInfo.PropertyType, value.ToString()), null);
                    }
                    else
                    {
                        propertyInfo.SetValue(currentObj, value, null);

                        //忘了当时为什么要加Convert.ChangeType
                        //try
                        //{
                            //在调用Convert.ChangeType 时，被转换的对象必须实现 IConvertible
                            //propertyInfo.SetValue(currentObj, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                        //}
                        //catch (InvalidCastException ex)
                        //{
                        //    Debug.Assert(false, ex.Message);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
        }

        public override string ToString()
        {
            if (this.MemberName == null)
            {
                return String.Empty;
            }

            StringBuilder str = new StringBuilder(this.MemberName.Length);
            str.Append(this.MemberName);

            str.Append(":");

            str.Append("NewValue:");
            if (this.NewValue != null)
                str.Append(this.NewValue.ToString());
            else
                str.Append("Null");

            str.Append(",");

            str.Append("OldValue:");
            if (this.OldValue != null)
                str.Append(this.OldValue.ToString());
            else
                str.Append("Null");

            return str.ToString();
        }

        #endregion

        #region EnumMemberValue

        public enum EnumMemberValue
        {
            OldValue,
            NewValue
        }

        #endregion
    }
}
