using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkup.DataRelationalMapping
{
    public class AttachedWhereItem
    {
        /// <summary>
        /// 表中的字段名，不需要@
        /// </summary>
        public string Field
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// 使用 OR 连接的不同可选值
        /// </summary>
        public object[] ValueArray
        {
            get;
            set;
        }

        private AttachedWhereType _type = AttachedWhereType.Equal;
        public AttachedWhereType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public AttachedWhereItem()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field">表中的字段名，不需要@</param>
        /// <param name="value"></param>
        public AttachedWhereItem(string field, object value)
        {
            Field = field;
            Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field">表中的字段名，不需要@</param>
        /// <param name="valueArray"></param>
        public AttachedWhereItem(string field, object [] valueArray)
        {
            Field = field;
            ValueArray = valueArray;
        }

        public static List<AttachedWhereItem> Parse(Dictionary<string, object> attachedWhere)
        {
            List<AttachedWhereItem> list = new List<AttachedWhereItem>();

            if (attachedWhere == null || attachedWhere.Count == 0)
                return list;

            foreach (var item in attachedWhere)
            {
                AttachedWhereItem attachedWhereItem = new AttachedWhereItem();
                attachedWhereItem.Field = item.Key;
                attachedWhereItem.Value = item.Value;
                list.Add(attachedWhereItem);
            }

            return list;
        }
    }

    public enum AttachedWhereType
    {
        Equal,
        Like
    }
}
