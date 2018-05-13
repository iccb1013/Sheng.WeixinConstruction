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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Linkup.Common
{
    public static class ReflectionHelper
    {
        public static List<Type> GetTypeListBaseOn<T>()
        {
            List<Type> result = new List<Type>();

            Type baseOnType = typeof(T);
            Assembly assembly = Assembly.GetAssembly(baseOnType);
            if (baseOnType.IsInterface)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsAbstract || type.IsInterface)
                        continue;

                    if (type.GetInterface(baseOnType.Name) != null)
                    {
                            result.Add(type);
                    }
                }
            }
            else
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsAbstract || type.IsInterface)
                        continue;

                    if (type.IsSubclassOf(baseOnType))
                        result.Add(type);
                }
            }

            return result;
        }

        public static List<AttributeT> GetTypeAttributeList<AttributeT>(Type type)
        {
            List<AttributeT> list = new List<AttributeT>();
            object[] attributes = type.GetCustomAttributes(typeof(AttributeT), true);
            if (attributes.Length > 0)
            {
                foreach (var item in attributes)
                {
                    if (item is AttributeT)
                    {
                        AttributeT attribute = (AttributeT)item;
                        if (attribute != null)
                            list.Add(attribute);
                    }
                }
            }

            return list;
        }

        public static List<FieldInfo> GetAllFieldInfo(Type targetType)
        {
            List<FieldInfo> allFieldInfo = new List<FieldInfo>();
            if (targetType == null)
                return allFieldInfo;

            allFieldInfo.AddRange(targetType.GetFields().ToList());

            if (targetType.BaseType != null)
                allFieldInfo.AddRange(GetAllFieldInfo(targetType.BaseType));

            return allFieldInfo;
        }

        public static object Clone(object targetObject)
        {
            if (targetObject == null)
                return null;

            Type targetType = targetObject.GetType();

            if (targetType.IsValueType)
                return targetObject;

            if (targetType == String.Empty.GetType())
                return targetObject;

            object cloneObject = Activator.CreateInstance(targetType);

            Type targetlistType = targetType.GetInterface("IList");
            if (targetlistType != null)
            {
                IList list = (IList)targetObject;
                IList cloneList = (IList)cloneObject;
                foreach (var item in list)
                {
                    cloneList.Add(Clone(item));
                }
            }
            else
            {

                List<PropertyInfo> propertyList = targetType.GetProperties().ToList();

                foreach (PropertyInfo property in propertyList)
                {
                    if (property.CanWrite == false)
                        continue;

                    List<ParameterInfo> parameterInfoList = property.GetIndexParameters().ToList();
                    if (parameterInfoList.Count == 0)
                    {
                        object propertyValue = property.GetValue(targetObject, null);
                        object clonePropertyValue = Clone(propertyValue);
                        property.SetValue(cloneObject, clonePropertyValue, null);
                    }
                    //else
                    //{
                    //    foreach (ParameterInfo item in parameterInfoList)
                    //    {

                    //    }
                    //}
                }
            }

            return cloneObject;
        }

        /// <summary>
        /// 把 sourceObject 对象里的 property 值拷贝到 targetObject 中
        /// 不要求 targetObject 和 sourceObject 是同一类型
        /// 也不要求 property 完全一样，targetObject 中的 property 可以是 sourceObject 的子集
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="sourceObject"></param>
        public static void Inject(object targetObject, object sourceObject)
        {
            Inject(targetObject, sourceObject, null);
        }

        /// <summary>
        /// 把 sourceObject 对象里的 property 值拷贝到 targetObject 中
        /// 不要求 targetObject 和 sourceObject 是同一类型
        /// 也不要求 property 完全一样，targetObject 中的 property 可以是 sourceObject 的子集
        /// excludeProperties 指示忽略不注入的 targetObject 中的属性
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="sourceObject"></param>
        public static void Inject(object targetObject, object sourceObject, List<string> excludeProperties)
        {
            if (targetObject == null || sourceObject == null)
                return;

            Type targetType = targetObject.GetType();
            Type sourceType = sourceObject.GetType();

            foreach (PropertyInfo targetObjProperty in targetType.GetProperties())
            {
                PropertyInfo sourceObjProperty = sourceType.GetProperty(targetObjProperty.Name);
                if (sourceObjProperty == null)
                    continue;

                if (excludeProperties != null && excludeProperties.Contains(sourceObjProperty.Name))
                    continue;

                if (targetObjProperty.CanWrite == false)
                    continue;

                targetObjProperty.FastSetValue(targetObject, sourceObjProperty.FastGetValue(sourceObject));
            }
        }
    }
}
