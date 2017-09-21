using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkup.DataRelationalMapping
{
    /// <summary>
    /// 修饰属性，表示属性的对象 是 当前数据集的一部分字段所表示的子对象
    /// </summary>
    public class PartialAttribute : RelationalMappingAttribute
    {
        private string _fieldRelationship;
        public string FieldRelationship
        {
            get { return _fieldRelationship; }
            set
            {
                _fieldRelationship = value;

                BuildFieldDictionary();
            }
        }

        private Dictionary<string, string> _fieldDictionary;
        public Dictionary<string, string> FieldDictionary
        {
            get { return _fieldDictionary; }
            set { _fieldDictionary = value; }
        }

        public PartialAttribute()
        {

        }

        private void BuildFieldDictionary()
        {
            _fieldDictionary = new Dictionary<string, string>();

            if (String.IsNullOrEmpty(_fieldRelationship))
                return;

            string[] fieldList = _fieldRelationship.Split(',');
            foreach (var field in fieldList)
            {
                string[] fieldPair = field.Split('=');
                _fieldDictionary.Add(fieldPair[0], fieldPair[1]);
            }
        }
    }
}
