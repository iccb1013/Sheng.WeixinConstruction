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
using System.Reflection;
using System.Text;

namespace Linkup.DataRelationalMapping
{
    class TypeMappingDescription
    {
        public Type Type
        {
            get;
            private set;
        }

        private string _table;
        public string Table
        {
            get
            {
                if (String.IsNullOrEmpty(_table))
                    return Type.Name;

                return _table;
            }
            set
            {
                _table = value;
            }
        }

        private List<PropertyMappingDescription> _propertyList = new List<PropertyMappingDescription>();
        public List<PropertyMappingDescription> PropertyList
        {
            get { return _propertyList; }
            set { _propertyList = value; }
        }

        public TypeMappingDescription(Type type)
        {
            Type = type;

            Build();
        }

        private void Build()
        {
            if (Type == null)
                return;

            object[] attributes = Type.GetCustomAttributes(true);
            foreach (var item in attributes)
            {
                if (item is TableAttribute)
                {
                    TableAttribute att = (TableAttribute)item;
                    Table = att.Name;
                }
            }

            _propertyList.Clear();
            PropertyInfo[] propertyList = Type.GetProperties();
            foreach (PropertyInfo property in propertyList)
            {
                _propertyList.Add(new PropertyMappingDescription(property));
            }
        }
    }

    class PropertyMappingDescription
    {
        public string Name
        {
            get
            {
                if (_propertyInfo == null)
                    return String.Empty;

                return _propertyInfo.Name;
            }
        }

        private string _column;
        public string Column
        {
            get
            {
                if (String.IsNullOrEmpty(_column))
                    return Name;

                return _column;
            }
            private set
            {
                _column = value;
            }
        }

        private bool _notMapped = false;
        public bool NotMapped
        {
            get { return _notMapped; }
            private set { _notMapped = value; }
        }

        private bool _key = false;
        public bool Key
        {
            get { return _key; }
            private set { _key = value; }
        }

        private bool _json = false;
        public bool Json
        {
            get { return _json; }
            private set { _json = value; }
        }

        public OrderByAttribute OrderBy
        {
            get;
            private set;
        }

        public DataHelperConvertAttribute Convert
        {
            get;
            private set;
        }

        public RelationAttribute Relation
        {
            get;
            private set;
        }

        public bool IsRelation
        {
            get { return Relation != null; }
        }

        public PartialAttribute Partial
        {
            get;
            private set;
        }

        public bool IsPartial
        {
            get { return Partial != null; }
        }

        public bool CanRead
        {
            get
            {
                return _propertyInfo.CanRead;
            }
        }

        public bool CanWrite
        {
            get
            {
                return _propertyInfo.CanWrite;
            }
        }

        private PropertyInfo _propertyInfo;
        public PropertyInfo PropertyInfo
        {
            get { return _propertyInfo; }
            set { _propertyInfo = value; }
        }

        public PropertyMappingDescription(PropertyInfo property)
        {
            PropertyInfo = property;

            Build();
        }

        private void Build()
        {
            object[] attributeList = _propertyInfo.GetCustomAttributes(true);
            foreach (var attribute in attributeList)
            {
                if (attribute is ColumnAttribute)
                {
                    ColumnAttribute att = (ColumnAttribute)attribute;
                    Column = att.Name;
                }
                else if (attribute is NotMappedAttribute)
                {
                    NotMapped = true;
                }
                else if (attribute is JsonAttribute)
                {
                    Json = true;
                }
                else if (attribute is DataHelperConvertAttribute)
                {
                    Convert = (DataHelperConvertAttribute)attribute;
                }
                else if (attribute is RelationAttribute)
                {
                    Relation = (RelationAttribute)attribute;
                }
                else if (attribute is PartialAttribute)
                {
                    Partial = (PartialAttribute)attribute;
                }
                else if (attribute is KeyAttribute)
                {
                    Key = true;
                }
                else if (attribute is OrderByAttribute)
                {
                    OrderBy = (OrderByAttribute)attribute;
                }
            }
        }
    }
}
