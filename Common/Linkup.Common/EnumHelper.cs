using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Linkup.Common
{
    public static class EnumHelper
    {
        public static string GetEnumMemberValue(Enum enumValue)
        {
            if (enumValue == null)
                return null;

            Type type = enumValue.GetType();
            if (type.IsEnum == false)
                return String.Empty;

            //EnumConverter convert = new EnumConverter(type);

            //UserType.Customer | UserType.Staff;
            //enumValue.ToString()  "Staff, Customer"

            string result = String.Empty;
            string[] enumValueArray = enumValue.ToString().Split(',');
            foreach (string enumValueStr in enumValueArray)
            {
                FieldInfo fieldInfo = type.GetField(enumValueStr.Trim());
                Debug.Assert(fieldInfo != null, "type.GetField(enumValue.ToString()) 返回 null");
                if (fieldInfo == null)
                {
                    continue;
                }

                foreach (var item in fieldInfo.GetCustomAttributes(false))
                {
                    EnumMemberAttribute enumMember = item as EnumMemberAttribute;
                    if (enumMember == null)
                        continue;

                    result += enumMember.Value + ",";
                }
            }

            result = result.TrimEnd(',');
            return result;
        }

        public static T GetEnumFieldByMemberValue<T>(string memberValue)
        {
            Type type = typeof(T);
            if (type.IsEnum == false)
                return default(T);

            string result = String.Empty;

            string[] memberValueArray = memberValue.ToString().Split(',');

            //TODO:这里可以缓存
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo fieldInfo in fields)
            {
                foreach (var item in fieldInfo.GetCustomAttributes(false))
                {
                    EnumMemberAttribute enumMember = item as EnumMemberAttribute;
                    if (enumMember == null)
                        continue;

                    if (memberValueArray.Contains(enumMember.Value))
                    {
                        result += fieldInfo.Name + ",";
                    }
                }
            }
            result = result.TrimEnd(',');

            if (String.IsNullOrEmpty(result))
            {
                return default(T);
            }
            else
            {
                EnumConverter convert = new EnumConverter(type);
                return (T)Enum.Parse(type, result);
            }
        }

        public static T GetEnumFieldByValue<T>(string value)
        {
            Type type = typeof(T);
            if (type.IsEnum == false)
                return default(T);

            //TODO:这里可以缓存

            return (T)Enum.Parse(type, value);
        }

        public static EnumItemCollection<T> GetEnumItemCollection<T>() where T : struct
        {
            EnumItemCollection<T> list = new EnumItemCollection<T>();

            Type enumType = typeof(T);
            if (enumType.IsEnum == false)
                return list;

            List<FieldInfo> fieldList = ReflectionHelper.GetAllFieldInfo(enumType);

            foreach (var field in fieldList)
            {
                if (field.IsSpecialName)
                    continue;

                EnumConverter convert = new EnumConverter(enumType);
                T enumValue = (T)Enum.Parse(enumType, field.Name);

                EnumItem<T> item = new EnumItem<T>();
                item.Description = GetDescriptionAttributeValue((Enum)Enum.Parse(enumType, field.Name));
                item.EnumValue = enumValue;

                list.Add(item);
            }

            return list;
        }

        public static string GetDescriptionAttributeValue(Enum enumValue)
        {
            var result = String.Empty;

            if (enumValue == null)
                return result;
            else
                result = enumValue.ToString();

            Type type = enumValue.GetType();
            if (type.IsEnum == false)
                return result;

            FieldInfo fieldInfo = type.GetField(enumValue.ToString());
            if (fieldInfo == null)
            {
                //  Debug.Assert(false, "type.GetField(value.ToString()) == null，value：" + value);
                return result;
            }

            foreach (var item in fieldInfo.GetCustomAttributes(false))
            {
                DescriptionAttribute enumDescription = item as DescriptionAttribute;
                if (enumDescription == null)
                    continue;

                return enumDescription.Description;
            }

            return result;
        }
       
    }

    public class EnumItem<T>
    {
        public string Description
        {
            get;
            set;
        }

        public T EnumValue
        {
            get;
            set;
        }

        public EnumItem()
        {

        }

        public EnumItem(string description, T enumValue)
        {
            Description = description;
            EnumValue = enumValue;
        }
    }

    public class EnumItemCollection<T>:Collection<EnumItem<T>>
    {

    }
}
